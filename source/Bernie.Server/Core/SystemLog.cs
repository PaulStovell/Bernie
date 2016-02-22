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
            KeepLimitedNumberOfEvents();
            KeepEventsOnlyFromLastFourDays();

            recentEvents.Enqueue(new LogEvent(DateTimeOffset.UtcNow, category, message));
        }

        private void KeepEventsOnlyFromLastFourDays()
        {
            while (recentEvents.Count > 1)
            {
                var peek = recentEvents.Peek();
                if (DateTimeOffset.UtcNow.Subtract(peek.When).TotalDays > 4)
                {
                    recentEvents.Dequeue();
                }
                else break;
            }
        }

        private void KeepLimitedNumberOfEvents()
        {
            while (recentEvents.Count > 200)
            {
                recentEvents.Dequeue();
            }
        }

        public List<LogEvent> GetRecentEvents()
        {
            return recentEvents.ToList();
        }

        public void Clear()
        {
            recentEvents.Clear();
        }
    }
}