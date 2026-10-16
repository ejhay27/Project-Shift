using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for ECON-001.</summary>
    public sealed class CurrencyWalletTests
    {
        [Test]
        public void Spend_MoreThanBalance_FailsWithoutMutation()
        {
            var w = new CurrencyWallet();
            w.Add(Currency.Shards, 10);
            var r = w.TrySpend(Currency.Shards, 11);
            Assert.IsFalse(r.Ok);
            Assert.AreEqual(10, w.Get(Currency.Shards));
        }

        [Test]
        public void Currencies_AreIndependent()
        {
            var w = new CurrencyWallet();
            w.Add(Currency.Credits, 5);
            Assert.AreEqual(0, w.Get(Currency.Shards));
            Assert.AreEqual(0, w.Get(Currency.Aether));
            Assert.AreEqual(5, w.Get(Currency.Credits));
        }

        [Test]
        public void RoundTrip_PreservesBalances()
        {
            var w = new CurrencyWallet();
            w.Add(Currency.Shards, 7);
            w.Add(Currency.Aether, 1);
            var restored = new CurrencyWallet();
            restored.LoadData(w.ToData());
            Assert.AreEqual(7, restored.Get(Currency.Shards));
            Assert.AreEqual(1, restored.Get(Currency.Aether));
        }
    }
}
