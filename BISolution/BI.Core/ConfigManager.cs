using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Xml.Linq;

namespace BI.Core
{ 
    public class ConfigManager
    {
        private readonly ILogger<ConfigManager> _logger;

        /// config 
        /// database info
        public string Addr_LogDB { get; set; }
        public string User_LogDB { get; set; }
        public string Pass_LogDB { get; set; }

        // analytics db
        public string Addr_AnalyticsDB { get; set; }
        public string User_AnalyticsDB { get; set; }
        public string Pass_AnalyticsDB { get; set; }

        public ConfigManager(IConfiguration configuration, ILogger<ConfigManager> logger)
        {
            _logger = logger;
            var config = configuration.GetSection("Config");

            Addr_LogDB = config["Database_Log:Addr"];
            User_LogDB = config["Database_Log:User"];
            Pass_LogDB = config["Database_Log:Pass"];

            Addr_AnalyticsDB = config["Database_Analytics:Addr"];
            User_AnalyticsDB = config["Database_Analytics:User"];
            Pass_AnalyticsDB = config["Database_Analytics:Pass"];
        }

        public void Initialize()
        {
            _logger.LogInformation("ConfigManager initialized.");
        }

        public string ConnectionString_GameLogDB()
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.UserID = User_LogDB;
            builder.Server = Addr_LogDB;
            builder.Password = Pass_LogDB;
            return builder.ConnectionString;
        }

        public string ConnectionString_AnalyticsDB()
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.UserID = User_AnalyticsDB;
            builder.Server = Addr_AnalyticsDB;
            builder.Password = Pass_AnalyticsDB;
            return builder.ConnectionString;
        }

    }
}