using System.Linq;

namespace Bernie.Server.Core
{
    public class NotificationService : INotificationService
    {
        private readonly ITextMessageService textMessage;
        private readonly NotificationConfiguration config;

        public NotificationService(ITextMessageService textMessage, NotificationConfiguration config)
        {
            this.textMessage = textMessage;
            this.config = config;
        }

        public void RaiseWarning(string sensor)
        {
            var message = $"Woof woof! Intrusion detected in {sensor}. You have 30 seconds to disarm Bernie before the alarm sounds. Go to https://bernie.stovell.io to disarm.";
            SendMessageToRecipients(message);
        }

        public void RaiseAlarm()
        {
            const string message = "Intrusion detected, alarm was not deactivated in time. Siren has sounded, and alarm is flashing. Please investigate. Woof!";
            SendMessageToRecipients(message);
        }

        private void SendMessageToRecipients(string message)
        {
            foreach (var phoneNumber in config.To)
            {
                textMessage.Send(message, phoneNumber);
            }
        }

        public void Cancel()
        {

        }
    }
}