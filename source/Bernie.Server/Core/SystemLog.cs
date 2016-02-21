using System;
using System.Collections.Generic;
using System.Linq;

namespace Bernie.Server.Core
{
    public class SystemLog : ILog, IRecentEventLog
    {
        private readonly Queue<LogEvent> recentEvents = new Queue<LogEvent>();

        public void Append(LogCategory category, string message)
        {
            recentEvents.Enqueue(new LogEvent(DateTimeOffset.UtcNow, category, message));
        }

        public List<LogEvent> GetRecentEvents()
        {
            return recentEvents.ToList();
        }
    }
}