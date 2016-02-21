namespace Bernie.Server.Model
{
    public interface IUserAuthenticator
    {
        bool Authenticate(string username, string password);
    }
}
