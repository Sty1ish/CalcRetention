
using Microsoft.Extensions.Logging;

namespace BI.LogTransformer.Analytics.Analyzer
{
    using BI.Core.Database.Analytics;
    using BI.Core.Database.GameLog;
    using BI.Core.Utils;
    using BI.LogTransformer.Analytics.Calculator;
    using Microsoft.Extensions.DependencyInjection;

    public class Analyzer_Retention : AnalyzerBase
    {
        private readonly ILogger<Analyzer_Retention> _logger;

        private readonly Calculator_Retention _calculatorRetention;

        public Analyzer_Retention(IServiceProvider serviceProvider, DBGameLog gameLogDB, DBAnalytics analyticsDB) : base(analyticsDB)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Analyzer_Retention>();

            _calculatorRetention = new Calculator_Retention(serviceProvider, gameLogDB);
        }

        /// <summary>
        /// Analyze Retention datas for target day.
        /// </summary>
        /// <param name="targetDayString">Day for anayzing</param>
        public override void Analyze(string targetDayString)
        {
            var targetDate = DateTime.Parse(targetDayString);

            // Retention Query Setting
            var pidDatas = _calculatorRetention.GetGameLogData(targetDate.Date)
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
            _analyticsDB.InsertRetentionDayData(targetDate, bytes);

            _logger.LogInformation($"[Analyze Retention] {TimeUtil.DateTimeToString(targetDate)} upload complted");
        }
    }
}
