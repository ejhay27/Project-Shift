using System;
using System.Collections.Generic;

namespace ProjectShift.Core
{
    /// <summary>
    /// Data-driven item definition (INV-001). Mirrors Assets/GameData/items.json.
    /// JsonUtility-safe.
    /// </summary>
    [Serializable]
    public sealed class ItemDefinition
    {
        public string Id;
        public string DisplayName;
        public string Category;
        public int StackLimit = 99;
        public string Description;

        public Result Validate()
        {
            if (string.IsNullOrEmpty(Id)) return Result.Fail("Item id missing.");
            if (string.IsNullOrEmpty(DisplayName)) return Result.Fail($"Item '{Id}' has no display name.");
            if (StackLimit <= 0) return Result.Fail($"Item '{Id}' has invalid stack limit.");
            return Result.Success();
        }
    }

    /// <summary>Validated lookup catalog for item definitions.</summary>
    public sealed class ItemCatalog
    {
        private readonly Dictionary<string, ItemDefinition> _defs =
            new Dictionary<string, ItemDefinition>(StringComparer.Ordinal);

        public ItemCatalog(IEnumerable<ItemDefinition> definitions)
        {
            Guard.NotNull(definitions, nameof(definitions));
            foreach (var def in definitions)
            {
                if (def == null) throw new ArgumentException("Null item definition.");
                var r = def.Validate();
                if (!r.Ok) throw new ArgumentException(r.Error);
                if (_defs.ContainsKey(def.Id))
                    throw new ArgumentException($"Duplicate item id '{def.Id}'.");
                _defs.Add(def.Id, def);
            }
        }

        public int Count => _defs.Count;

        public bool TryGet(string id, out ItemDefinition def) => _defs.TryGetValue(id, out def);
    }
}
