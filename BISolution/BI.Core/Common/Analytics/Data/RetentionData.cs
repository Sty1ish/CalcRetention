namespace BI.Core.Common.Analytics.Data
{
    // 반환을 원하는 타입 : List<RetentionData>의 json 형태를 Redash로 보내야 함
    public class RetentionData
    {
        public DateTime Date { get; set; }

        public int DayCount { get; set; }

        public int TotalPidCount { get; set; }

        public int RetentionCount { get; set; }
    }
}
