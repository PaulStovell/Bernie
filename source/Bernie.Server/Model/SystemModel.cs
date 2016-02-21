using System.Collections.Generic;
using Bernie.Server.Core;

namespace Bernie.Server.Model
{
    public class SystemModel
    {
        public SecuritySystemState State { get; set; }
        public List<LogEvent> Events { get; set; }
    }
}
