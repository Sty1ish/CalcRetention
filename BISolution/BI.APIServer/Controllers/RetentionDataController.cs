using BI.APIServer.DataLoader;
using BI.Core;
using BI.Core.Database.Analytics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BI.API_Server.Controllers
{

    /// <summary>
    /// 리텐션 쿼리문 출력 제어, API 컨트롤러 
    /// </summary>
    public class RetentionDataController : ControllerBase
    {
        private ILogger<RetentionDataController> _logger;

        private readonly dataLoaderRetention _retentionProvider;

        public RetentionDataController(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<RetentionDataController>();

            var configManager = serviceProvider.GetService<ConfigManager>();
            var analyticsDB = new DBAnalytics(serviceProvider, configManager.ConnectionString_AnalyticsDB());
            _retentionProvider = new dataLoaderRetention(serviceProvider, analyticsDB);
        }

        // GET: api/RetentionDatas.
        [HttpGet]
        [Route("retention")]
        public string GetRetentionData(string dateTime, string country, string market, int analyticsDate)
        {
            _logger.LogInformation($"[Retention Controller Filter] Date: {dateTime}, analyticsDate : {analyticsDate}, Country: {country}, Market: {market}.");

            var targetDate = Convert.ToDateTime(dateTime);

            // filter condition
            var countries = country?.Split(',').ToList(); // 변수를 "변수, 변수, 변수" 로 줄 수 있음 (리대시 요청 양식)
            var markets = market?.Split(',').ToList(); // 변수를 "변수, 변수, 변수" 로 줄 수 있음 (리대시 요청 양식)

            // get retention
            var retentions = _retentionProvider.GetRentionDatasRange(targetDate, analyticsDate, countries, markets);

            _logger.LogInformation($"[Retention Controller Filter] Retention Data Length : {retentions.Count}.");

            // Output Convert
            var redashDatas = JsonConvert.SerializeObject(retentions);

            return redashDatas;
        }


        // 롤링은 Retention 매니저 쓰는건 맞는데, PID 구하고 연산하는 과정이 전혀 달라질것 같아서 나눠야 할듯.
    }
}
