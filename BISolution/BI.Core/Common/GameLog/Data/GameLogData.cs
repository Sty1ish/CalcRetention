using BI.Core.Common.GameLog.Types;
using Dapper.ColumnMapper;
using MemoryPack;

namespace BI.Core.Common.GameLog.Data
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GameLogData
    {
        [ColumnMapping("id")]
        [MemoryPackOrder(0)]
        public int Id { get; set; }

        [ColumnMapping("country_code")]
        [MemoryPackOrder(1)]
        public string Country_Code { get; set; }

        [ColumnMapping("market_type")]
        [MemoryPackOrder(2)]
        public MarketType Market_Type { get; set; }

        [ColumnMapping("ip_address")]
        [MemoryPackOrder(3)]
        public string Ip_Address { get; set; }

        [ColumnMapping("uid")]
        [MemoryPackOrder(4)]
        public string Uid { get; set; }

        [ColumnMapping("pid")]
        [MemoryPackOrder(5)]
        public string Pid { get; set; }

        [ColumnMapping("playername")]
        [MemoryPackOrder(6)]
        public string PlayerName { get; set; }

        [ColumnMapping("servicetype")]
        [MemoryPackOrder(7)]
        public ServiceType ServiceType { get; set; }

        [ColumnMapping("logtype")]
        [MemoryPackOrder(8)]
        public GameLogType LogType { get; set; }

        [ColumnMapping("logdata")]
        [MemoryPackOrder(9)]
        public byte[] LogData { get; set; }

        [ColumnMapping("extdata")]
        [MemoryPackOrder(10)]
        public string ExtData { get; set; } // 실제 형태 모름

        [ColumnMapping("logtime")]
        [MemoryPackOrder(11)]
        public DateTime LogTime { get; set; }

        [ColumnMapping("regtime")]
        [MemoryPackOrder(12)]
        public DateTime RegTime { get; set; }

        [ColumnMapping("playerdata")]
        [MemoryPackOrder(13)]
        public string PlayerData { get; set; } // 실제 형태 모름

        //[MemoryPackConstructor]
        //public GameLogData(int id, string CountryCode, MarketType MarketType, LogType LogType, string IpAddress,
        //    string Uid, string Pid, string PlayerName, ServiceType ServiceType, byte[] LogData, string ExtData,
        //    DateTime LogTime, DateTime RegTime, string PlayerData)
        //{
        //    this.Id = id;
        //    this.CountryCode = CountryCode;
        //    this.MarketType = MarketType;
        //    this.LogType = LogType;
        //    this.IpAddress = IpAddress;
        //    this.Uid = Uid;
        //    this.Pid = Pid;
        //    this.PlayerName = PlayerName;
        //    this.ServiceType = ServiceType;
        //    this.LogData = LogData;
        //    this.ExtData = ExtData;
        //    this.LogTime = LogTime;
        //    this.RegTime = RegTime;
        //    this.PlayerData = PlayerData;
        //}
    }
}
