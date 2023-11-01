using LogTransfomer.Database.GameLog;
using LogTransfomer.Database.GameLog.Types;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System.Configuration;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;

namespace LogTransfomer.Database
{
    public class GameLogDB
    {
        private string _ipAddress { get; set; }
        private string _password { get; set; }
        private string _userName { get; set; }

        public void Initialize(string ipAddress, string userName, string password )
        {
            _ipAddress = ipAddress;
            _password = password;
            _userName = userName;
        }



        /// <summary>
        /// 기준일에, 리스트로 인풋되는 로그 타입과 일치하는 데이터베이스 전체를 반환한다.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tableName"></param>
        /// <param name="logTypes"></param>
        /// <returns></returns>
        public IEnumerable<GameLogData> GetLogDatas(string databaseName, string tableName, List<LogType> logTypes = null)
        {

            var sb = new StringBuilder();
            sb.Append($"SELECT * FROM {tableName}");
            if (logTypes != null)
            {
                sb.Append(" WHERE logtype IN (");
                foreach(var logType in logTypes )
                {
                    sb.Append($"{(int)logType},");
                }

                sb.Append("0)");
            }

            var datas = new List<GameLogData>();

            var connectionBuilder = new MySqlConnectionStringBuilder();
            connectionBuilder.Database = databaseName;
            connectionBuilder.Server = _ipAddress;
            connectionBuilder.UserID = _userName;
            connectionBuilder.Password = _password;


            // 커넥션이 생기지 않는 경우 Null 반환
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionBuilder.ConnectionString))
                {
                    conn.Open();

                    if (IsExistTable(databaseName, tableName))
                    {
                        MySqlCommand cmd = new MySqlCommand(sb.ToString(), conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            var data = new GameLogData();

                            data.CountryCode = rdr.GetString("country_code");
                            data.MarketType = (MarketType)rdr.GetInt32("market_type");
                            data.IpAddress = rdr.GetString("ip_address");
                            data.Uid = rdr.GetString("uid");
                            data.Pid = rdr.GetString("pid");
                            data.PlayerName = rdr.GetString("playername");
                            data.ServiceType = (ServiceType)rdr.GetInt32("servicetype");
                            data.LogType = (LogType)rdr.GetInt32("logtype");

                            if (rdr.IsDBNull("logdata") == false)
                            {
                                byte[] buffer = new byte[16777215]; // hard cording - 데이터는 16,777,215 바이트까지 수신
                                long bytesToRead = rdr.GetBytes(rdr.GetOrdinal("logdata"), 0, buffer, 0, 16777215); // 읽고 BLOB 데이터의 크기를 찾기
                                data.LogData = new byte[bytesToRead]; 
                                Buffer.BlockCopy(buffer, 0, data.LogData, 0, (int)bytesToRead); // new byte에 버퍼값을 복사합니다.
                            }

                            if (rdr.IsDBNull("playerdata") == false)
                            {
                                data.PlayerData = rdr.GetString("playerdata");
                            }

                            if (rdr.IsDBNull("extdata") == false)
                            {
                                data.ExtData = rdr.GetString("extdata");
                            }

                            data.LogTime = rdr.GetDateTime("logtime");
                            data.RegTime = rdr.GetDateTime("regtime");

                            datas.Add(data);
                        }
                        rdr.Close();
                        conn.Close();
                    }
                }

                return datas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }



        public bool IsExistTable(string databaseName, string table)
        {
            var connectionBuilder = new MySqlConnectionStringBuilder();
            connectionBuilder.Database = databaseName;
            connectionBuilder.Server = _ipAddress;
            connectionBuilder.UserID = _userName;
            connectionBuilder.Password = _password;

            // 이 쿼리로 진행하면, 접속한 DataBase에서 해당 테이블이 있는지 검색한다. 커넥션에 202308로 발생했으니, 202308 아래 테이블만 있는지 체크 되지.
            var sql = $"SELECT Id FROM {table}";
            var isExist = true;

            using (var conn = new MySqlConnection(connectionBuilder.ConnectionString))
            {
                conn.Open();

                var cmd = new MySqlCommand(sql, conn);
                var rdr = cmd.ExecuteReader();

                isExist = rdr.Read();
                rdr.Close();
            }

            return isExist;
        }
    }
}
