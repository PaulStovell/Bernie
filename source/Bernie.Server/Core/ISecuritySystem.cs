namespace Bernie.Server.Core
{
    public interface ISecuritySystem
    {
        SecuritySystemState State { get; }
        void Arm();
        void Disarm();
        void RaiseAlarm();
        void MotionDetected();
    }
}