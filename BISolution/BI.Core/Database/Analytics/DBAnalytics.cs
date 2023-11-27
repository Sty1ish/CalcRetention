using BI.Core.Common.Analytics.Data;
using BI.Core.Common.Analytics.Types;
using Dapper;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;
using BI.Core.Utils;

namespace BI.Core.Database.Analytics
{

    public partial class DBAnalytics
    {
        private ILogger<DBAnalytics> _logger;

        private string _connectionString { get; set; }
        private const string DBNAME_REDASH = "redash";

        public DBAnalytics(IServiceProvider serviceProvider, string connectionString)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            _logger = loggerFactory.CreateLogger<DBAnalytics>();

            _connectionString = connectionString;
        }

        /// <summary>
        /// Get Database connection string.
        /// </summary>
        public string GetDBConnectionString(string dbName)
        {
            var builder = new MySqlConnectionStringBuilder(_connectionString);
            builder.Database = dbName;
            return builder.ConnectionString;
        }

        /// <summary>
        /// 기준일에, 리스트로 인풋되는 로그 타입과 일치하는 데이터베이스 전체를 반환한다.
        /// 이 구문이 크게 에러난것으로 파악됨. 전체 리메이크 해야함.
        /// </summary>
        public List<UniquePidData> GetDailyPidData(string tableName, DateTime datetime)
        {
            var uniquePidDatas = new List<UniquePidData>();
            var connString = GetDBConnectionString(DBNAME_REDASH);

            // Set query.
            var sql = $"SELECT logdate, Details FROM {tableName} WHERE logdate=@DateVal AND logtype = @LogType LIMIT 1";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    var gameLogData = conn.QueryFirstOrDefault<AnalyData_RetentionData>(
                        param: new {
                            DateVal = datetime.ToString("yyyy-MM-dd"),
                            LogType = AnalyticsType.UniquePids,
                        },
                        sql: sql,
                        commandTimeout: 60,
                        commandType: CommandType.Text
                        );

                    if (gameLogData != null)
                    {
                        var bytePids = gameLogData.Details;
                        var unserial_data = SerializeUtil.Deserialize<List<UniquePidData>>(bytePids);
                        uniquePidDatas.AddRange(unserial_data);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.ToString());
            }

            return uniquePidDatas;
        }
    }

}


/*
 Redash 테이블 스키마 정리

CREATE TABLE `TBL_Analytics` (
	`Id` INT(11) NOT NULL AUTO_INCREMENT,
	`Date` DATE NULL DEFAULT NULL,
	`Details` LONGBLOB NULL DEFAULT NULL,
	`logtype` INT(11) NULL DEFAULT NULL,
	`regtime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`Id`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=214
;
*/