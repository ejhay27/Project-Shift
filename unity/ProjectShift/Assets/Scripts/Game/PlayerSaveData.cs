using System;
using System.Collections.Generic;
using ProjectShift.Core;

namespace ProjectShift.Game
{
    /// <summary>
    /// JsonUtility-safe aggregate save payload (INV-004). Grows each
    /// milestone toward the full §31 list (INV-005).
    /// </summary>
    [Serializable]
    public sealed class PlayerSaveData
    {
        public string GameVersion = "0.1.0-m1";
        public float PlayerX;
        public float PlayerY;
        public float PlayerZ;
        public CurrencyWalletData Wallet = new CurrencyWalletData();
        public InventoryData Inventory = new InventoryData();
        public GameClockData Clock = new GameClockData();
        public List<ResourceNodeData> Nodes = new List<ResourceNodeData>();
        public TeleportNetworkData Teleport = new TeleportNetworkData();
    }

    // --- JSON data-file wrappers (Assets/GameData/*.json) ---

    [Serializable]
    public sealed class ItemsJson
    {
        public List<ItemDefinition> Items = new List<ItemDefinition>();
    }

    [Serializable]
    public sealed class ResourceNodesJson
    {
        public List<ResourceNodeData> Nodes = new List<ResourceNodeData>();
    }
}
