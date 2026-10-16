using System;

namespace ProjectShift.Game
{
    /// <summary>
    /// Single clock gateway for the Game layer. Core never touches the clock
    /// directly (determinism); MP later swaps this for host-synced time.
    /// Ticks are UTC (GATHER-007 offline progress, TELE-003 anti-bypass).
    /// </summary>
    public static class DateTimeProxy
    {
        public static long UtcNowTicks => DateTime.UtcNow.Ticks;
    }
}
