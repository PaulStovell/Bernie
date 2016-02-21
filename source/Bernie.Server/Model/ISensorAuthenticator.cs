namespace Bernie.Server.Model
{
    public interface ISensorAuthenticator
    {
        bool AuthenticateToken(string token);
    }
}
