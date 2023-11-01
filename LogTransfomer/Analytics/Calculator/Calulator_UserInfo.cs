using LogTransfomer.Database;
using LogTransfomer.Database.GameLog;
using LogTransfomer.Database.GameLog.Types;

/// 일정 기간 내 접속한 유저 단위로, 로그의 개수, value를 세기 위한 Calculator
/// byte로 들어오는 데이터 파싱 필요.
/// 
namespace LogTransfomer.Analytics.Calculator
{
    public class Calulator_UserInfo
    {
        private readonly GameLogDB _gameLogDB;

        public Calulator_UserInfo(string _ipAddress, string _userName, string _password) // INIT
        {
            _gameLogDB = new GameLogDB();
            _gameLogDB.Initialize(_ipAddress, _userName, _password);
        }

        // 특정 기간의 유저단위 로그를 보기 위해, 특정 기간의 로그 전체를 반환해야.
        public List<GameLogData> GetGameLogData(DateTime targetDate, List<LogType> logTypes = null)
        {
            var logDatas = new List<GameLogData>();

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
                    // 여기서 필요한거만 거르고, logdata를 해석해야한다.
                    // 해석된 데이터를 반환
                    foreach (var data in datas)
                    {
                        var treatdata = data.LogData; // 여기서 처리해서 다시 밀어넣는 식으로 작성해야.
                    }
                    logDatas.AddRange(datas);
                }
            }
            return logDatas;
        }
    }
}
