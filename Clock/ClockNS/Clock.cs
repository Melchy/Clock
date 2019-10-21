using System;
using System.Threading;

namespace ClockNS
{
    public static class Clock
    {
        private static Func<DateTimeOffset> _utcNow = () => DateTimeOffset.UtcNow;

        private static AsyncLocal<Func<DateTimeOffset>?> _override = new AsyncLocal<Func<DateTimeOffset>?>();

        public static DateTimeOffset UtcNow => (_override.Value ?? _utcNow)();

        internal static void Set(Func<DateTimeOffset> func)
        {
            _override.Value = func;
        }

        internal static void Reset()
        {
            _override.Value = null;
        }
    }
}