using MemoryPack;
using Dapper.ColumnMapper;


namespace BI.Core.Common.Analytics.Data
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class AnalyData_RetentionData
    {
        [ColumnMapping("Id")]
        [MemoryPackOrder(0)]
        public int Id { get; set; }

        [ColumnMapping("Date")]
        [MemoryPackOrder(1)]
        public DateTime Date { get; set; }

        [ColumnMapping("Details")]
        [MemoryPackOrder(2)]
        public byte[] Details { get; set; }

        [ColumnMapping("logtype")]
        [MemoryPackOrder(3)]
        public int logtype { get; set; }

        [ColumnMapping("regtime")]
        [MemoryPackOrder(4)]
        public DateTime regtime { get; set; }

    }
}