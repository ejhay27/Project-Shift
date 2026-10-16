using System.IO;
using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for INV-004 (file IO in temp dir).</summary>
    public sealed class VersionedSaveTests
    {
        private string _path;

        [SetUp]
        public void SetUp()
        {
            _path = Path.Combine(Path.GetTempPath(),
                "pshift_test_" + Path.GetRandomFileName() + ".json");
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var p in new[] { _path, _path + ".tmp", _path + ".bak" })
                if (File.Exists(p)) File.Delete(p);
        }

        [Test]
        public void AtomicWrite_CreatesReadableFile()
        {
            var r = VersionedSave.AtomicWrite(_path, "{\"a\":1}");
            Assert.IsTrue(r.Ok);
            Assert.AreEqual("{\"a\":1}",
                VersionedSave.ReadAllTextSafe(_path).Value);
        }

        [Test]
        public void AtomicWrite_RotatesBackup()
        {
            VersionedSave.AtomicWrite(_path, "v1");
            VersionedSave.AtomicWrite(_path, "v2");
            Assert.AreEqual("v2", File.ReadAllText(_path));
            Assert.AreEqual("v1", File.ReadAllText(_path + ".bak"));
        }

        [Test]
        public void Validate_RejectsTamperedPayload()
        {
            var env = VersionedSave.Create("0.1.0-m1", "{\"a\":1}", 123);
            Assert.IsTrue(VersionedSave.Validate(env, SaveConstants.SchemaVersion).Ok);
            env.PayloadJson = "{\"a\":2}"; // tamper
            Assert.IsFalse(VersionedSave.Validate(env, SaveConstants.SchemaVersion).Ok);
        }

        [Test]
        public void Validate_RejectsSchemaMismatch()
        {
            var env = VersionedSave.Create("0.1.0-m1", "{}", 1);
            Assert.IsFalse(VersionedSave.Validate(env, 999).Ok);
        }
    }
}
