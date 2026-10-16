using System;
using System.Collections.Generic;

namespace ProjectShift.Core
{
    [Serializable]
    public sealed class ItemStackData
    {
        public string ItemId;
        public int Count;
    }

    [Serializable]
    public sealed class InventoryData
    {
        public int Capacity;
        public List<ItemStackData> Stacks = new List<ItemStackData>();
    }

    /// <summary>
    /// Slot-based inventory (INV-001). Operations are atomic: validation runs
    /// before any mutation, so failures never half-apply (dupe-proofing).
    /// </summary>
    public sealed class InventorySystem
    {
        private sealed class Slot
        {
            public string ItemId;
            public int Count;
        }

        private readonly List<Slot> _slots = new List<Slot>();
        private readonly ItemCatalog _catalog;

        public int Capacity { get; }
        public int UsedSlots => _slots.Count;

        public InventorySystem(ItemCatalog catalog, int capacity)
        {
            Guard.NotNull(catalog, nameof(catalog));
            Guard.Positive(capacity, nameof(capacity));
            _catalog = catalog;
            Capacity = capacity;
        }

        public int GetCount(string itemId)
        {
            int total = 0;
            for (int i = 0; i < _slots.Count; i++)
                if (_slots[i].ItemId == itemId) total += _slots[i].Count;
            return total;
        }

        /// <summary>Add items. Value = leftover count that did not fit.</summary>
        public Result<int> Add(string itemId, int count)
        {
            if (!_catalog.TryGet(itemId, out var def))
                return Result<int>.Fail($"Unknown item '{itemId}'.");
            if (count <= 0) return Result<int>.Fail("Count must be > 0.");

            int remaining = count;

            // Fill existing stacks first.
            for (int i = 0; i < _slots.Count && remaining > 0; i++)
            {
                if (_slots[i].ItemId != itemId) continue;
                int space = def.StackLimit - _slots[i].Count;
                int take = Math.Min(space, remaining);
                _slots[i].Count += take;
                remaining -= take;
            }

            // Open new slots while capacity allows.
            while (remaining > 0 && _slots.Count < Capacity)
            {
                int take = Math.Min(def.StackLimit, remaining);
                _slots.Add(new Slot { ItemId = itemId, Count = take });
                remaining -= take;
            }

            return Result<int>.Success(remaining);
        }

        /// <summary>Remove items. Fails atomically if insufficient.</summary>
        public Result Remove(string itemId, int count)
        {
            if (count <= 0) return Result.Fail("Count must be > 0.");
            if (GetCount(itemId) < count)
                return Result.Fail($"Insufficient '{itemId}': need {count}.");

            int remaining = count;
            for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
            {
                if (_slots[i].ItemId != itemId) continue;
                int take = Math.Min(_slots[i].Count, remaining);
                _slots[i].Count -= take;
                remaining -= take;
                if (_slots[i].Count == 0) _slots.RemoveAt(i);
            }
            return Result.Success();
        }

        /// <summary>Swap two slots (inventory UX support).</summary>
        public Result Move(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= _slots.Count) return Result.Fail("Bad from-index.");
            if (toIndex < 0 || toIndex >= _slots.Count) return Result.Fail("Bad to-index.");
            var tmp = _slots[fromIndex];
            _slots[fromIndex] = _slots[toIndex];
            _slots[toIndex] = tmp;
            return Result.Success();
        }

        public InventoryData ToData()
        {
            var data = new InventoryData { Capacity = Capacity };
            foreach (var s in _slots)
                data.Stacks.Add(new ItemStackData { ItemId = s.ItemId, Count = s.Count });
            return data;
        }

        public static InventorySystem FromData(ItemCatalog catalog, InventoryData data)
        {
            Guard.NotNull(catalog, nameof(catalog));
            Guard.NotNull(data, nameof(data));
            var inv = new InventorySystem(catalog, Math.Max(1, data.Capacity));
            foreach (var s in data.Stacks)
            {
                if (s == null || s.Count <= 0) throw new ArgumentException("Corrupt stack.");
                var r = inv.Add(s.ItemId, s.Count);
                if (!r.Ok || r.Value != 0) throw new ArgumentException($"Cannot restore stack '{s.ItemId}'.");
            }
            return inv;
        }
    }
}
