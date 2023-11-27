using PowerArgs;

namespace BI.APIServer
{
    public class Arguments
    {
        [ArgDescription("configuration file")]
        public string ConfigFile { get; set; }

        [ArgDescription("DayString - 20230101")]
        public string DayString { get; set; }
    }
}
