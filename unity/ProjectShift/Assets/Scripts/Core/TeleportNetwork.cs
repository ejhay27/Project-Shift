using System;
using System.Collections.Generic;

namespace ProjectShift.Core
{
    [Serializable]
    public sealed class TeleportDestinationData
    {
        public string Id;
        public string DisplayName;
        public bool Discovered;
        public bool Active = true;
    }

    [Serializable]
    public sealed class TeleportCooldownData
    {
        public string PlayerId;
        public long LastTeleportUtcTicks;
    }

    [Serializable]
    public sealed class TeleportNetworkData
    {
        public List<TeleportDestinationData> Destinations = new List<TeleportDestinationData>();
        public List<TeleportCooldownData> Cooldowns = new List<TeleportCooldownData>();
    }

    /// <summary>
    /// Wayfarer/Waygate rules (TELE-003). 5-minute cooldown per player, stamped
    /// ONLY on successful teleport; persisted per playerId so leave/rejoin
    /// cannot bypass it (§36). MP: host executes RecordTeleport.
    /// </summary>
    public sealed class TeleportNetwork
    {
        public const double CooldownMinutes = 5.0;

        private readonly Dictionary<string, TeleportDestinationData> _dests =
            new Dictionary<string, TeleportDestinationData>(StringComparer.Ordinal);
        private readonly Dictionary<string, long> _cooldowns =
            new Dictionary<string, long>(StringComparer.Ordinal);

        public void Register(TeleportDestinationData dest)
        {
            Guard.NotNull(dest, nameof(dest));
            Guard.NotNullOrEmpty(dest.Id, "Id");
            if (_dests.ContainsKey(dest.Id))
                throw new ArgumentException($"Duplicate destination '{dest.Id}'.");
            _dests.Add(dest.Id, dest);
        }

        public Result Discover(string destId)
        {
            if (!_dests.TryGetValue(destId, out var d))
                return Result.Fail($"Unknown destination '{destId}'.");
            d.Discovered = true;
            return Result.Success();
        }

        public Result SetActive(string destId, bool active)
        {
            if (!_dests.TryGetValue(destId, out var d))
                return Result.Fail($"Unknown destination '{destId}'.");
            d.Active = active;
            return Result.Success();
        }

        public double CooldownRemainingSeconds(string playerId, long nowUtcTicks)
        {
            if (!_cooldowns.TryGetValue(playerId, out var last)) return 0;
            long ready = last + (long)(CooldownMinutes * 60.0 * TimeSpan.TicksPerSecond);
            long left = ready - nowUtcTicks;
            return left > 0 ? left / (double)TimeSpan.TicksPerSecond : 0;
        }

        public Result CanTeleport(string playerId, string destId, long nowUtcTicks)
        {
            if (!_dests.TryGetValue(destId, out var d))
                return Result.Fail($"Unknown destination '{destId}'.");
            if (!d.Discovered) return Result.Fail("Destination not discovered.");
            if (!d.Active) return Result.Fail("Destination is inactive.");
            double left = CooldownRemainingSeconds(playerId, nowUtcTicks);
            if (left > 0)
                return Result.Fail($"Teleport cooling down ({Math.Ceiling(left)}s left).");
            return Result.Success();
        }

        /// <summary>Validate then stamp cooldown. Call only for real teleports.</summary>
        public Result RecordTeleport(string playerId, string destId, long nowUtcTicks)
        {
            var check = CanTeleport(playerId, destId, nowUtcTicks);
            if (!check.Ok) return check; // failed attempt consumes nothing
            _cooldowns[playerId] = nowUtcTicks;
            return Result.Success();
        }

        public TeleportNetworkData ToData()
        {
            var data = new TeleportNetworkData();
            foreach (var d in _dests.Values)
                data.Destinations.Add(new TeleportDestinationData
                {
                    Id = d.Id,
                    DisplayName = d.DisplayName,
                    Discovered = d.Discovered,
                    Active = d.Active
                });
            foreach (var kv in _cooldowns)
                data.Cooldowns.Add(new TeleportCooldownData
                {
                    PlayerId = kv.Key,
                    LastTeleportUtcTicks = kv.Value
                });
            return data;
        }

        public void LoadData(TeleportNetworkData data)
        {
            Guard.NotNull(data, nameof(data));
            _dests.Clear();
            _cooldowns.Clear();
            foreach (var d in data.Destinations) Register(d);
            foreach (var c in data.Cooldowns)
            {
                if (c == null || string.IsNullOrEmpty(c.PlayerId))
                    throw new ArgumentException("Corrupt cooldown entry.");
                _cooldowns[c.PlayerId] = c.LastTeleportUtcTicks;
            }
        }
    }
}
