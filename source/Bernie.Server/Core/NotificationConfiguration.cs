namespace Bernie.Server.Core
{
    public class NotificationConfiguration
    {
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
    }
}