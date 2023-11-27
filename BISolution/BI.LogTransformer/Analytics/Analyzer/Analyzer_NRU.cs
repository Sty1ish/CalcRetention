using BI.Core.Common.Analytics.Types;
using BI.Core.Common.GameLog.Types;
using BI.Core.Database.Analytics;
using BI.Core.Database.GameLog;
using BI.Core.Utils;
using BI.LogTransformer.Analytics.Calculator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BI.LogTransformer.Analytics.Analyzer
{
    public class Analyzer_NRU : AnalyzerBase
    {
        private readonly ILogger<Analyzer_NRU> _logger;

        private readonly Calculator_NRU _calculator_Event;

        private readonly List<GameLogType> _logTypes;

        private readonly AnalyticsType _eventType;

        public Analyzer_NRU(IServiceProvider serviceProvider, DBGameLog gameLogDB, DBAnalytics analyticsDB) : base(analyticsDB)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Analyzer_NRU>();

            _calculator_Event = new Calculator_NRU(serviceProvider, gameLogDB);

            _logTypes = new List<GameLogType>() {
                GameLogType.CreatePlayer
            };

            _eventType = AnalyticsType.NewResisterPids;
        }

        /// <summary>
        /// Analyze Retention datas for target day.
        /// </summary>
        /// <param name="targetDayString">Day for anayzing</param>
        public override void Analyze(string targetDayString)
        {
            var targetDate = DateTime.Parse(targetDayString);

            // Retention Query Setting
            var pidDatas = _calculator_Event.GetGameLogData(targetDate.Date, _logTypes)
                .DistinctBy(d => d.Pid);

            if (null == pidDatas)
            {
                return;
            }

            if (pidDatas.Count() == 0)
            {
                return;
            }

            // serialize
            var bytes = SerializeUtil.Serialize(pidDatas);

            // db insert
            _analyticsDB.InsertNRUDayData(targetDate, bytes, _eventType);

            _logger.LogInformation($"[Analyze Event] : Event : {String.Join(Environment.NewLine, _logTypes)},  {TimeUtil.DateTimeToString(targetDate)} upload complted");
        }
    }
}
