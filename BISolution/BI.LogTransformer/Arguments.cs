using PowerArgs;

namespace BI.LogTransformer
{
    public class Arguments
    {
        [ArgDescription("configuration file")]
        public string ConfigFile { get; set; }

        [ArgDescription("DayString - 20230101")]
        public string DayString { get; set; }
    }
}
