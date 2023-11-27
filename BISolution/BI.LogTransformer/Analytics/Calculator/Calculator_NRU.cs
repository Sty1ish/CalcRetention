using BI.Core.Common.Analytics.Data;
using BI.Core.Common.GameLog.Types;
using BI.Core.Database.GameLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.LogTransformer.Analytics.Calculator
{
    public class Calculator_NRU
    {
        private readonly ILogger<Calculator_NRU> _logger;

        private readonly DBGameLog _gameLogDB;

        public Calculator_NRU(IServiceProvider serviceProvider, DBGameLog gameLogDB)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Calculator_NRU>();
            _gameLogDB = gameLogDB;
        }


        /// <summary>
        /// List<PidDataType>을 생성하는 역할
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
