using System;
using System.Threading.Tasks;
using Bernie.Server.Core;
using Xunit;

namespace Bernie.Tests.Core
{
    public class SecuritySystemTests
    {
        private readonly DummyLog log;
        private readonly DummyClock clock;
        private readonly SecuritySystem system;
        private readonly DummyNotificationService notifications;

        public SecuritySystemTests()
        {
            log = new DummyLog();
            clock = new DummyClock();
            notifications = new DummyNotificationService();
            system = new SecuritySystem(clock, log, notifications);
        }

        [Fact]
        public void StartsAsDisarmed()
        {
            Assert.Equal(SecuritySystemState.Disarmed, system.State);
            log.AssertLogged("System started after power outage");
        }
        
        [Fact]
        public void CanBeArmed()
        {
            system.Arm("Paul");
            log.AssertLogged("was armed");
        }

        [Fact]
        public void WhenArmedMotionTriggersIntruderWarningImmediately()
        {
            system.Arm("Paul");
            system.MotionDetected("Study");

            Assert.Equal(1, notifications.WarningsRaised);
            Assert.Equal(0, notifications.AlarmsRaised);
        }

        [Fact]
        public void WhenDisarmedMotionIsIgnored()
        {
            system.Disarm("Paul");
            system.MotionDetected("Study");

            Assert.Equal(0, notifications.WarningsRaised);
            Assert.Equal(0, notifications.AlarmsRaised);
        }

        [Fact]
        public void WhenArmedAlarmIsRaisedAfter30Seconds()
        {
            system.Arm("Paul");
            system.MotionDetected("Study");

            Assert.Equal(1, notifications.WarningsRaised);
            Assert.Equal(0, notifications.AlarmsRaised);

            clock.WindForward(TimeSpan.FromSeconds(30));
            Assert.Equal(0, notifications.AlarmsRaised);

            clock.WindForward(TimeSpan.FromSeconds(31));
            Assert.Equal(1, notifications.AlarmsRaised);
        }

        [Fact]
        public void DisarmingCancelsAnyAlarms()
        {
            system.Arm("Paul");
            system.MotionDetected("Study");
            clock.WindForward(TimeSpan.FromSeconds(31));

            Assert.True(notifications.IsRaisingAlarm);

            system.Disarm("Paul");

            Assert.False(notifications.IsRaisingAlarm);
        }
        
        [Fact]
        public void DisarmingCancelsAnyAlarmsBeforeTheyCanBeRaised()
        {
            system.Arm("Paul");
            system.MotionDetected("Study");

            clock.WindForward(TimeSpan.FromSeconds(10));

            Assert.Equal(1, notifications.WarningsRaised);
            Assert.Equal(0, notifications.AlarmsRaised);

            system.Disarm("Paul");
            
            clock.WindForward(TimeSpan.FromSeconds(31));

            Assert.Equal(0, notifications.AlarmsRaised);
        }

        [Fact]
        public void MultipleMotionDetectionsWillNotRaiseAdditionalWarningsUntil()
        {
            system.Arm("Paul");
            system.MotionDetected("Study");
            system.MotionDetected("Study");
            system.MotionDetected("Study");
            system.MotionDetected("Study");

            Assert.Equal(1, notifications.WarningsRaised);
            
            clock.WindForward(TimeSpan.FromSeconds(10));
            system.MotionDetected("Study");
            system.MotionDetected("Study");

            Assert.Equal(1, notifications.WarningsRaised);

            clock.WindForward(TimeSpan.FromMinutes(2));
            system.MotionDetected("Study");
            system.MotionDetected("Study");
            system.MotionDetected("Study");
            system.MotionDetected("Study");

            Assert.Equal(2, notifications.WarningsRaised);

            clock.WindForward(TimeSpan.FromMinutes(3));
            system.MotionDetected("Study");
            system.MotionDetected("Study");
            system.MotionDetected("Study");
            system.MotionDetected("Study");

            Assert.Equal(3, notifications.WarningsRaised);
        }
    }
}
