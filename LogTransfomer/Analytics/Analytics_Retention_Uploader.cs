using LogTransfomer.Analytics.Data;
using LogTransfomer.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer.Analytics
{
    /// <summary>
    /// 역할
    /// Analytics_Retention : 연산 기준일 ~ 데이터 마지막날까지 리텐션 결과를 반환한다. 
    /// Analytics_Retention_Uploader : 다양한 연산 기준일 (데이터 시작일 ~ 종료일)에서 구한 데이터의 합계를 Json으로 변환, 적재.
    /// </summary>
    public class Analytics_Retention_Uploader
    {
        private readonly AnalyticsDB _AnalyticsDB;

        public Analytics_Retention_Uploader(string _ipAddress, string _userName, string _password)
        {
            _AnalyticsDB = new AnalyticsDB();
            _AnalyticsDB.Initialize(_ipAddress, _userName, _password);
        }

        public void SaveDB_RetentionData(DateTime TargetDate, List<AnalyData_RetentionData> data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            _AnalyticsDB.UpdateRetentionTable(TargetDate, json);
        }
    }
}
