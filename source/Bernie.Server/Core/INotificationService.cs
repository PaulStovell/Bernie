namespace Bernie.Server.Core
{
    public interface INotificationService
    {
        void RaiseWarning();
        void RaiseAlarm();
        void Cancel();
    }
}