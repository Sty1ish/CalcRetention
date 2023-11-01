using LogTransfomer.Analytics.Calculator.Data;
using LogTransfomer.Database;
using LogTransfomer.Database.GameLog;
using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer.Analytics.Calculator
{
    /// <summary>
    /// Retention 계산
    /// - DB에서 날짜기준 데이터를 가져와서 병합, 반환
    /// </summary>
    public class Calculator_Retention
    {
        private readonly GameLogDB _gameLogDB;

        public Calculator_Retention(string _ipAddress, string _userName, string _password) // INIT
        {
            _gameLogDB = new GameLogDB();
            _gameLogDB.Initialize(_ipAddress, _userName, _password);
        }

        /// <summary>
        /// "기준일" 기준 "INIT, Status" 로그에 찍힌 "연산 날짜"에 맞는 Pid/국가/마켓값 반환
        /// Dictionary<DateTime, Unique List<PidDataType>>로 반환됨
        /// </summary>
        public Dictionary<DateTime, List<UniquePidData>> GetUniquePids(DateTime startDate, int dayCount)
        {
            var uniquePlayerPids = new Dictionary<DateTime, List<UniquePidData>>();

            // config
            Console.WriteLine("DailyKPI - Calc_UniquePid");
            startDate = startDate.Date;
            // LogType Setting
            var logTypes = new List<LogType>();
            logTypes.Add(LogType.Init);
            logTypes.Add(LogType.Status_Info);


            for (int i = 0; i <= dayCount; i++)
            {
                var targetDate = startDate.AddDays(i);
                var uniqueLogDatas = new List<UniquePidData>();

                // 가져온 Data에서 Pid를 distinct
                var logDatas = GetGameLogData(targetDate)
                    .DistinctBy(d => d.Pid);

                uniqueLogDatas.AddRange(logDatas);

                // add dictionary
                uniquePlayerPids.Add(targetDate, uniqueLogDatas);
            }

            return uniquePlayerPids;
        }

        /// <summary>
        /// "기준일"의 1~9번 테이블 쿼리 결과 반환
        /// List<PidDataType>을 생성하는 역할
        /// </summary>
        /// <param name="targetDate"></param>
        public List<UniquePidData> GetGameLogData(DateTime targetDate, List<LogType> logTypes = null)
        {
            var pidDatas = new List<UniquePidData>(); // pid가 필요하면 사용
            var logDatas = new List<GameLogData>(); // log가 필요하면 사용

            var targetMonth = targetDate.ToString("yyyyMM");

            var databaseName = $"GameLog_{targetMonth}";
            var tableNameBase = $"tb_gamelog_{targetDate.ToString("yyyyMMdd")}";

            for (int i = 1; i < 10; i++)
            {
                var tableName = $"{tableNameBase}_{i}";

                var datas = _gameLogDB.GetLogDatas(databaseName, tableName, logTypes);

                if (null == datas)
                {
                    continue;
                }
                else 
                {
                    // GameLogDB의 Select * 결과의 일부만 저장, 반환
                    foreach (var data in datas)
                    {
                        var line = new UniquePidData()
                        {
                            Pid = data.Pid,
                            CountryCode = data.CountryCode,
                            MarketType = data.MarketType,
                        };
                        pidDatas.Add(line);
                    }
                }
            }
            return pidDatas;
        }
    }
}
