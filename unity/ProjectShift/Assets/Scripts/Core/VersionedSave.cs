using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjectShift.Core
{
    /// <summary>Save-schema version. Bump with a migration on breaking change.</summary>
    public static class SaveConstants
    {
        public const int SchemaVersion = 1;
    }

    /// <summary>
    /// Save envelope (INV-004). JsonUtility-safe. Serialization of the envelope
    /// itself is done by the caller (Unity JsonUtility) so Core stays free of
    /// JSON dependencies; checksum + atomic-write live here.
    /// </summary>
    [Serializable]
    public sealed class SaveEnvelope
    {
        public int SchemaVersion;
        public string GameVersion;
        public long SavedAtUtcTicks;
        public string PayloadJson;
        public string ChecksumSha256;
    }

    public static class VersionedSave
    {
        public static string ComputeChecksum(string payloadJson)
        {
            Guard.NotNull(payloadJson, nameof(payloadJson));
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(payloadJson);
                byte[] hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static SaveEnvelope Create(string gameVersion, string payloadJson, long nowUtcTicks)
        {
            Guard.NotNullOrEmpty(gameVersion, nameof(gameVersion));
            Guard.NotNull(payloadJson, nameof(payloadJson));
            return new SaveEnvelope
            {
                SchemaVersion = SaveConstants.SchemaVersion,
                GameVersion = gameVersion,
                SavedAtUtcTicks = nowUtcTicks,
                PayloadJson = payloadJson,
                ChecksumSha256 = ComputeChecksum(payloadJson)
            };
        }

        public static Result Validate(SaveEnvelope envelope, int expectedSchema)
        {
            if (envelope == null) return Result.Fail("Envelope is null.");
            if (envelope.SchemaVersion != expectedSchema)
                return Result.Fail($"Schema mismatch: have {envelope.SchemaVersion}, want {expectedSchema}.");
            if (envelope.PayloadJson == null) return Result.Fail("Payload missing.");
            string actual = ComputeChecksum(envelope.PayloadJson);
            if (!string.Equals(actual, envelope.ChecksumSha256, StringComparison.OrdinalIgnoreCase))
                return Result.Fail("Checksum mismatch (corrupt save).");
            return Result.Success();
        }

        /// <summary>
        /// Crash-safe write: content → path.tmp → rotate path → path.bak →
        /// move tmp into place. A valid save is never overwritten by a
        /// partial write (§36).
        /// </summary>
        public static Result AtomicWrite(string path, string fileContent)
        {
            try
            {
                Guard.NotNullOrEmpty(path, nameof(path));
                Guard.NotNull(fileContent, nameof(fileContent));
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string tmp = path + ".tmp";
                string bak = path + ".bak";
                File.WriteAllText(tmp, fileContent, Encoding.UTF8);
                if (File.Exists(path))
                {
                    if (File.Exists(bak)) File.Delete(bak);
                    File.Move(path, bak);
                }
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmp, path);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Atomic write failed: {ex.Message}");
            }
        }

        public static Result<string> ReadAllTextSafe(string path)
        {
            try
            {
                if (!File.Exists(path)) return Result<string>.Fail("File not found.");
                return Result<string>.Success(File.ReadAllText(path, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Read failed: {ex.Message}");
            }
        }
    }
}
