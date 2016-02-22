namespace Bernie.Server.Core
{
    public interface INotificationService
    {
        void RaiseWarning(string sensor);
        void RaiseAlarm();
        void Cancel();
    }
}