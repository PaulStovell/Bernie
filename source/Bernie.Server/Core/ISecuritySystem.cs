namespace Bernie.Server.Core
{
    public interface ISecuritySystem
    {
        SecuritySystemState State { get; }
        void Arm(string who);
        void Disarm(string who);
        void MotionDetected(string sensor);
    }
}