using MemoryPack;
using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer.Database.GameLog
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GameLogData
    {

        [MemoryPackOrder(0)]
        public int Id { get; set; }

        [MemoryPackOrder(1)]
        public string CountryCode { get; set; }

        [MemoryPackOrder(2)]
        public MarketType MarketType { get; set; }

        [MemoryPackOrder(3)]
        public LogType LogType { get; set; }

        [MemoryPackOrder(4)]
        public string IpAddress { get; set; }

        [MemoryPackOrder(5)]
        public string Uid { get; set; }

        [MemoryPackOrder(6)]
        public string Pid { get; set; }

        [MemoryPackOrder(7)]
        public string PlayerName { get; set; }

        [MemoryPackOrder(8)]
        public ServiceType ServiceType { get; set; }

        [MemoryPackOrder(9)]
        public byte[] LogData { get; set; }

        [MemoryPackOrder(10)]
        public string ExtData { get; set; } // 실제 형태 모름

        [MemoryPackOrder(11)]
        public DateTime LogTime { get; set; }

        [MemoryPackOrder(12)]
        public DateTime RegTime { get; set; }

        [MemoryPackOrder(13)]
        public string PlayerData { get; set; } // 실제 형태 모름

        public GameLogData()
        {
        }

        [MemoryPackConstructor]
        public GameLogData(int id, string CountryCode, MarketType MarketType, LogType LogType, string IpAddress,
            string Uid, string Pid, string PlayerName, ServiceType ServiceType, byte[] LogData, string ExtData,
            DateTime LogTime, DateTime RegTime, string PlayerData)
        {
            this.Id = id;
            this.CountryCode = CountryCode;
            this.MarketType = MarketType;
            this.LogType = LogType;
            this.IpAddress = IpAddress;
            this.Uid = Uid;
            this.Pid = Pid;
            this.PlayerName = PlayerName;
            this.ServiceType = ServiceType;
            this.LogData = LogData;
            this.ExtData = ExtData;
            this.LogTime = LogTime;
            this.RegTime = RegTime;
            this.PlayerData = PlayerData;
        }
    }
}
