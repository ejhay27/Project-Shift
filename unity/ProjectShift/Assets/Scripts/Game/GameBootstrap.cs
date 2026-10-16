using System.Collections.Generic;
using ProjectShift.Core;
using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// M1 composition root: loads JSON catalogs, initializes WorldState,
    /// binds player/camera/HUD/save, registers node defs, restores runtime
    /// node states from save when present. DontDestroyOnLoad.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private TextAsset _itemsJson;
        [SerializeField] private TextAsset _nodesJson;
        [SerializeField] private int _inventoryCapacity = 24;

        [Header("Scene refs (wired by builder)")]
        [SerializeField] private WorldState _world;
        [SerializeField] private SaveManager _saves;
        [SerializeField] private MobileHud _hud;
        [SerializeField] private PlayerController _player;
        [SerializeField] private CameraRig _cameraRig;
        [SerializeField] private InteractionManager _interactions;
        [SerializeField] private TouchJoystick _joystick;
        [SerializeField] private TouchPlayerInput _touchInput;
        [SerializeField] private bool _loadSaveOnStart = true;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            // 1. Catalogs from JSON (validated at author time by tools/validate_data.py).
            var items = JsonUtility.FromJson<ItemsJson>(_itemsJson.text);
            var catalog = new ItemCatalog(items.Items);

            // 2. World + wiring.
            _world.Initialize(catalog, _inventoryCapacity, _hud, _player.transform);
            _saves.Bind(_world);
            _interactions.Bind(_player.transform, _hud);
            _cameraRig.SetTarget(_player.transform);
            _player.SetCameraYaw(Camera.main.transform);
            _touchInput.SetJoystick(_joystick);
            _player.SetInput(_touchInput);

            // Desktop Editors get keyboard control instead of touch.
#if UNITY_EDITOR || UNITY_STANDALONE
            var keyboard = _player.GetComponent<KeyboardPlayerInput>()
                ?? _player.gameObject.AddComponent<KeyboardPlayerInput>();
            _player.SetInput(keyboard);
#endif

            _hud.InteractButton.onClick.AddListener(_interactions.TryInteract);
            _hud.JumpButton.onClick.AddListener(_touchInput.OnJumpButton);
            _hud.SaveButton.onClick.AddListener(() => _saves.Save());

            // 3. Node definitions.
            var nodes = JsonUtility.FromJson<ResourceNodesJson>(_nodesJson.text);
            var sceneNodes = FindObjectsByType<ResourceNodeBehaviour>(
                FindObjectsSortMode.None);
            var defsById = new Dictionary<string, ResourceNodeData>();
            foreach (var def in nodes.Nodes) defsById[def.NodeId] = def;
            foreach (var behaviour in sceneNodes)
            {
                if (defsById.TryGetValue(behaviour.NodeId, out var def))
                {
                    // Fresh runtime copy per session; save may overwrite below.
                    _world.RegisterNode(Clone(def));
                    behaviour.Bind(_world, behaviour.NodeId);
                }
            }

            // 4. Restore save (node states re-applied onto live defs).
            if (_loadSaveOnStart && _saves.HasSave())
            {
                var result = _saves.Load(_inventoryCapacity);
                if (result.Ok) ReapplyNodeStates();
                else _hud.ShowToast("Starting new game: " + result.Error);
            }
            else
            {
                _hud.ShowToast("New game — WASD/arrows or left stick to move.");
            }
        }

        private void ReapplyNodeStates()
        {
            // Load() populated WorldState; node runtime states were restored
            // into the save payload list. Re-register states onto live defs.
            // (M1 keeps defs static; M2 merges full node persistence here.)
            foreach (var behaviour in FindObjectsByType<ResourceNodeBehaviour>(
                FindObjectsSortMode.None))
            {
                behaviour.Bind(_world, behaviour.NodeId);
            }
        }

        private static ResourceNodeData Clone(ResourceNodeData def)
        {
            return new ResourceNodeData
            {
                NodeId = def.NodeId,
                ResourceId = def.ResourceId,
                YieldMin = def.YieldMin,
                YieldMax = def.YieldMax,
                RequiredToolTier = def.RequiredToolTier,
                RespawnMinutes = def.RespawnMinutes,
                RegionId = def.RegionId,
                State = ResourceNodeState.Available,
                DepletedAtUtcTicks = 0
            };
        }
    }
}
