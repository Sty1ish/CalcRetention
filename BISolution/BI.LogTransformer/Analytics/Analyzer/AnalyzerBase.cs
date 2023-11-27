using BI.Core.Common.GameLog.Types;
using BI.Core.Database.Analytics;

namespace BI.LogTransformer.Analytics.Analyzer
{
    /// <summary>
    /// AnalyzerBase
    /// </summary>
    public abstract class AnalyzerBase
    {
        protected readonly DBAnalytics _analyticsDB;

        public AnalyzerBase(DBAnalytics analyticsDB)
        {
            _analyticsDB = analyticsDB;
        }

        public abstract void Analyze(string dayString);
    }
}
