namespace Bernie.Server.Core
{
    public interface ITextMessageService
    {
        void Send(string message, string to);
    }
}