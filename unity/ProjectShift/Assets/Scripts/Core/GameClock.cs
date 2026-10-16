using System;

namespace ProjectShift.Core
{
    /// <summary>Day phases per spec §28.</summary>
    public enum DayPhase
    {
        Night,
        Morning,
        Daytime,
        Afternoon,
        Sunset
    }

    /// <summary>JsonUtility-safe DTO for save files.</summary>
    [Serializable]
    public sealed class GameClockData
    {
        public double TotalGameMinutes;
        public double GameMinutesPerRealSecond = 1.0;
    }

    /// <summary>
    /// Deterministic world clock (TIME-001). Pure function of elapsed real
    /// seconds; the host advances it and replicates it in multiplayer.
    /// </summary>
    public sealed class GameClock
    {
        public const double MinutesPerDay = 1440.0;

        public double TotalGameMinutes { get; private set; }
        public double GameMinutesPerRealSecond { get; private set; }

        public GameClock(double gameMinutesPerRealSecond = 1.0)
        {
            if (gameMinutesPerRealSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(gameMinutesPerRealSecond));
            GameMinutesPerRealSecond = gameMinutesPerRealSecond;
        }

        public void Advance(double realSeconds)
        {
            if (realSeconds < 0) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            TotalGameMinutes += realSeconds * GameMinutesPerRealSecond;
        }

        /// <summary>Skip time (sleep/rest). MP: host-only call.</summary>
        public void SkipMinutes(double gameMinutes)
        {
            if (gameMinutes < 0) throw new ArgumentOutOfRangeException(nameof(gameMinutes));
            TotalGameMinutes += gameMinutes;
        }

        public int DayCount => (int)(TotalGameMinutes / MinutesPerDay) + 1;

        public double TimeOfDay01
        {
            get
            {
                double m = TotalGameMinutes % MinutesPerDay;
                return m / MinutesPerDay;
            }
        }

        public DayPhase Phase
        {
            get
            {
                double hour = TimeOfDay01 * 24.0;
                if (hour >= 5.0 && hour < 9.0) return DayPhase.Morning;
                if (hour >= 9.0 && hour < 16.0) return DayPhase.Daytime;
                if (hour >= 16.0 && hour < 18.5) return DayPhase.Afternoon;
                if (hour >= 18.5 && hour < 20.5) return DayPhase.Sunset;
                return DayPhase.Night;
            }
        }

        public GameClockData ToData()
        {
            return new GameClockData
            {
                TotalGameMinutes = TotalGameMinutes,
                GameMinutesPerRealSecond = GameMinutesPerRealSecond
            };
        }

        public void LoadData(GameClockData data)
        {
            Guard.NotNull(data, nameof(data));
            if (data.TotalGameMinutes < 0) throw new ArgumentOutOfRangeException(nameof(data));
            TotalGameMinutes = data.TotalGameMinutes;
            if (data.GameMinutesPerRealSecond > 0)
                GameMinutesPerRealSecond = data.GameMinutesPerRealSecond;
        }
    }
}
