using System;

namespace Bernie.Server.Core
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
        IDisposable In(TimeSpan timespan, Action callback);
    }
}