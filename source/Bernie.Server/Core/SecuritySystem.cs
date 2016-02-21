using System;

namespace Bernie.Server.Core
{
    public interface ISecuritySystem
    {
        SecuritySystemState State { get; }
        void Arm();
        void Disarm();
        void RaiseAlarm();
        void MotionDetected();
    }

    public class SecuritySystem : ISecuritySystem
    {
        private readonly IClock clock;
        private readonly ILog log;
        private readonly INotificationService notificationService;
        private SecuritySystemState state;
        private IDisposable countdown;
        private readonly Throttle warningThrottle = new Throttle(TimeSpan.FromMinutes(5));
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

        public void Arm()
        {
            State = SecuritySystemState.Armed;
            log.Append(LogCategory.Armed, "System was armed");
        }

        public void Disarm()
        {
            State = SecuritySystemState.Disarmed;
            log.Append(LogCategory.Disarmed, "System was disarmed");
        }

        public void RaiseAlarm()
        {
            if (alarmThrottle.ShouldRaise(clock.Now))
            {
                notificationService.RaiseAlarm();
                log.Append(LogCategory.Alarm, "Intruder detected, alarm not deactivated in time. Alarm raised.");
            }
        }

        public void MotionDetected()
        {
            if (!IsArmed)
                return;

            SendWarning();

            countdown = clock.In(TimeSpan.FromSeconds(30), () =>
            {
                if (!IsArmed)
                    return;
                RaiseAlarm();
            });
        }

        private bool IsArmed => state == SecuritySystemState.Armed;

        private void SendWarning()
        {
            if (warningThrottle.ShouldRaise(clock.Now))
            {
                notificationService.RaiseWarning();
                log.Append(LogCategory.IntruderWarning, "Intruder detected. Sending a warning message. Alarm will sound in 30 seconds.");
            }
        }
    }
}