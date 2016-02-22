namespace Bernie.Server.Model
{
    public class UserAuthenticator : IUserAuthenticator
    {
        private readonly UserAuthenticationOptions usersSection;

        public UserAuthenticator(UserAuthenticationOptions usersSection)
        {
            this.usersSection = usersSection;
        }

        public bool Authenticate(string username, string password)
        {
            string expectedPassword;
            if (!usersSection.Users.TryGetValue(username, out expectedPassword))
                return false;

            return password == expectedPassword;
        }
    }
}