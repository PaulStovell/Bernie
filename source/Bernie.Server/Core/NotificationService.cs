using System;

namespace Bernie.Server.Core
{
    public class NotificationService : INotificationService
    {
        public void RaiseWarning()
        {
            Console.WriteLine("WARNING");
        }

        public void RaiseAlarm()
        {
            Console.WriteLine("ALARM");
        }

        public void Cancel()
        {

        }
    }
}