using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Threading;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class AutoUpdaterTests
    {
        private AutoUpdater autoUpdater;
        private IUpdateManager updateManager;

        [SetUp]
        public void Setup()
        {
            updateManager = MockRepository.GenerateStub<IUpdateManager>();
            autoUpdater = new AutoUpdater(updateManager);
        }

        [Test]
        public void Ctor_SetsTheDefaultCheckIntervalTo1hour()
        {
            Assert.That(autoUpdater.SecondsBetweenChecks, Is.EqualTo(3600));
        }

        [Test]
        public void Start_CheckForUpdatesOnStart()
        {
            autoUpdater.SecondsBetweenChecks = 10000;

            autoUpdater.Start();
            Thread.Sleep(1000);

            updateManager.AssertWasCalled(x => x.CheckForUpdate());
        }

        [Test]
        public void Start_DoNotCheckBeforeTime()
        {
            autoUpdater.SecondsBetweenChecks = 2;

            autoUpdater.Start();
            Thread.Sleep(1000);

            updateManager.AssertWasCalled(x => x.CheckForUpdate(), s=>s.Repeat.Once());
        }

        [Test]
        public void Start_StoppedUpdater_CheckAfterWaitTime()
        {
            autoUpdater.SecondsBetweenChecks = 1;

            autoUpdater.Start();
            Thread.Sleep(1500);

            updateManager.AssertWasCalled(x => x.CheckForUpdate(), s => s.Repeat.Twice());
        }

        [Test]
        public void Start_StartedUpdater_DoNotStartAgain()
        {
            autoUpdater.SecondsBetweenChecks = 1;

            autoUpdater.Start();
            Thread.Sleep(100);
            autoUpdater.Start();
            Thread.Sleep(100);

            updateManager.AssertWasCalled(x => x.CheckForUpdate(), s => s.Repeat.Once());
        }

        [Test]
        public void Stop_StartedUpdater_StopsTheChecks()
        {
            autoUpdater.SecondsBetweenChecks = 1;

            autoUpdater.Start();
            Thread.Sleep(300);
            autoUpdater.Stop();
            Thread.Sleep(1500);

            updateManager.AssertWasCalled(x => x.CheckForUpdate(), s => s.Repeat.Once());
        }

        [Test]
        public void Stop_StoppedUpdater_DoNothing()
        {
            autoUpdater.SecondsBetweenChecks = 1;

            autoUpdater.Stop();
        }

        [Test]
        public void Updated_IsCalledAfterUpdate()
        {
            bool called = false;
            UpdateInfo info = new UpdateInfo(true, "2.0.0");
            updateManager.Stub(x => x.CheckForUpdate()).Return(info);
            autoUpdater.Updated += (sender, e) => called = true;

            autoUpdater.Start();
            Thread.Sleep(100);

            Assert.That(called, Is.True);
        }
    }
}
