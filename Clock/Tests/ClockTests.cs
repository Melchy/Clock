using System;
using System.Threading.Tasks;
using ClockTesting;
using FluentAssertions.Common;
using Xunit;

namespace Tests
{
    public class ClockTests
    {
        [Fact]
        public void OverridingTimeToSpecificInstanceLeadsToTimeBeingTheSameAfterDelay()
        {
            var anonymousTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            const int anonymousDelay = 2000;

            using var _ = new OverrideClock(anonymousTime);
            
            Task.Delay(anonymousDelay).Wait();
            var timeAfterDelay = Clock.UtcNow;

            anonymousTime.IsSameOrEqualTo(timeAfterDelay);
        }

        [Fact]
        public void TimeIsTheSameBeforeAndAfterOverride()
        {
            DateTime.UtcNow.Date.IsSameOrEqualTo(Clock.UtcNow.Date);
            
            var anonymousTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            const int anonymousDelay = 2000;

            using (new OverrideClock(anonymousTime))
                Task.Delay(anonymousDelay).Wait();
            
            DateTime.UtcNow.Date.IsSameOrEqualTo(Clock.UtcNow.Date);
        }
    }
}