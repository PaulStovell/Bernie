using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Bernie.Server.Core
{
    public class TextMessageService : ITextMessageService
    {
        private readonly NotificationConfiguration config;

        public TextMessageService(NotificationConfiguration config)
        {
            this.config = config;
        }

        public void Send(string message, string to)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(config.TwilioAccountSid + ":" + config.TwilioAuthToken)));

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "To", to },
                { "From", config.From },
                { "Body", message }
            });

            client.PostAsync(
                "https://api.twilio.com/2010-04-01/Accounts/" + config.TwilioAccountSid + "/Messages.json",
                content)
                .Wait();
        }
    }
}