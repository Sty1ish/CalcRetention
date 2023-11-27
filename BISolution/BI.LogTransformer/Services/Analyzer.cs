using Microsoft.Extensions.Logging;

namespace BI.LogTransformer.Services
{
    using BI.Core;
    using BI.Core.Common.Analytics.Types;
    using BI.Core.Common.GameLog.Types;
    using BI.Core.Database.Analytics;
    using BI.Core.Database.GameLog;
    using BI.LogTransformer.Analytics.Analyzer;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class Analyzer
    {
        private readonly ILogger<Analyzer> _logger;
        private readonly IServiceProvider _serviceProvider;

        // Configuration for Database.
        private readonly ConfigManager _configManager;

        // Database
        private readonly DBGameLog _gameLogDB;
        private readonly DBAnalytics _analyticsDB;

        // connection strings
        private readonly string _connectionString_GameLogDB;
        private readonly string _connectionString_AnalyticsDB;

        // Analyze logics..........
        private Analyzer_Retention _analyzerRetention { get; set; }
        private Analyzer_Login _analyzerLogin { get; set; }
        private Analyzer_NRU _analyzer_NRU { get; set; }

        private readonly string _dayString;

        /// <summary>
        /// constructor
        /// </summary>
        public Analyzer(IServiceProvider serviceProvider, string dayString)
        {
            _serviceProvider = serviceProvider;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            _logger = loggerFactory.CreateLogger<Analyzer>();
            _configManager = serviceProvider.GetService<ConfigManager>();

            _dayString = dayString;

            _connectionString_GameLogDB = _configManager.ConnectionString_GameLogDB();
            _connectionString_AnalyticsDB = _configManager.ConnectionString_AnalyticsDB();

            // database
            _gameLogDB = new DBGameLog(serviceProvider, _connectionString_GameLogDB);
            _analyticsDB = new DBAnalytics(serviceProvider, _connectionString_AnalyticsDB);
        }

        public void Initialized_Analyze()
        {
            // analyzer create
            _analyzerRetention = new Analyzer_Retention(_serviceProvider, _gameLogDB, _analyticsDB);
            _analyzer_NRU = new Analyzer_NRU(_serviceProvider, _gameLogDB, _analyticsDB);

            // 복호화 예제
            _analyzerLogin = new Analyzer_Login(_serviceProvider, _gameLogDB, _analyticsDB);

        }

        public void AnalyzeAll()
        {
            _logger.LogInformation(":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            _logger.LogInformation("BI Analyze Start:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            _logger.LogInformation(":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            _logger.LogInformation("Configuration Info:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");

            _logger.LogInformation($"Target Day. Day: {_dayString}");
            _logger.LogInformation($"Log Database. Address: {_configManager.Addr_LogDB}, User: {_configManager.User_LogDB}, Pass: {_configManager.Pass_LogDB}");
            _logger.LogInformation($"Analytics Database. Address: {_configManager.Addr_AnalyticsDB}, User: {_configManager.User_AnalyticsDB}, Pass: {_configManager.Pass_AnalyticsDB}");

            // Update TBL_Analytics
            _analyzerRetention.Analyze(_dayString);
            _analyzer_NRU.Analyze(_dayString);

            
            // 이후 이렇게 늘어남 (파싱 - 출력 코드로 가능성 확인)
            _analyzerLogin.Analyze(_dayString);
        }
    }
}
