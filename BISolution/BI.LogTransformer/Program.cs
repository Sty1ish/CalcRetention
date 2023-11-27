using BI.Core;
using BI.Core.Database.Analytics;
using BI.Core.Database.GameLog;
using BI.LogTransformer.Analytics.Analyzer;
using BI.LogTransformer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerArgs;

namespace BI.LogTransformer
{
    internal class Program
    {
        static void Main(string[] args)
        {
             var arguments = new Arguments();

            try
            {
                arguments = Args.Parse<Arguments>(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Arguments Error.");
                Console.WriteLine(ex.Message);

                return;
            }

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg =>
                {
                    cfg.AddJsonFile(arguments.ConfigFile);
                })
                .ConfigureServices(services =>{
                    services.AddSingleton<ConfigManager>();

                    services.AddTransient<DBAnalytics>();
                    services.AddTransient<DBGameLog>();
                })
            .Build();

            Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("LogTransformer Program Start::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");

            var serviceProvider = host.Services;

            // config manager initialize
            var configManager = host.Services.GetService<ConfigManager>();
            configManager.Initialize();

            var targetDayString = arguments.DayString;

            // update AnalyticsDB -> target Date
            var analyzer = new Analyzer(serviceProvider, targetDayString);
            analyzer.Initialized_Analyze();

            // Run all Analyzer
            analyzer.AnalyzeAll();
           
        }
    }
}

