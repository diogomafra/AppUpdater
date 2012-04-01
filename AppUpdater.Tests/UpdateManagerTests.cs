using AppUpdater.LocalStructure;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using NUnit.Framework;
using Rhino.Mocks;

namespace AppUpdater.Tests
{
    public class UpdateManagerTests
    {
        [TestFixture]
        public class NotInitialized
        {
            private UpdateManager updateManager;
            private IUpdateServer updateServer;
            private ILocalStructureManager localStructureManager;

            [SetUp]
            public void Setup()
            {
                updateServer = MockRepository.GenerateStub<IUpdateServer>();
                localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
                var updaterChef = MockRepository.GenerateStub<IUpdaterChef>();
                updateManager = new UpdateManager(updateServer, localStructureManager, updaterChef);
            }

            [Test]
            public void Initialize_LoadsTheCurrentVersion()
            {
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return("1.3.4");

                updateManager.Initialize();

                Assert.That(updateManager.CurrentVersion, Is.EqualTo("1.3.4"));
            }
        }

        [TestFixture]
        public class Initialized
        {
            private UpdateManager updateManager;
            private IUpdateServer updateServer;
            private ILocalStructureManager localStructureManager;
            private IUpdaterChef updaterChef;
            private string initialVersion;

            [SetUp]
            public void Setup()
            {
                updateServer = MockRepository.GenerateStub<IUpdateServer>();
                localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
                updaterChef = MockRepository.GenerateStub<IUpdaterChef>();
                updateManager = new UpdateManager(updateServer, localStructureManager, updaterChef);

                initialVersion = "1.2.3";
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return(initialVersion);
                updateManager.Initialize();
            }

            [Test]
            public void CheckForUpdate_WithoutUpdate_ReturnsFalse()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return(initialVersion);

                UpdateInfo updateInfo;
                bool hasUpdate = updateManager.CheckForUpdate(out updateInfo);

                Assert.That(hasUpdate, Is.False);
            }

            [Test]
            public void CheckForUpdate_WithUpdate_ReturnsTrue()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return("2.6.8");

                UpdateInfo updateInfo;
                bool hasUpdate = updateManager.CheckForUpdate(out updateInfo);

                Assert.That(hasUpdate, Is.True);
            }

            [Test]
            public void CheckForUpdate_WithUpdate_ReturnsTheUpdateInfo()
            {
                string newVersion = "2.6.8";
                updateServer.Stub(x => x.GetCurrentVersion()).Return(newVersion);

                UpdateInfo updateInfo;
                bool hasUpdate = updateManager.CheckForUpdate(out updateInfo);

                Assert.That(updateInfo, Is.Not.Null);
                Assert.That(updateInfo.Version, Is.EqualTo(newVersion));
            }

            [Test]
            public void DoUpdate_ChangesTheCurrentVersion()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                Assert.That(updateManager.CurrentVersion, Is.EqualTo(newVersion));
            }

            [Test]
            public void DoUpdate_SavesTheCurrentVersion()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                localStructureManager.AssertWasCalled(x => x.SetCurrentVersion(newVersion));
            }

            [Test]
            public void DoUpdate_ExecutesTheUpdate()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                updaterChef.AssertWasCalled(x => x.Cook(Arg<UpdateRecipe>.Is.Anything));
            }
        }
    }
}
