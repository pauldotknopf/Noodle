using System;

namespace Noodle
{
    public static class CommonExtensions
    {
        public static TimeSpan CalculateInterval(this int interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotSupportedException("Unknown time unit: " + unit);
            }
        }

        public static TimeSpan CalculateInterval(this TimeUnit unit, int interval)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotSupportedException("Unknown time unit: " + unit);
            }
        }
    }
}
