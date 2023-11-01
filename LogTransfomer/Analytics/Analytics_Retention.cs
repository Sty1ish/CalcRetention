using LogTransfomer.Analytics.Calculator.Data;
using LogTransfomer.Analytics.Data;
using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer.Analytics
{
    /// <summary>
    /// 리텐션 집계, DB기록
    /// - Classic, Rolling
    /// - 조건 : 국가, 마켓
    /// </summary>
    public class Analytics_Retention
    {
        private bool _is_rollup = false;
        private readonly DateTime _firstDate;                                               // 시작일
        private readonly DateTime _lastDate;                                                // 종료일
        private readonly IEnumerable<string> _countryList;                                  // 객체에서 가지고 있는 국가 정보
        private readonly DateTime _ReleaseDate = new DateTime(2023, 08, 12);                // 출시일 (해당일 이전 데이터 X)
        private readonly List<int> _calcDates = new List<int> { 1,2,3,4,5,6,7,14,21,28,60}; // 리텐션 연산할 기준일
      
        private readonly Dictionary<DateTime, List<UniquePidData>> _uniquePidDatas;         // Calculator에서 집계된 로그 전체 DB
        private Dictionary<DateTime, List<UniquePidData>> _fillterPidDatas;                 // 원본 데이터에서 필터가 적용된 데이터

        private int _currentMarket;                                                         // 현재 필터가 적용된 마켓
        private string _currentCountry;                                                     // 현재 필터가 적용된 국가

        public IEnumerable<string> countryList() => _countryList;                                  // return countryList()

        public Analytics_Retention(Dictionary<DateTime, List<UniquePidData>> data)
        {
            _uniquePidDatas = data;
            _fillterPidDatas = data;


            var firstDate = data.OrderBy(x => x.Key)
                .Select(x => x.Key)
                .First();

            // firstDate는 출시일보다는 뒤에 존재해야.
            if (firstDate < _ReleaseDate)
            {
                _firstDate = _ReleaseDate;
            }
            else 
            { 
                _firstDate = firstDate;
            }

            _lastDate = data.OrderByDescending(x => x.Key)
                .Select(x => x.Key)
                .First();

            // 데이터 내 모든 국가 탐색으로 변경
            //_countryList = (from x in data[_firstDate] select x.CountryCode).Distinct();
            var countryList = new List<string>();
            foreach (var (key, val) in data)
            {
                countryList.AddRange(data[key].Select(x => x.CountryCode).Distinct().ToList());
            }
            _countryList = countryList.Distinct();

        }
        


        /// <summary>
        /// 보유중인 리스트에서, 시작-종료일의 Pid값 중복 개수
        /// </summary>
        public int Analytics_DayDiff(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            var startPids = _fillterPidDatas[startDate]
                .Select(x => x.Pid)
                .Distinct();

            var endPids = _fillterPidDatas[endDate]
                .Select(x => x.Pid)
                .Distinct();

            var userDateDiff = startPids.Intersect(endPids)
                .Count();

            return userDateDiff;
        }


        /// <summary>
        /// 인풋 받은 데이터 (기준일 ~ 기준일 + n일)를 기준으로 기준일의 리텐션 연산
        /// </summary>
        public List<AnalyData_RetentionData> SummariseRetention(DateTime calc_date)
        {
            // condition chk
            if (calc_date < _firstDate)
            { 
                Console.WriteLine($"계산일({calc_date})은 데이터의 시작일({_firstDate})보다 작을 수 없습니다");
                return null;
            }
            
            if (calc_date > _lastDate) 
            { 
                Console.WriteLine($"계산일({calc_date})은 데이터의 종료일({_lastDate})보다 클 수 없습니다");
                return null;
            }

            // init
            var total = Analytics_DayDiff(calc_date, calc_date);
            var data = new List<AnalyData_RetentionData>();

            var checkDate = (_lastDate - calc_date).Days;
            
            for (int i = 1; i <= checkDate; i++)            // int i = 1인 이유 : target date와 calc date의 0일 차이는 연산 불필요
            {
                var targetDate = calc_date.AddDays(i);

                // 기호님이 말씀하신 부분. / 1~7일과 해당일만 연산하면 데이터 양 절약 가능. 필수적인거만 쿼리하고 보간은 뭐... 통계 써야지.
                if (false == _calcDates.Contains((targetDate - calc_date).Days))
                {
                    continue;
                }

                var line = new AnalyData_RetentionData()
                {
                    Country = _currentCountry,
                    Market = _currentMarket,
                    DateTime = calc_date,
                    DayCount = (targetDate - calc_date).Days,
                    TotalPidCount = total,
                    RetentionCount = Analytics_DayDiff(calc_date, targetDate),
                };

                data.Add(line);
            }
            return data;
        }


        /// <summary>
        /// _filterPidData를 필터링하는 구문 / Rolling 연산을 위해 pid 중복 검사.
        /// targetDate에 접속한 유저를 기준으로, Pid값을 Rollup
        /// for문 안에서 재 선언시, Rollback을 이용해 중복 Rollup을 방지해야함.
        /// </summary>
        public void RollupData(DateTime TargetDate)
        {
            if (true == _is_rollup) 
            {
                Console.WriteLine("Already Rollup Data -> Automatically execute RollbackData();");
                Console.WriteLine("Rollup 함수 재 사용시 명시적으로 RollbackData();를 사용해주세요");

                RollbackData();
            }
            
            _is_rollup = true;

            var startPidData = _uniquePidDatas[TargetDate]
                .Select(x => x.Pid)
                .Distinct()
                .ToList();

            var orderedDict = _uniquePidDatas
                .OrderByDescending(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);

            var tempData = new List<UniquePidData>();

            // _endDate에서 _firstDate까지 다음 작업을 반복
            foreach (var (key, val) in orderedDict)
            {
                _fillterPidDatas[key].AddRange(tempData);

                tempData.AddRange(val.Where(x => startPidData.Contains(x.Pid))
                    .Distinct()
                    .ToList()); // 클래스의 Instersect는 검사 방법 별도 정의 필요.
            }
        }

        /// <summary>
        /// Filtering Country and Market
        /// </summary>
        public void fillterData(string country, MarketType market)
        {
            // country
            foreach (var (key, val) in _fillterPidDatas)
            {
                _fillterPidDatas[key] = val
                    .Where(x => x.CountryCode == country).ToList();
                _currentCountry = country;
            }
            // market
            foreach (var (key, val) in _fillterPidDatas)
            {
                _fillterPidDatas[key] = val
                    .Where(x => x.MarketType.Equals(market)).ToList();
                _currentMarket = (int)market;
            }
        }

        /// <summary>
        /// reset _fillterPidDatas 
        /// </summary>
        public void RollbackData()
        {
            _fillterPidDatas = _uniquePidDatas.ToDictionary(x => x.Key, x => x.Value);
            _is_rollup = false;
        }
    }
}


/*
 CREATE TABLE `CONVERT_JSONTBL_Retention` (
`Id` INT(11) NOT NULL AUTO_INCREMENT,
`Date` DATE NULL DEFAULT NULL,
`Details` JSON,
PRIMARY KEY (`Id`) USING BTREE
)
 */
