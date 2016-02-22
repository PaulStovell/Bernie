using Bernie.Server.Core;

namespace Bernie.Tests.Core
{
    public class DummyNotificationService : INotificationService
    {
        public bool IsRaisingWarning { get; private set; }
        public bool IsRaisingAlarm { get; private set; }
        public int AlarmsRaised { get; private set; }
        public int WarningsRaised { get; private set; }

        public void RaiseWarning(string sensor)
        {
            IsRaisingWarning = true;
            WarningsRaised++;
        }
        
        public void RaiseAlarm()
        {
            IsRaisingAlarm = true;
            AlarmsRaised++;
        }

        public void Cancel()
        {
            IsRaisingAlarm = false;
            IsRaisingWarning = false;
        }
    }
}