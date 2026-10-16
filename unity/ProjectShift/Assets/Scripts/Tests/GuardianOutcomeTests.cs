using System;
using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for TREEG-007 + GUARD-003 incl. §36 acceptance.</summary>
    public sealed class GuardianOutcomeTests
    {
        private const long T0 = 639000000000000000L;

        [Test]
        public void WorldTree_SlayThenBond_IsRejected()
        {
            var g = new WorldTreeGuardianOutcome();
            Assert.IsTrue(g.TryRecordBossSlain(T0).Ok);
            Assert.IsFalse(g.TryRecordPetBonded(T0 + 1).Ok);
            Assert.AreEqual(GuardianOutcome.BossSlain, g.Outcome);
        }

        [Test]
        public void WorldTree_BondThenSlay_IsRejected()
        {
            var g = new WorldTreeGuardianOutcome();
            Assert.IsTrue(g.TryRecordPetBonded(T0).Ok);
            Assert.IsFalse(g.TryRecordBossSlain(T0 + 1).Ok);
            Assert.AreEqual(GuardianOutcome.PetBonded, g.Outcome);
        }

        [Test]
        public void WorldTree_NeverRespawns_EitherOutcome()
        {
            Assert.IsFalse(new WorldTreeGuardianOutcome().GuardianRespawns);
        }

        [Test]
        public void WorldTree_Outcome_SurvivesReload()
        {
            var g = new WorldTreeGuardianOutcome();
            g.TryRecordBossSlain(T0);
            var reloaded = new WorldTreeGuardianOutcome();
            reloaded.LoadData(g.ToData());
            Assert.AreEqual(GuardianOutcome.BossSlain, reloaded.Outcome);
            Assert.IsFalse(reloaded.TryRecordPetBonded(T0 + 100).Ok);
        }

        [Test]
        public void CityGuardian_Resurrects_AfterTenMinutes()
        {
            var g = new SanctuaryGuardianState();
            Assert.IsTrue(g.RecordDefeat(T0).Ok);
            long tenMin = (long)(10.0 * 60.0 * TimeSpan.TicksPerSecond);
            Assert.IsFalse(g.Update(T0 + tenMin - 1));
            Assert.AreEqual(SanctuaryGuardianStatus.Defeated, g.Status);
            Assert.IsTrue(g.Update(T0 + tenMin));
            Assert.AreEqual(SanctuaryGuardianStatus.Active, g.Status);
        }

        [Test]
        public void CityGuardian_Countdown_Decreases()
        {
            var g = new SanctuaryGuardianState();
            g.RecordDefeat(T0);
            double a = g.CountdownRemainingSeconds(T0);
            double b = g.CountdownRemainingSeconds(T0 + 60 * TimeSpan.TicksPerSecond);
            Assert.Greater(a, b);
            Assert.AreEqual(0, g.CountdownRemainingSeconds(
                T0 + (long)(11 * 60 * TimeSpan.TicksPerSecond)), 0.001);
        }
    }
}
