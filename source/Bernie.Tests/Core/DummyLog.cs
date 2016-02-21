using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bernie.Server.Core;
using Xunit;

namespace Bernie.Tests.Core
{
    public class DummyLog : ILog
    {
        readonly List<string> logs = new List<string>();
        
        public void Append(LogCategory category, string message)
        {
            logs.Add(message);
        }

        public void AssertLogged(string pattern)
        {
            if (logs.Any(line => Regex.IsMatch(line, pattern)))
            {
                return;
            }

            Assert.True(false, "Expected a log entry to match the pattern: " + pattern + Environment.NewLine + "Instead got: " + Environment.NewLine + string.Join(Environment.NewLine, logs));
        }
    }
}