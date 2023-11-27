using BI.Core.Common.GameLog.Types;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Core.Common.Analytics.Data
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GAMELOG_byteData
    {
        [MemoryPackOrder(0)]
        public string CountryCode { get; set; }

        [MemoryPackOrder(1)]
        public MarketType MarketType { get; set; }

        [MemoryPackOrder(2)]
        public string Pid { get; set; }

        [MemoryPackOrder(3)]
        public byte[] LogData { get; set; }
    }
}
