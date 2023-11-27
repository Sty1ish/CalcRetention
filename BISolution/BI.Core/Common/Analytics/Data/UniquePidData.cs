using BI.Core.Common.GameLog.Types;
using MemoryPack;

namespace BI.Core.Common.Analytics.Data
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class UniquePidData
    {
        [MemoryPackOrder(0)]
        public string CountryCode { get; set;}

        [MemoryPackOrder(1)]
        public MarketType MarketType { get; set;}

        [MemoryPackOrder(2)]
        public string Pid { get; set;}
    }
}
