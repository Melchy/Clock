using System;

namespace ClockNS
{
    public class OverrideClock : IDisposable
    {
        public OverrideClock(DateTimeOffset dateTimeToSet)
        {
            Clock.Set(() => dateTimeToSet);
        }

        public void Dispose()
        {
            Clock.Reset();
        }
    }
}