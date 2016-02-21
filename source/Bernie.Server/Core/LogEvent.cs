using System;

namespace Bernie.Server.Core
{
    public class LogEvent
    {
        public LogEvent(DateTimeOffset when, LogCategory category, string message)
        {
            When = when;
            Category = category;
            Message = message;
        }

        public DateTimeOffset When { get; }
        public LogCategory Category { get; set; }
        public string Message { get; }
    }
}