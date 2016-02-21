using System;
using System.Collections.Generic;
using System.Linq;
using Bernie.Server.Core;

namespace Bernie.Tests.Core
{
    public class DummyClock : IClock
    {
        private readonly List<PendingCallback> pendingCallbacks = new List<PendingCallback>(); 

        public DateTimeOffset Now { get; set; }

        public void WindForward(TimeSpan time)
        {
            Now = Now.Add(time);

            foreach (var pending in pendingCallbacks.Where(pending => pending.IsDue(Now)))
            {
                pending.Raise();
            }
        }

        public IDisposable In(TimeSpan timespan, Action callback)
        {
            var pending = new PendingCallback(Now.Add(timespan), callback);
            pendingCallbacks.Add(pending);
            return new ActionDisposable(() =>
            {
                pendingCallbacks.Remove(pending);
            });
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