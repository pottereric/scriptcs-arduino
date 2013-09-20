using System;

namespace ScriptCs.Arduino
{
    public static class IntExtensions
    {
        public static TimeSpan Milliseconds(this int i)
        {
            return TimeSpan.FromMilliseconds(i);
        }

        public static TimeSpan Millisecond(this int i)
        {
            return i.Milliseconds();
        }

        public static TimeSpan Seconds(this int i)
        {
            return TimeSpan.FromSeconds(i);
        }

        public static TimeSpan Second(this int i)
        {
            return i.Seconds();
        }

        public static TimeSpan Minutes(this int i)
        {
            return TimeSpan.FromMinutes(i);
        }

        public static TimeSpan Minute(this int i)
        {
            return i.Minutes();
        }
    }
}