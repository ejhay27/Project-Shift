using System.IO;
using ProjectShift.Core;
using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// File-level save/load (INV-004): versioned envelope + checksum, atomic
    /// write with backup rotation, corrupt-main → backup fallback. Never
    /// silently replaces a valid save with a corrupt state (§36).
    /// </summary>
    public sealed class SaveManager : MonoBehaviour
    {
        [SerializeField] private string _gameVersion = "0.1.0-m1";
        [SerializeField] private int _slot = 1;
        [SerializeField] private WorldState _world;

        public string SavePath =>
            Path.Combine(Application.persistentDataPath, "ProjectShift", $"save_slot{_slot}.json");

        public void Bind(WorldState world) => _world = world;

        public Result Save()
        {
            if (_world == null) return Result.Fail("No world bound.");
            string payload = JsonUtility.ToJson(_world.ToSaveData(_gameVersion));
            var envelope = VersionedSave.Create(_gameVersion, payload, DateTimeProxy.UtcNowTicks);
            string fileContent = JsonUtility.ToJson(envelope);
            var write = VersionedSave.AtomicWrite(SavePath, fileContent);
            if (write.Ok) _world.Hud?.ShowToast("Progress saved.");
            else _world.Hud?.ShowToast("Save failed: " + write.Error);
            return write;
        }

        public Result Load(int inventoryCapacity)
        {
            if (_world == null) return Result.Fail("No world bound.");
            // Try main, then backup. Never auto-overwrite on failure.
            string[] candidates = { SavePath, SavePath + ".bak" };
            foreach (string path in candidates)
            {
                var read = VersionedSave.ReadAllTextSafe(path);
                if (!read.Ok) continue;
                SaveEnvelope envelope = null;
                try { envelope = JsonUtility.FromJson<SaveEnvelope>(read.Value); }
                catch { envelope = null; }
                var valid = VersionedSave.Validate(envelope, SaveConstants.SchemaVersion);
                if (!valid.Ok) continue;
                PlayerSaveData data = null;
                try { data = JsonUtility.FromJson<PlayerSaveData>(envelope.PayloadJson); }
                catch { data = null; }
                if (data == null) continue;
                _world.LoadSaveData(data, inventoryCapacity);
                _world.Hud?.ShowToast(path.EndsWith(".bak")
                    ? "Main save corrupt — restored backup."
                    : "Progress loaded.");
                return Result.Success();
            }
            return Result.Fail("No valid save found (main + backup checked).");
        }

        public bool HasSave() => File.Exists(SavePath) || File.Exists(SavePath + ".bak");
    }
}
