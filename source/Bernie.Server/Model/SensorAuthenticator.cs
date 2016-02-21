namespace Bernie.Server.Model
{
    public class SensorAuthenticator : ISensorAuthenticator
    {
        private readonly string expectedToken;

        public SensorAuthenticator(string expectedToken)
        {
            this.expectedToken = expectedToken;
        }

        public bool AuthenticateToken(string token)
        {
            return expectedToken == token;
        }
    }
}