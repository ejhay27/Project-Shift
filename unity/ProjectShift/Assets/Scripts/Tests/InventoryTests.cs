using System.Collections.Generic;
using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for INV-001. Run in Unity Test Runner.</summary>
    public sealed class InventoryTests
    {
        private static ItemCatalog TestCatalog()
        {
            return new ItemCatalog(new List<ItemDefinition>
            {
                new ItemDefinition { Id = "wood.oak", DisplayName = "Oak", StackLimit = 10 },
                new ItemDefinition { Id = "tool.axe", DisplayName = "Axe", StackLimit = 1 }
            });
        }

        [Test]
        public void Add_StacksUpToLimit_ThenOpensNewSlot()
        {
            var inv = new InventorySystem(TestCatalog(), 4);
            var r = inv.Add("wood.oak", 25);
            Assert.IsTrue(r.Ok);
            Assert.AreEqual(0, r.Value);
            Assert.AreEqual(25, inv.GetCount("wood.oak"));
            Assert.AreEqual(3, inv.UsedSlots);
        }

        [Test]
        public void Add_RespectsCapacity_ReturnsLeftover()
        {
            var inv = new InventorySystem(TestCatalog(), 1);
            var r = inv.Add("wood.oak", 15);
            Assert.IsTrue(r.Ok);
            Assert.AreEqual(5, r.Value);
            Assert.AreEqual(10, inv.GetCount("wood.oak"));
        }

        [Test]
        public void Add_UnknownItem_FailsWithoutMutation()
        {
            var inv = new InventorySystem(TestCatalog(), 4);
            var r = inv.Add("nope.missing", 3);
            Assert.IsFalse(r.Ok);
            Assert.AreEqual(0, inv.UsedSlots);
        }

        [Test]
        public void Remove_IsAtomic_OnInsufficientStock()
        {
            var inv = new InventorySystem(TestCatalog(), 4);
            inv.Add("wood.oak", 5);
            var r = inv.Remove("wood.oak", 6);
            Assert.IsFalse(r.Ok);
            Assert.AreEqual(5, inv.GetCount("wood.oak")); // untouched
        }

        [Test]
        public void RoundTrip_ToData_FromData_PreservesStacks()
        {
            var inv = new InventorySystem(TestCatalog(), 4);
            inv.Add("wood.oak", 12);
            inv.Add("tool.axe", 1);
            var restored = InventorySystem.FromData(TestCatalog(), inv.ToData());
            Assert.AreEqual(12, restored.GetCount("wood.oak"));
            Assert.AreEqual(1, restored.GetCount("tool.axe"));
        }
    }
}
