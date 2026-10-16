using System;
using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for GATHER-002 incl. §36 acceptance.</summary>
    public sealed class ResourceNodeTests
    {
        private const long T0 = 639000000000000000L; // fixed UTC ticks
        private static readonly long TenMinutes =
            (long)(10.0 * 60.0 * TimeSpan.TicksPerSecond);

        private static ResourceNodeSystem OneStoneNode()
        {
            var sys = new ResourceNodeSystem();
            sys.Register(new ResourceNodeData
            {
                NodeId = "stone.01",
                ResourceId = "stone.common",
                YieldMin = 2,
                YieldMax = 3,
                RespawnMinutes = ResourceNodeSystem.DefaultStoneRespawnMinutes
            });
            return sys;
        }

        [Test]
        public void Stone_Respawns_AfterTenMinutes()
        {
            var sys = OneStoneNode();
            var h = sys.Harvest("stone.01", 0, T0);
            Assert.IsTrue(h.Ok);

            var early = sys.UpdateRespawns(T0 + TenMinutes - 1, _ => false);
            Assert.AreEqual(0, early.Count); // not yet

            var due = sys.UpdateRespawns(T0 + TenMinutes, _ => false);
            Assert.AreEqual(1, due.Count);
            Assert.AreEqual("stone.01", due[0]);
        }

        [Test]
        public void Respawn_Blocked_WhenOccupied_RetriesLater()
        {
            var sys = OneStoneNode();
            sys.Harvest("stone.01", 0, T0);
            // Due but occupied (player/building/road) → stays depleted.
            var blocked = sys.UpdateRespawns(T0 + TenMinutes, _ => true);
            Assert.AreEqual(0, blocked.Count);
            // Same timestamp, now free → respawns (no lost timer).
            var freed = sys.UpdateRespawns(T0 + TenMinutes, _ => false);
            Assert.AreEqual(1, freed.Count);
        }

        [Test]
        public void OfflineProgress_Works_FromTimestamps()
        {
            var sys = OneStoneNode();
            sys.Harvest("stone.01", 0, T0);
            // Simulate "game was closed for an hour": one pass catches up.
            var respawned = sys.UpdateRespawns(T0 + 60 * 60 * TimeSpan.TicksPerSecond,
                _ => false);
            Assert.AreEqual(1, respawned.Count);
        }

        [Test]
        public void Harvest_DepletedNode_Fails()
        {
            var sys = OneStoneNode();
            sys.Harvest("stone.01", 0, T0);
            var again = sys.Harvest("stone.01", 0, T0 + 1);
            Assert.IsFalse(again.Ok);
        }
    }
}
