using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace BI.Core.Utils
{
    public static class TimeUtil
    {
        /// <summary>
        /// Convert DateTime to String.
        /// </summary>
        public static string DateTimeToString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Compare two days is same.
        /// </summary>
        public static bool IsSameDay(DateTime left, DateTime right)
        {
            if (left.Year != right.Year) 
            {
                return false;
            }

            if(left.Month != right.Month)
            {
                return false;
            }

            if (left.Day != right.Day)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check destination DateTime is past tahn source DateTime.
        /// </summary>
        public static bool IsPastTime(DateTime src, DateTime dst)
        {
            if (DateTime.MinValue == src)
            {
                return false;
            }

            if (DateTime.Compare(src, dst) > 0)
            {
                return true;
            }
            return false;
        }
    }
}
