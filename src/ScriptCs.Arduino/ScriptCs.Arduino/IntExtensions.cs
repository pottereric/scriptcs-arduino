using System;

namespace ScriptCs.Arduino
{
    public static class IntExtensions
    {
        public static TimeSpan Milliseconds(this int i)
        {
            return TimeSpan.FromMilliseconds(i);
        }
        public static TimeSpan Seconds(this int i)
        {
            return TimeSpan.FromSeconds(i);
        }
        public static TimeSpan Minutes(this int i)
        {
            return TimeSpan.FromMinutes(i);
        }
    }
}