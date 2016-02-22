using System;
using System.Collections.Generic;
using System.Threading;

namespace Bernie.Server.Core
{
    public class SystemClock : IClock
    {
        private readonly List<PendingCallback> pendingCallbacks = new List<PendingCallback>();
        private readonly object sync = new object();
        private readonly Timer timer;

        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        public SystemClock()
        {
            timer = new Timer(Tick, null, 1000, Timeout.Infinite);
        }

        public IDisposable In(TimeSpan timespan, Action callback)
        {
            var pending = new PendingCallback(Now.Add(timespan), callback);

            lock (sync)
            {
                pendingCallbacks.Add(pending);
            }
            return new ActionDisposable(() =>
            {
                lock (sync)
                {
                    pendingCallbacks.Remove(pending);
                }
            });
        }

        private void Tick(object state)
        {
            var now = Now;
            var ready = new List<PendingCallback>();
            lock (sync)
            {
                for (var i = 0; i < pendingCallbacks.Count; i++)
                {
                    var pending = pendingCallbacks[i];
                    if (!pending.IsDue(now))
                        continue;

                    pendingCallbacks.RemoveAt(i);
                    ready.Add(pending);
                    i--;
                }
            }

            foreach (var pending in ready)
            {
                pending.Raise();
            }

            timer.Change(1000, Timeout.Infinite);
        }

        private class ActionDisposable : IDisposable
        {
            private readonly Action callback;

            public ActionDisposable(Action callback)
            {
                this.callback = callback;
            }

            public void Dispose()
            {
                callback();
            }
        }

        private class PendingCallback
        {
            private readonly DateTimeOffset due;
            private readonly Action callback;

            public PendingCallback(DateTimeOffset due, Action callback)
            {
                this.due = due;
                this.callback = callback;
            }

            public bool IsDue(DateTimeOffset now)
            {
                return now > due;
            }

            public void Raise()
            {
                callback();
            }
        }
    }
}