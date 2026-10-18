using System;

namespace ProjectShift.Core
{
    /// <summary>
    /// The World Tree Guardian's permanent outcome (TREEG-007).
    /// Mutually exclusive by spec §8: slay-loot XOR bond-pet, never both.
    /// </summary>
    public enum GuardianOutcome
    {
        Unresolved = 0,
        BossSlain = 1,  // Outcome A: loot + access, never respawns, no pet
        PetBonded = 2   // Outcome B: pet form, no boss-kill loot
    }

    [Serializable]
    public sealed class WorldTreeGuardianData
    {
        public GuardianOutcome Outcome = GuardianOutcome.Unresolved;
        public long ResolvedAtUtcTicks;
    }

    public sealed class WorldTreeGuardianOutcome
    {
        public GuardianOutcome Outcome { get; private set; } = GuardianOutcome.Unresolved;
        public long ResolvedAtUtcTicks { get; private set; }

        /// <summary>The unique guardian never respawns in either outcome.</summary>
        public bool GuardianRespawns => false;

        public Result TryRecordBossSlain(long nowUtcTicks)
        {
            if (Outcome != GuardianOutcome.Unresolved)
                return Result.Fail($"Guardian already resolved as {Outcome}.");
            Outcome = GuardianOutcome.BossSlain;
            ResolvedAtUtcTicks = nowUtcTicks;
            return Result.Success();
        }

        public Result TryRecordPetBonded(long nowUtcTicks)
        {
            if (Outcome != GuardianOutcome.Unresolved)
                return Result.Fail($"Guardian already resolved as {Outcome}.");
            Outcome = GuardianOutcome.PetBonded;
            ResolvedAtUtcTicks = nowUtcTicks;
            return Result.Success();
        }

        public WorldTreeGuardianData ToData()
        {
            return new WorldTreeGuardianData
            {
                Outcome = Outcome,
                ResolvedAtUtcTicks = ResolvedAtUtcTicks
            };
        }

        public void LoadData(WorldTreeGuardianData data)
        {
            Guard.NotNull(data, nameof(data));
            Outcome = data.Outcome;
            ResolvedAtUtcTicks = data.ResolvedAtUtcTicks;
        }
    }
}
