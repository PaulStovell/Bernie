using System;

namespace Bernie.Server.Core
{
    public class Throttle
    {
        private DateTimeOffset? lastRaised = new DateTimeOffset();
        private readonly TimeSpan timeToWait;

        public Throttle(TimeSpan timeToWait)
        {
            this.timeToWait = timeToWait;
        }

        public bool ShouldRaise(DateTimeOffset now)
        {
            if (lastRaised == null || now.Subtract(lastRaised.Value) > timeToWait)
            {
                lastRaised = now;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            lastRaised = null;
        }
    }
}