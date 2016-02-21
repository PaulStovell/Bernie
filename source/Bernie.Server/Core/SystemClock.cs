using System;

namespace Bernie.Server.Core
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        public IDisposable In(TimeSpan timespan, Action callback)
        {
            callback();
            return null;
        }
    }
}