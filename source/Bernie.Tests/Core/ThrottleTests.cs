using System;
using Bernie.Server.Core;
using Xunit;

namespace Bernie.Tests.Core
{
    public class ThrottleTests
    {
        [Fact]
        public void PreventsEventsBeingRaisedTooManyTimes()
        {
            var now = DateTimeOffset.UtcNow;
            var throttle = new Throttle(TimeSpan.FromSeconds(30));

            Assert.True(throttle.ShouldRaise(now));
            Assert.False(throttle.ShouldRaise(now));
            Assert.False(throttle.ShouldRaise(now.Subtract(TimeSpan.FromSeconds(10))));
            Assert.False(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(10))));
            Assert.False(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(20))));
            Assert.False(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(30))));
            Assert.True(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(31))));
            Assert.False(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(31))));
            throttle.Reset();
            Assert.True(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(31))));
            Assert.False(throttle.ShouldRaise(now.Add(TimeSpan.FromSeconds(31))));
        }
    }
}
