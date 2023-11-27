using Dapper;
using MySqlConnector;
using System.Data;

namespace BI.Core.Database.Analytics
{
    using BI.Core.Common.Analytics.Types;
    using MySqlConnector;
    using System.Data;

    public partial class DBAnalytics
    {
        private const string TABLE_NRU = "TBL_Analytics";

        /// <summary>
        /// Insert retention day data.
        /// </summary>
        public bool InsertNRUDayData(DateTime date, byte[] details, AnalyticsType eventType)
        {
            var connString = GetDBConnectionString(DBNAME_REDASH);
            var tableName = TABLE_NRU;

            // 커넥션이 생기지 않는 경우 Null 반환
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                // Update or Insert가 아니라, 해당하는 위치 값을 지우고 > 삽입하면 1개 절차.

                // 쿼리시 해당 값이 존재하는지 확인
                DeleteNRUDayData(tableName, date, (int)eventType);

                var sql = $"INSERT INTO {tableName} (logdate, Details, logtype, regtime) VALUES (@logdate, @Details, @logtype, @regtime)";

                var rowAffect = conn.Execute(
                    sql: sql,
                    param: new
                    {
                        logdate = date.ToString("yyyy-MM-dd"),
                        Details = details,
                        logtype = (int)eventType,
                        regtime = DateTime.Now
                    },
                    commandTimeout: 60,
                    commandType: CommandType.Text
                    );

                if (rowAffect > 0)
                {
                    return true; // 성공시 true
                }
            }
            return false; // 실패시 false 반환
        }

        /// <summary>
        /// Update or Insert문을 결정하기 위한 쿼리문
        /// 값이 하나라도 반환되면 True, 반환되지 않으면 False 반환
        /// 이게 존재 체크가 아니라, 그냥 해당 조건 Input하면 값 지우는 역할 수행하면 끝,
        /// </summary>
        public void DeleteNRUDayData(string tableName, DateTime date, int logtype)
        {
            var connString = GetDBConnectionString(DBNAME_REDASH);

            var sql = $"DELETE FROM {tableName} WHERE logdate=@logdate AND logtype=@LogType";

            using (var conn = new MySqlConnection(connString))
            {
                conn.Execute(
                    sql: sql,
                    param: new
                    {
                        logdate = date.ToString("yyyy-MM-dd"),
                        Logtype = logtype,
                    },
                    commandTimeout: 60,
                    commandType: CommandType.Text
                    );
            }
        }
    }
}
