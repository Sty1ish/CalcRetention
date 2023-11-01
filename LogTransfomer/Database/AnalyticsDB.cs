using MySql.Data.MySqlClient;
using System.Text;
using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer.Database
{
    public class AnalyticsDB
    {
        private string _ipAddress { get; set; }
        private string _password { get; set; }
        private string _userName { get; set; }

        public void Initialize(string ipAddress, string userName, string password)
        {
            _ipAddress = ipAddress;
            _password = password;
            _userName = userName;
        }

        public void UpdateRetentionTable(DateTime Date, string json, string TableName = "TBL_JsonRetention")
        {
            var connectionBuilder = new MySqlConnectionStringBuilder();
            connectionBuilder.Database = "redash"; // 이거 테이블이 고정이면 상수로 주는게 맞음.
            connectionBuilder.Server = _ipAddress;
            connectionBuilder.UserID = _userName;
            connectionBuilder.Password = _password;


            // 커넥션이 생기지 않는 경우 Null 반환
            using (MySqlConnection conn = new MySqlConnection(connectionBuilder.ConnectionString))
            {
                conn.Open();

                MySqlCommand sqlCommand = new MySqlCommand();
                sqlCommand.Connection = conn;

                // 쿼리시 해당 값이 존재하는지
                var is_exist = IsVaildQuery(
                    $"SELECT * FROM {TableName} WHERE " +
                    $" (`Date` = '{Date.ToString("yyyy-MM-dd")}')"
                    );

                // Append Type
                var sb = new StringBuilder();
                sb.Append("{ \"Datas\" : ");
                sb.Append(json);
                sb.Append(" }");

                // update or insert data
                if (is_exist)
                {
                    sqlCommand.CommandText = $"UPDATE {TableName} SET Details = @Details WHERE (`Date` = @Date)";
                }
                else
                {
                    sqlCommand.CommandText = $"INSERT INTO {TableName} (`Date`, Details) " +
                        $"VALUES (@Date, @Details)" ;
                }
                sqlCommand.Parameters.AddWithValue("@Date", Date.ToString("yyyy-MM-dd"));
                sqlCommand.Parameters.AddWithValue("@Details", sb);
                // JObject.Parse(json)

                sqlCommand.ExecuteNonQuery();

                conn.Close();
            }
        }



        /// <summary>
        /// 쿼리 가능 여부 체크
        /// 값이 하나라도 반환되면 True, 반환되지 않으면 False 반환
        /// </summary>
        public bool IsVaildQuery(string sql)
        {
            var connectionBuilder = new MySqlConnectionStringBuilder();
            connectionBuilder.Database = "redash";
            connectionBuilder.Server = _ipAddress;
            connectionBuilder.UserID = _userName;
            connectionBuilder.Password = _password;        

            var isExist = true;

            using (var conn = new MySqlConnection(connectionBuilder.ConnectionString))
            {
                conn.Open();

                var cmd = new MySqlCommand(sql, conn);
                var rdr = cmd.ExecuteReader();

                isExist = rdr.Read(); // 첫번째 읽기에서 null을 읽으면 없는거.
                rdr.Close();
            }

            return isExist;
        }
    }
}


/*
 Redash 테이블 스키마 정리

 CREATE TABLE CONVERT_TBL_Retention (
 Id		INT NOT NULL AUTO_INCREMENT,
 Country	VARCHAR(20),
 Market	 INT,
 `Day`   DATE,
 Period  INT,
 `Total` Int,
 `Value` INT,
 PRIMARY KEY(Id)
 );
*/