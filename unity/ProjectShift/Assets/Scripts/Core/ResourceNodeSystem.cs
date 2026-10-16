using System;
using System.Collections.Generic;

namespace ProjectShift.Core
{
    public enum ResourceNodeState
    {
        Available,
        Depleted
    }

    /// <summary>JsonUtility-safe node definition + runtime state.</summary>
    [Serializable]
    public sealed class ResourceNodeData
    {
        public string NodeId;
        public string ResourceId;
        public int YieldMin = 1;
        public int YieldMax = 1;
        public int RequiredToolTier;
        public double RespawnMinutes = 10.0;
        public string RegionId;
        public ResourceNodeState State = ResourceNodeState.Available;
        public long DepletedAtUtcTicks;
    }

    public sealed class HarvestResult
    {
        public string ResourceId;
        public int Yield;
    }

    /// <summary>
    /// Deterministic resource nodes (GATHER-002). Respawn is a pure function of
    /// UTC timestamps so offline progress is automatic; occupancy callback lets
    /// the Game layer defer respawns inside players/buildings/roads (§36).
    /// </summary>
    public sealed class ResourceNodeSystem
    {
        public const double DefaultStoneRespawnMinutes = 10.0;

        private readonly Dictionary<string, ResourceNodeData> _nodes =
            new Dictionary<string, ResourceNodeData>(StringComparer.Ordinal);

        public int Count => _nodes.Count;

        public void Register(ResourceNodeData def)
        {
            Guard.NotNull(def, nameof(def));
            Guard.NotNullOrEmpty(def.NodeId, "NodeId");
            Guard.NotNullOrEmpty(def.ResourceId, "ResourceId");
            if (def.YieldMin <= 0 || def.YieldMax < def.YieldMin)
                throw new ArgumentException($"Bad yield range on '{def.NodeId}'.");
            if (def.RespawnMinutes <= 0)
                throw new ArgumentException($"Bad respawn on '{def.NodeId}'.");
            if (_nodes.ContainsKey(def.NodeId))
                throw new ArgumentException($"Duplicate node '{def.NodeId}'.");
            _nodes.Add(def.NodeId, def);
        }

        public bool TryGet(string nodeId, out ResourceNodeData node) =>
            _nodes.TryGetValue(nodeId, out node);

        /// <summary>
        /// Harvest a node. Yield roller is injectable (Game layer supplies RNG);
        /// default is deterministic YieldMin so Core stays side-effect free.
        /// </summary>
        public Result<HarvestResult> Harvest(string nodeId, int toolTier, long nowUtcTicks,
            Func<int, int, int> rollYield = null)
        {
            if (!_nodes.TryGetValue(nodeId, out var node))
                return Result<HarvestResult>.Fail($"Unknown node '{nodeId}'.");
            if (node.State != ResourceNodeState.Available)
                return Result<HarvestResult>.Fail("Node is depleted.");
            if (toolTier < node.RequiredToolTier)
                return Result<HarvestResult>.Fail("Tool tier too low.");

            int amount = rollYield != null
                ? rollYield(node.YieldMin, node.YieldMax)
                : node.YieldMin;
            amount = Math.Max(node.YieldMin, Math.Min(node.YieldMax, amount));

            node.State = ResourceNodeState.Depleted;
            node.DepletedAtUtcTicks = nowUtcTicks;
            return Result<HarvestResult>.Success(new HarvestResult
            {
                ResourceId = node.ResourceId,
                Yield = amount
            });
        }

        /// <summary>
        /// Respawn pass. Nodes due AND unoccupied respawn; occupied ones wait.
        /// Returns respawned node ids. MP: host runs this, replicates results.
        /// </summary>
        public List<string> UpdateRespawns(long nowUtcTicks, Func<string, bool> isOccupied)
        {
            Guard.NotNull(isOccupied, nameof(isOccupied));
            var respawned = new List<string>();
            foreach (var kv in _nodes)
            {
                var node = kv.Value;
                if (node.State != ResourceNodeState.Depleted) continue;
                long dueTicks = node.DepletedAtUtcTicks +
                    (long)(node.RespawnMinutes * 60.0 * TimeSpan.TicksPerSecond);
                if (nowUtcTicks < dueTicks) continue;
                if (isOccupied(node.NodeId)) continue; // stays depleted; retry later
                node.State = ResourceNodeState.Available;
                node.DepletedAtUtcTicks = 0;
                respawned.Add(node.NodeId);
            }
            return respawned;
        }

        public List<ResourceNodeData> Snapshot()
        {
            return new List<ResourceNodeData>(_nodes.Values);
        }
    }
}
