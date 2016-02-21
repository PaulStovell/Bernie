using System;

namespace Bernie.Server.Core
{
    public class SecuritySystem : ISecuritySystem
    {
        private readonly IClock clock;
        private readonly ILog log;
        private readonly INotificationService notificationService;
        private SecuritySystemState state;
        private IDisposable countdown;
        private readonly Throttle warningThrottle = new Throttle(TimeSpan.FromMinutes(1));
        private readonly Throttle alarmThrottle = new Throttle(TimeSpan.FromHours(1));

        public SecuritySystem(IClock clock, ILog log, INotificationService notificationService)
        {
            this.clock = clock;
            this.log = log;
            this.notificationService = notificationService;
            state = SecuritySystemState.Disarmed;
            log.Append(LogCategory.Disarmed, "System started after power outage");
        }
        
        public SecuritySystemState State
        {
            get { return state; }
            private set
            {
                var old = state;
                if (old == value) return;
                state = value;
                countdown?.Dispose();
                warningThrottle.Reset();
                alarmThrottle.Reset();
                notificationService.Cancel();
            }
        }

        public void Arm(string who)
        {
            State = SecuritySystemState.Armed;
            log.Append(LogCategory.Armed, $"System was armed by {who}");
        }

        public void Disarm(string who)
        {
            State = SecuritySystemState.Disarmed;
            log.Append(LogCategory.Disarmed, $"System was disarmed by {who}");
        }

        public void MotionDetected(string sensor)
        {
            if (!IsArmed)
                return;

            SendWarning(sensor);

            countdown = clock.In(TimeSpan.FromSeconds(30), () =>
            {
                if (!IsArmed)
                    return;
                RaiseAlarm();
            });
        }

        private bool IsArmed => state == SecuritySystemState.Armed;

        private void SendWarning(string sensorName)
        {
            if (!warningThrottle.ShouldRaise(clock.Now))
                return;

            notificationService.RaiseWarning();
            log.Append(LogCategory.IntruderWarning, $"Intruder detected in {sensorName}. Sending a warning message. Alarm will sound in 30 seconds.");
        }

        private void RaiseAlarm()
        {
            if (!alarmThrottle.ShouldRaise(clock.Now))
                return;

            notificationService.RaiseAlarm();
            log.Append(LogCategory.Alarm, "Alarm not deactivated in time after intruder was detected. Alarm flashing, siren sounded.");
        }
    }
}