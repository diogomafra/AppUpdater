using AppUpdater.LocalStructure;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using NUnit.Framework;
using Rhino.Mocks;
using System;

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
            private string[] installedVersions;

            [SetUp]
            public void Setup()
            {
                updateServer = MockRepository.GenerateStub<IUpdateServer>();
                localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
                updaterChef = MockRepository.GenerateStub<IUpdaterChef>();
                updateManager = new UpdateManager(updateServer, localStructureManager, updaterChef);

                initialVersion = "1.2.3";
                installedVersions = new string[] { "1.0.0", "1.1.1", "1.2.3" };
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return(initialVersion);
                localStructureManager.Stub(x => x.GetExecutingVersion()).Return(initialVersion);
                localStructureManager.Stub(x => x.GetInstalledVersions()).Do(new Func<string[]>(()=>installedVersions));
                updateManager.Initialize();
            }


            [Test]
            public void CheckForUpdate_WithoutUpdate_HasUpdateIsFalse()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return(initialVersion);

                UpdateInfo updateInfo = updateManager.CheckForUpdate();

                Assert.That(updateInfo.HasUpdate, Is.False);
            }

            [Test]
            public void CheckForUpdate_WithoutUpdate_ReturnsTheCurrentVersion()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return(initialVersion);

                UpdateInfo updateInfo = updateManager.CheckForUpdate();

                Assert.That(updateInfo.Version, Is.EqualTo(initialVersion));
            }

            [Test]
            public void CheckForUpdate_WithUpdate_HasUpdateIsTrue()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return("2.6.8");

                UpdateInfo updateInfo = updateManager.CheckForUpdate();

                Assert.That(updateInfo.HasUpdate, Is.True);
            }

            [Test]
            public void CheckForUpdate_WithUpdate_ReturnsTheNewVersionNumber()
            {
                string newVersion = "2.6.8";
                updateServer.Stub(x => x.GetCurrentVersion()).Return(newVersion);

                UpdateInfo updateInfo = updateManager.CheckForUpdate();

                Assert.That(updateInfo.Version, Is.EqualTo(newVersion));
            }

            [Test]
            public void DoUpdate_ChangesTheCurrentVersion()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                Assert.That(updateManager.CurrentVersion, Is.EqualTo(newVersion));
            }

            [Test]
            public void DoUpdate_SavesTheCurrentVersion()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                localStructureManager.AssertWasCalled(x => x.SetCurrentVersion(newVersion));
            }

            [Test]
            public void DoUpdate_SavesTheLastValidVersionAsTheExecutingBeingExecuted()
            {
                string versionBeingExecuted = initialVersion;
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return("2.0.0");
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                localStructureManager.AssertWasCalled(x => x.SetLastValidVersion(versionBeingExecuted));
            }

            [Test]
            public void DoUpdate_ExecutesTheUpdate()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));

                updateManager.DoUpdate(updateInfo);

                updaterChef.AssertWasCalled(x => x.Cook(Arg<UpdateRecipe>.Is.Anything));
            }

            [Test]
            public void DoUpdate_RemovesOldVersions()
            {
                var updateInfo = SetupUpdateToVersion("3.1");

                updateManager.DoUpdate(updateInfo);

                localStructureManager.AssertWasCalled(x => x.DeleteVersionDir("1.0.0"));
                localStructureManager.AssertWasCalled(x => x.DeleteVersionDir("1.1.1"));
            }

            [Test]
            public void DoUpdate_DoesNotRemoveTheExecutingVersion()
            {
                var updateInfo = SetupUpdateToVersion("3.1");

                updateManager.DoUpdate(updateInfo);

                localStructureManager.AssertWasNotCalled(x => x.DeleteVersionDir(initialVersion));
            }

            [Test]
            public void DoUpdate_DoesNotRemoveTheNewVersion()
            {
                installedVersions = new string[] { "1.0.0", "1.1.1", "1.2.3", "3.1"};
                var updateInfo = SetupUpdateToVersion("3.1");

                updateManager.DoUpdate(updateInfo);

                localStructureManager.AssertWasNotCalled(x => x.DeleteVersionDir("3.1"));
            }

            [Test]
            public void DoUpdate_WithAnErrorWhileDeletingTheOldVersion_IgnoresTheError()
            {
                localStructureManager.Stub(x => x.DeleteVersionDir("1.0.0")).Throw(new Exception("Error deliting version."));
                var updateInfo = SetupUpdateToVersion("3.1");

                updateManager.DoUpdate(updateInfo);
            }

            private UpdateInfo SetupUpdateToVersion(string newVersion)
            {
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(new VersionManifest(newVersion, new VersionManifestFile[0]));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));
                return updateInfo;
            }
        }
    }
}
