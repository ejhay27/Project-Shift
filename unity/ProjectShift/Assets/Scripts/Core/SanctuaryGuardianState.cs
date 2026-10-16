using System;

namespace ProjectShift.Core
{
    public enum SanctuaryGuardianStatus
    {
        Active,
        Defeated // dormant; resurrects after the timer (NOT permanent)
    }

    [Serializable]
    public sealed class SanctuaryGuardianData
    {
        public SanctuaryGuardianStatus Status = SanctuaryGuardianStatus.Active;
        public long DefeatedAtUtcTicks;
    }

    /// <summary>
    /// City Guardian defeat/resurrection (GUARD-003): ~10-minute dormant
    /// window with a visible countdown, then resurrection. Unlike the World
    /// Tree Guardian, this one is designed to return.
    /// </summary>
    public sealed class SanctuaryGuardianState
    {
        public const double ResurrectAfterMinutes = 10.0;

        public SanctuaryGuardianStatus Status { get; private set; } = SanctuaryGuardianStatus.Active;
        public long DefeatedAtUtcTicks { get; private set; }

        public Result RecordDefeat(long nowUtcTicks)
        {
            if (Status == SanctuaryGuardianStatus.Defeated)
                return Result.Fail("Guardian already dormant.");
            Status = SanctuaryGuardianStatus.Defeated;
            DefeatedAtUtcTicks = nowUtcTicks;
            return Result.Success();
        }

        public double CountdownRemainingSeconds(long nowUtcTicks)
        {
            if (Status == SanctuaryGuardianStatus.Active) return 0;
            long ready = DefeatedAtUtcTicks +
                (long)(ResurrectAfterMinutes * 60.0 * TimeSpan.TicksPerSecond);
            long left = ready - nowUtcTicks;
            return left > 0 ? left / (double)TimeSpan.TicksPerSecond : 0;
        }

        /// <summary>Returns true on the tick the guardian resurrects.</summary>
        public bool Update(long nowUtcTicks)
        {
            if (Status == SanctuaryGuardianStatus.Active) return false;
            if (CountdownRemainingSeconds(nowUtcTicks) > 0) return false;
            Status = SanctuaryGuardianStatus.Active;
            DefeatedAtUtcTicks = 0;
            return true;
        }

        public SanctuaryGuardianData ToData()
        {
            return new SanctuaryGuardianData
            {
                Status = Status,
                DefeatedAtUtcTicks = DefeatedAtUtcTicks
            };
        }

        public void LoadData(SanctuaryGuardianData data)
        {
            Guard.NotNull(data, nameof(data));
            Status = data.Status;
            DefeatedAtUtcTicks = data.DefeatedAtUtcTicks;
        }
    }
}
