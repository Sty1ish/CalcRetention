using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTransfomer.Database.GameLog
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial struct LogData_Login
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsNew { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        [MemoryPackOrder(1)]
        public string UserId { get; set; }
    }
}
