using BI.Core.Common.GameLog.Data;
using BI.Core.Common.GameLog.Types;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;

namespace BI.Core.Database.GameLog
{
    public class DBGameLog
    {
        private ILogger<DBGameLog> _logger;
        private string _connectionString { get; set; }

        public DBGameLog(IServiceProvider serviceProvider, string connectionString)
        {
            var loggerFactyory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactyory.CreateLogger<DBGameLog>();
            _connectionString = connectionString;
        }

        public string GetConnectionString(string dbName)
        {
            var builder = new MySqlConnectionStringBuilder(_connectionString);
            builder.Database = dbName;
            return builder.ConnectionString;
        }

        /// <summary>
        /// 기준일에, 리스트로 인풋되는 로그 타입과 일치하는 데이터베이스 전체를 반환한다.
        /// </summary>
        public IEnumerable<GameLogData> GetLogDatas(string databaseName, string tableName, List<GameLogType> logTypes = null)
        {
            var gameLogDatas = new List<GameLogData>();
            var connString = GetConnectionString(databaseName);

            var sql = $"SELECT * FROM {tableName} WHERE logtype IN @LogTypes";

            if (logTypes == null)
            {
                logTypes = new List<GameLogType>();
                foreach (var type in Enum.GetValues<GameLogType>())
                {
                    logTypes.Add(type);
                }
            }

            // 커넥션이 생기지 않는 경우 Null + 에러메세지 반환
            try
            {
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    var logDatas = conn.Query<GameLogData>(
                        sql: sql,
                        param: new
                        {
                            TableName = tableName,
                            LogTypes = logTypes,
                        },
                        commandTimeout: 60,
                        commandType: CommandType.Text
                        );

                    gameLogDatas.AddRange(logDatas);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.ToString());
            }

            return gameLogDatas;
        }

        public IEnumerable<string> GetTableList(string databaseName)
        {
            var connString = GetConnectionString(databaseName);

            using (var conn = new MySqlConnection(connString))
            {
                var gameLogDatas = conn.Query<string>(
                    sql: "SHOW TABLES",
                    commandTimeout: 60,
                    commandType: CommandType.Text
                    );

                return gameLogDatas;
            }
        }
    }
}
