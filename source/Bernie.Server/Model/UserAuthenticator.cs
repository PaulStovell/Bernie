using Microsoft.Extensions.Configuration;

namespace Bernie.Server.Model
{
    public class UserAuthenticator : IUserAuthenticator
    {
        private readonly IConfigurationSection usersSection;

        public UserAuthenticator(IConfigurationSection usersSection)
        {
            this.usersSection = usersSection;
        }

        public bool Authenticate(string username, string password)
        {
            var expectedPassword = usersSection[username];
            if (string.IsNullOrWhiteSpace(expectedPassword))
                return false;

            return password == expectedPassword;
        }
    }
}