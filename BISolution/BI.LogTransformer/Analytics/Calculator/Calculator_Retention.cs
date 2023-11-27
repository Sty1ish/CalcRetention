using BI.Core.Common.Analytics.Data;
using BI.Core.Common.GameLog.Types;
using BI.Core.Database.GameLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BI.LogTransformer.Analytics.Calculator
{
    /// <summary>
    /// Retention 계산
    /// - DB에서 날짜기준 데이터를 가져와서 병합, 반환
    /// </summary>
    public class Calculator_Retention
    {
        private readonly ILogger<Calculator_Retention> _logger;

        private readonly DBGameLog _gameLogDB;

        public Calculator_Retention(IServiceProvider serviceProvider, DBGameLog gameLogDB)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Calculator_Retention>();
            _gameLogDB = gameLogDB;
        }


        /// <summary>
        /// "기준일"의 1~9번 테이블 쿼리 결과 반환
        /// List<PidDataType>을 생성하는 역할
        /// 테이블 리스트를 확인후 쿼리하는 방안이 맞다. (변경해야 할 요소)
        /// </summary>
        public List<UniquePidData> GetGameLogData(DateTime targetDate, List<GameLogType> logTypes = null)
        {
            var uniquePidDatas = new List<UniquePidData>(); // pid가 필요하면 사용

            var targetMonthString = targetDate.ToString("yyyyMM");

            var dbName = $"GameLog_{targetMonthString}";
            var tableNameBase = $"tb_gamelog_{targetDate.ToString("yyyyMMdd")}";

            var existTableNames = _gameLogDB
                .GetTableList(dbName)
                .Where(x => x.Contains(tableNameBase));

            // 테이블 목록을 가져옴
            foreach (var tableName in existTableNames) 
            {
                var datas = _gameLogDB.GetLogDatas(dbName, tableName, logTypes);

                if (null == datas)
                {
                    continue;
                }

                // GameLogDB의 Select * 결과의 일부만 저장, 반환
                foreach (var data in datas)
                {
                    uniquePidDatas.Add(new UniquePidData
                    {
                        Pid = data.Pid,
                        CountryCode = data.Country_Code,
                        MarketType = data.Market_Type,
                    });
                }
            }

            return uniquePidDatas;
        }
    }
}
