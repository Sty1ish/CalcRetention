namespace BI.APIServer.DataLoader
{
    using BI.Core.Common.Analytics.Data;
    using BI.Core.Database.Analytics;
    using BI.Core.Utils;

    /// <summary>
    /// RetentionManager : 조건에 맞는 Retention 데이터 연산 결과 반환
    /// </summary>
    public class dataLoaderRetention
    {
        private readonly ILogger<dataLoaderRetention> _logger;

        private readonly DBAnalytics _analyticsDB;

        private const string TABLE_ANALYTICS = "TBL_Analytics";

        public dataLoaderRetention(IServiceProvider serviceProvider, DBAnalytics analyticsDB)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<dataLoaderRetention>();

            _analyticsDB = analyticsDB;
        }

        /// 시작일 ~ 종료일이 기준일인 Retention Data를 반환함 (60일전 60일, 59일전 60일. 68일전 60일)
        public List<RetentionData> GetRentionDatasRange(DateTime targetDay, int dayCount, List<string> countries, List<string> markets)
        {
            var firstDay = targetDay.AddDays(-dayCount);
            var lastDay = targetDay.AddDays(dayCount);

            // load Data.
            var pidDatas = GetPidDatasRange(firstDay, lastDay);

            var retentionDatas = new List<RetentionData>();

            // analytics retention Range
            for (int i = 0; i <= dayCount; i++)
            {
                var dayRetentions = GetDayRentionDatas(pidDatas, targetDay.AddDays(i), dayCount, countries, markets);

                retentionDatas.AddRange(dayRetentions);
            }

            return retentionDatas;
        }

        // 요청에 조건이 포함된 경우 (매개변수 4종)
        /// 계산일 x daycount개수만큼 반환 (기준일만 연산)
        public List<RetentionData> GetDayRentionDatas(Dictionary<DateTime, IEnumerable<UniquePidData>> pidDataDic, 
            DateTime targetDay, int dayCount, List<string> countries, List<string> markets)
        {
            // start info.
            var startDay = targetDay.AddDays(dayCount * -1);
            var startPidDatas = FilterPidData(pidDataDic[startDay], countries, markets);

            // data check.
            _logger.LogInformation($"[Analytics Retention] - {startDay.ToString("yyyy-MM-dd")} : {startPidDatas.Count()} Data Queryed");

            // Init calculation.
            var totalRetentionCount = GetRetentionCount(startPidDatas, startPidDatas);
            var retentionDatas = new List<RetentionData>();

            for (int i = 1; i <= dayCount; i++) 
            {
                var calcDay = startDay.AddDays(i);
                var calcData = pidDataDic[calcDay];

                var retentionCount = GetRetentionCount(startPidDatas, calcData);

                // 이 객체의 리스트를 저장 / 반환해야함.
                retentionDatas.Add(new RetentionData
                {
                    Date = startDay,
                    DayCount = i,
                    TotalPidCount = totalRetentionCount,
                    RetentionCount = retentionCount
                });
            }

            return retentionDatas;
        }

        /// <summary>
        /// Filter pid datas with countries and markets.
        /// </summary>
        private IEnumerable<UniquePidData> FilterPidData(IEnumerable<UniquePidData> pidDatas, List<string> countries, List<string> markets)
        {
            if (countries != null)
            {
                pidDatas = pidDatas.Where(x => countries.Contains(x.CountryCode)).ToList();
            }

            if (markets != null)
            {
                pidDatas = pidDatas.Where(x => markets.Contains(((int)x.MarketType).ToString())).ToList();
            }

            return pidDatas;
        }

        /// A 객체와, B 객체의 Pid값을 비교 후, 교집합의 수를 반환한다.
        public int GetRetentionCount(IEnumerable<UniquePidData> x, IEnumerable<UniquePidData> y)
        {
            var startPids = x.Select(x => x.Pid).Distinct();
            var targetPids = y.Select(y => y.Pid).Distinct();

            return startPids.Intersect(targetPids).Count();
        }

        /// <summary>  
        /// 0일(targetDay)부터, N일 전(QueryDate)까지 데이터를 로드함.
        /// </summary>
        public Dictionary<DateTime, IEnumerable<UniquePidData>> GetPidDatasRange(DateTime firstDay, DateTime lastDay)
        {
            if (true == TimeUtil.IsPastTime(firstDay, lastDay))
            {
                return new Dictionary<DateTime, IEnumerable<UniquePidData>>();
            }
            
            var pidDataDic = new Dictionary<DateTime, IEnumerable<UniquePidData>>();

            for (var day = firstDay; false == TimeUtil.IsSameDay(day, lastDay.AddDays(1)); day = day.AddDays(1)) 
            {
                var dailyPidData = _analyticsDB.GetDailyPidData(TABLE_ANALYTICS, day);
                pidDataDic.Add(day, dailyPidData);
            }

            return pidDataDic;
        }
    }
}
