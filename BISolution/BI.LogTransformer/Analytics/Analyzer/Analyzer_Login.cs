using Microsoft.Extensions.Logging;

namespace BI.LogTransformer.Analytics.Analyzer
{
    using BI.Core.Common.GameLog.Types;
    using BI.Core.Database.Analytics;
    using BI.Core.Database.GameLog;
    using BI.Core.GameLog;
    using BI.Core.Utils;
    using BI.LogTransformer.Analytics.Calculator;
    using Microsoft.Extensions.DependencyInjection;
    public class Analyzer_Login : AnalyzerBase
    {
        private readonly ILogger<Analyzer_Retention> _logger;

        private readonly Calculator_Login _calculatorLogin;

        private readonly List<GameLogType> _logTypes;

        public Analyzer_Login(IServiceProvider serviceProvider, DBGameLog gameLogDB, DBAnalytics analyticsDB) : base(analyticsDB)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Analyzer_Retention>();

            _calculatorLogin = new Calculator_Login(serviceProvider, gameLogDB);

            var logType = new List<GameLogType>() {
                GameLogType.Login
            };

            _logTypes = logType;
        }

        public override void Analyze(string targetDayString)
        {
            var targetDate = DateTime.Parse(targetDayString);

            var queryDatas = _calculatorLogin.GetGameLogData(targetDate.Date, _logTypes);

            if (null == queryDatas)
            {
                return;
            }

            if (queryDatas.Count() == 0)
            {
                return;
            }

            _logger.LogInformation($"[Deserialize Login]");

            var logdatas = queryDatas.Select(x => x.LogData);

            // logic
            // 현재는 데이터 떼와서 Deserialize 하는 역할이 전부.
            foreach (var logdata in logdatas)
            {
                var bytes = SerializeUtil.Deserialize<GAMELOG_Login>(logdata);

                Console.WriteLine($"UID : {bytes.UserId}, is_new : {bytes.IsNew}");
            }
        }
    }
}
