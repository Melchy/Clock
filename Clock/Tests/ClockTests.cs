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
        public void Overriding_time_to_specific_instance_leads_to_time_being_the_same_after_delay()
        {
            var anonymousTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            const int anonymousDelay = 2000;

            using (new OverrideClock(anonymousTime))
            {
                Task.Delay(anonymousDelay).Wait();
                var timeAfterDelay = Clock.UtcNow;

                anonymousTime.IsSameOrEqualTo(timeAfterDelay);
            }
        }

        [Fact]
        public void Time_is_the_same_before_and_after_override()
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