using System.Collections.Generic;

namespace Bernie.Server.Core
{
    public interface IRecentEventLog
    {
        List<LogEvent> GetRecentEvents();
        void Clear();
    }
}