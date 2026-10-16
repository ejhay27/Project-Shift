using ProjectShift.Core;
using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// Visible bridge for one ResourceNodeSystem entry (GATHER-002):
    /// harvest interaction, full/depleted visuals, respawn polling with
    /// occupancy check (never respawns inside the player).
    /// </summary>
    public sealed class ResourceNodeBehaviour : Interactable
    {
        [SerializeField] private string _nodeId;
        [SerializeField] private GameObject _fullVisual;
        [SerializeField] private GameObject _depletedVisual;
        [SerializeField] private int _toolTier = 0;
        [SerializeField] private float _respawnPollSeconds = 5f;

        private WorldState _world;
        private float _nextPoll;

        public string NodeId => _nodeId;

        public void Bind(WorldState world, string nodeId)
        {
            _world = world;
            _nodeId = nodeId;
            RefreshVisual();
        }

        public override string Prompt
        {
            get
            {
                if (_world == null) return "Harvest";
                if (_world.Nodes.TryGet(_nodeId, out var node) &&
                    node.State == ResourceNodeState.Available)
                    return $"Harvest ({node.ResourceId})";
                return "Depleted";
            }
        }

        public override bool CanInteract(GameObject user)
        {
            return _world != null
                && _world.Nodes.TryGet(_nodeId, out var node)
                && node.State == ResourceNodeState.Available;
        }

        public override void Interact(GameObject user)
        {
            if (_world == null) return;
            long now = DateTimeProxy.UtcNowTicks;
            var result = _world.Nodes.Harvest(_nodeId, _toolTier, now, RollYield);
            if (!result.Ok)
            {
                _world.Hud?.ShowToast(result.Error);
                return;
            }
            var add = _world.Inventory.Add(result.Value.ResourceId, result.Value.Yield);
            if (!add.Ok || add.Value > 0)
            {
                // Inventory full: in M1 we keep it simple and report; M2 adds
                // ground-drop / partial-yield handling (deadlock-free: the node
                // stays harvested; no silent item loss — toast + journal).
                _world.Hud?.ShowToast("Inventory full!");
            }
            else
            {
                _world.Hud?.ShowToast($"+{result.Value.Yield} {result.Value.ResourceId}");
            }
            RefreshVisual();
        }

        private void Update()
        {
            if (_world == null || Time.time < _nextPoll) return;
            _nextPoll = Time.time + _respawnPollSeconds;
            long now = DateTimeProxy.UtcNowTicks;
            var respawned = _world.Nodes.UpdateRespawns(now, IsOccupied);
            if (respawned.Contains(_nodeId)) RefreshVisual();
        }

        private bool IsOccupied(string nodeId)
        {
            // M1 occupancy rule: a live player collider overlapping the node
            // point blocks respawn. Buildings/roads join in M2 (BUILD-005).
            var hits = Physics.OverlapSphere(transform.position, 1.0f);
            foreach (var h in hits)
                if (h.GetComponentInParent<PlayerController>() != null)
                    return true;
            return false;
        }

        private int RollYield(int min, int max) => Random.Range(min, max + 1);

        private void RefreshVisual()
        {
            bool available = _world == null || !_world.Nodes.TryGet(_nodeId, out var node)
                || node.State == ResourceNodeState.Available;
            if (_fullVisual != null) _fullVisual.SetActive(available);
            if (_depletedVisual != null) _depletedVisual.SetActive(!available);
        }
    }
}
