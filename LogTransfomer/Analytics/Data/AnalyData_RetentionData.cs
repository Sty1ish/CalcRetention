using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer.Analytics.Data
{
    public class AnalyData_RetentionData
    {
        public string Country { get; set; }

        public int Market { get; set; }

        public DateTime DateTime { get; set; }

        public int DayCount { get; set; }

        public int TotalPidCount { get; set; }

        public int RetentionCount { get; set; }
    }
}
