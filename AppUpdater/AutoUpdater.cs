using System;
using System.Threading;
using AppUpdater.Log;

namespace AppUpdater
{
    public class AutoUpdater
    {
        private ILog log = Logger.For<AutoUpdater>();
        private int secondsBetweenChecks;
        private readonly IUpdateManager updateManager;
        private Thread thread;

        public int SecondsBetweenChecks
        {
            get { return secondsBetweenChecks; }
            set { secondsBetweenChecks = value; }
        }

        public event EventHandler Updated;

        public AutoUpdater(IUpdateManager updateManager)
        {
            this.updateManager = updateManager;
            secondsBetweenChecks = 3600;
        }

        public void Start()
        {
            if (thread == null || !thread.IsAlive)
            {
                log.Debug("Starting the AutoUpdater.");
                thread = new Thread(CheckForUpdates);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                log.Debug("Stopping the AutoUpdater.");
                thread.Abort();
            }

            thread = null;
        }

        private void CheckForUpdates()
        {
            while (true)
            {
                try
                {
                    log.Debug("Checking for updates.");
                    UpdateInfo updateInfo;
                    if (updateManager.CheckForUpdate(out updateInfo))
                    {
                        log.Debug("Updates found. Installing new files.");
                        updateManager.DoUpdate(updateInfo);
                        log.Debug("Update is ready.");
                        RaiseUpdated();
                    }
                    else
                    {
                        log.Debug("No updates found.");
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception err)
                {
                    log.Error(err.Message);
                }

                Thread.Sleep(secondsBetweenChecks * 1000);
            }
        }

        private void RaiseUpdated()
        {
            EventHandler updated = Updated;
            if (updated != null)
            {
                updated(this, EventArgs.Empty);
            }
        }
    }
}
