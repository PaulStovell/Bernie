namespace Bernie.Server.Core
{
    public interface ILog
    {
        void Append(LogCategory category, string message);
    }
}