using System;
using System.Collections.Generic;

namespace ProjectShift.Core
{
    /// <summary>Three economy currencies per spec §30.</summary>
    public enum Currency
    {
        Shards,  // SHD everyday
        Credits, // CR city commerce
        Aether   // AET rare relic energy
    }

    [Serializable]
    public sealed class CurrencyEntry
    {
        public Currency Currency;
        public long Amount;
    }

    [Serializable]
    public sealed class CurrencyWalletData
    {
        public List<CurrencyEntry> Entries = new List<CurrencyEntry>();
    }

    /// <summary>
    /// Wallet with no-negative invariant (ECON-001). All mutations validated;
    /// failures leave balances untouched.
    /// </summary>
    public sealed class CurrencyWallet
    {
        private readonly Dictionary<Currency, long> _balances = new Dictionary<Currency, long>
        {
            { Currency.Shards, 0 },
            { Currency.Credits, 0 },
            { Currency.Aether, 0 }
        };

        public long Get(Currency currency) => _balances[currency];

        public void Add(Currency currency, long amount)
        {
            Guard.NonNegative(amount, nameof(amount));
            checked { _balances[currency] += amount; }
        }

        public Result TrySpend(Currency currency, long amount)
        {
            if (amount <= 0) return Result.Fail("Amount must be > 0.");
            if (_balances[currency] < amount)
                return Result.Fail($"Insufficient {currency}: have {_balances[currency]}, need {amount}.");
            _balances[currency] -= amount;
            return Result.Success();
        }

        public CurrencyWalletData ToData()
        {
            var data = new CurrencyWalletData();
            foreach (var kv in _balances)
                data.Entries.Add(new CurrencyEntry { Currency = kv.Key, Amount = kv.Value });
            return data;
        }

        public void LoadData(CurrencyWalletData data)
        {
            Guard.NotNull(data, nameof(data));
            foreach (var e in data.Entries)
            {
                if (e == null || e.Amount < 0)
                    throw new ArgumentException("Corrupt wallet entry.");
                _balances[e.Currency] = e.Amount;
            }
        }
    }
}
