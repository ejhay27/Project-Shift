using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for TIME-001.</summary>
    public sealed class GameClockTests
    {
        [Test]
        public void Phases_FollowSpecBoundaries()
        {
            var clock = new GameClock(60.0); // 1 game-hour per real second
            clock.Advance(6.0);              // 06:00
            Assert.AreEqual(DayPhase.Morning, clock.Phase);
            clock.Advance(4.0);              // 10:00
            Assert.AreEqual(DayPhase.Daytime, clock.Phase);
            clock.Advance(7.0);              // 17:00
            Assert.AreEqual(DayPhase.Afternoon, clock.Phase);
            clock.Advance(2.0);              // 19:00
            Assert.AreEqual(DayPhase.Sunset, clock.Phase);
            clock.Advance(4.0);              // 23:00
            Assert.AreEqual(DayPhase.Night, clock.Phase);
        }

        [Test]
        public void DayCount_Increments_AtMidnight()
        {
            var clock = new GameClock(1440.0); // 1 game-day per real second
            Assert.AreEqual(1, clock.DayCount);
            clock.Advance(1.0);
            Assert.AreEqual(2, clock.DayCount);
        }

        [Test]
        public void SkipMinutes_Advances_ForSleep()
        {
            var clock = new GameClock(1.0);
            clock.SkipMinutes(480.0);
            Assert.AreEqual(480.0, clock.TotalGameMinutes, 0.001);
        }
    }
}
