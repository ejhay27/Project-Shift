using System;

namespace ProjectShift.Core
{
    /// <summary>
    /// Lightweight operation result so gameplay rules report failures
    /// without throwing exceptions in hot paths. M0 foundation.
    /// </summary>
    public readonly struct Result
    {
        public bool Ok { get; }
        public string Error { get; }

        private Result(bool ok, string error)
        {
            Ok = ok;
            Error = error;
        }

        public static Result Success() => new Result(true, string.Empty);
        public static Result Fail(string error) => new Result(false, error ?? "Unknown error.");
    }

    /// <summary>Result carrying a value on success.</summary>
    public readonly struct Result<T>
    {
        public bool Ok { get; }
        public string Error { get; }
        public T Value { get; }

        private Result(bool ok, T value, string error)
        {
            Ok = ok;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, string.Empty);
        public static Result<T> Fail(string error) => new Result<T>(false, default, error ?? "Unknown error.");
    }

    /// <summary>Argument guards for fail-fast constructor validation.</summary>
    public static class Guard
    {
        public static void NotNull(object value, string name)
        {
            if (value == null) throw new ArgumentNullException(name);
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("Must not be null or empty.", name);
        }

        public static void Positive(int value, string name)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(name, "Must be > 0.");
        }

        public static void NonNegative(long value, string name)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, "Must be >= 0.");
        }
    }
}
