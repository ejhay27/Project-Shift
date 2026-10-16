using System.Collections.Generic;
using ProjectShift.Core;
using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// MonoBehaviour container owning the Core simulation instances for the
    /// active session (INV-001/004, GATHER-002, ECON-001, TIME-001, TELE-003).
    /// Save/load flows through ToSaveData()/LoadSaveData().
    /// </summary>
    public sealed class WorldState : MonoBehaviour
    {
        public ItemCatalog Catalog { get; private set; }
        public InventorySystem Inventory { get; private set; }
        public ResourceNodeSystem Nodes { get; } = new ResourceNodeSystem();
        public CurrencyWallet Wallet { get; } = new CurrencyWallet();
        public GameClock Clock { get; } = new GameClock(1.0);
        public TeleportNetwork Teleport { get; } = new TeleportNetwork();

        public MobileHud Hud { get; private set; }
        public Transform PlayerTransform { get; private set; }

        public void Initialize(ItemCatalog catalog, int inventoryCapacity,
            MobileHud hud, Transform player)
        {
            Catalog = catalog;
            Inventory = new InventorySystem(catalog, inventoryCapacity);
            Hud = hud;
            PlayerTransform = player;
        }

        public void RegisterNode(ResourceNodeData def) => Nodes.Register(def);

        private void Update()
        {
            // M1: local clock advance. M8: host advances, clients replicate.
            Clock.Advance(Time.deltaTime);
        }

        public PlayerSaveData ToSaveData(string gameVersion)
        {
            Vector3 p = PlayerTransform != null ? PlayerTransform.position : Vector3.zero;
            return new PlayerSaveData
            {
                GameVersion = gameVersion,
                PlayerX = p.x,
                PlayerY = p.y,
                PlayerZ = p.z,
                Wallet = Wallet.ToData(),
                Inventory = Inventory.ToData(),
                Clock = Clock.ToData(),
                Nodes = Nodes.Snapshot(),
                Teleport = Teleport.ToData()
            };
        }

        public void LoadSaveData(PlayerSaveData data, int inventoryCapacity)
        {
            if (data == null) throw new System.ArgumentNullException(nameof(data));
            Wallet.LoadData(data.Wallet ?? new CurrencyWalletData());
            Inventory = InventorySystem.FromData(Catalog,
                data.Inventory ?? new InventoryData { Capacity = inventoryCapacity });
            if (data.Clock != null) Clock.LoadData(data.Clock);
            // Nodes: re-register runtime state onto existing defs is handled
            // by GameBootstrap (defs come from data files; states from save).
            if (data.Teleport != null) Teleport.LoadData(data.Teleport);
            if (PlayerTransform != null)
                PlayerTransform.position = new Vector3(data.PlayerX, data.PlayerY, data.PlayerZ);
        }
    }
}
