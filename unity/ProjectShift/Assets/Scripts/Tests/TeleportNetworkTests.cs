using System;
using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for TELE-003 incl. §36 acceptance.</summary>
    public sealed class TeleportNetworkTests
    {
        private const long T0 = 639000000000000000L;
        private static readonly long FiveMinutes =
            (long)(5.0 * 60.0 * TimeSpan.TicksPerSecond);

        private static TeleportNetwork Home()
        {
            var net = new TeleportNetwork();
            net.Register(new TeleportDestinationData
            {
                Id = "home", DisplayName = "Home", Discovered = true, Active = true
            });
            return net;
        }

        [Test]
        public void Success_TriggersFiveMinuteCooldown()
        {
            var net = Home();
            Assert.IsTrue(net.RecordTeleport("p1", "home", T0).Ok);
            Assert.IsFalse(net.CanTeleport("p1", "home", T0 + FiveMinutes - 1).Ok);
            Assert.IsTrue(net.CanTeleport("p1", "home", T0 + FiveMinutes).Ok);
        }

        [Test]
        public void FailedAttempt_DoesNotConsumeCooldown()
        {
            var net = Home();
            net.SetActive("home", false);
            Assert.IsFalse(net.RecordTeleport("p1", "home", T0).Ok);
            net.SetActive("home", true);
            // Still free to go: the failure stamped nothing.
            Assert.IsTrue(net.CanTeleport("p1", "home", T0).Ok);
        }

        [Test]
        public void Cooldown_Survives_Reload_Rejoin()
        {
            var net = Home();
            net.RecordTeleport("p1", "home", T0);
            // Simulate leave/rejoin: serialize + fresh instance.
            var data = net.ToData();
            var rejoined = new TeleportNetwork();
            rejoined.LoadData(data);
            Assert.IsFalse(rejoined.CanTeleport("p1", "home", T0 + 1).Ok);
            Assert.IsTrue(rejoined.CanTeleport("p1", "home", T0 + FiveMinutes).Ok);
        }

        [Test]
        public void Undiscovered_Destination_Rejected()
        {
            var net = new TeleportNetwork();
            net.Register(new TeleportDestinationData { Id = "far", Discovered = false });
            Assert.IsFalse(net.CanTeleport("p1", "far", T0).Ok);
        }
    }
}
