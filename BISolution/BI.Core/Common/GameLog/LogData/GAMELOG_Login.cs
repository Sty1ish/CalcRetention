
using MemoryPack;

namespace BI.Core.GameLog
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GAMELOG_Login // 클래스도, 구조체도 안됨..
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
