using System.IO;
using AppUpdater.LocalStructure;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using AppUpdater.Log;
using System.Linq;
using System;

namespace AppUpdater
{
    public class UpdateManager : IUpdateManager
    {
        private ILog log = Logger.For<UpdateManager>();
        private readonly IUpdateServer updateServer;
        private readonly ILocalStructureManager localStructureManager;
        private readonly IUpdaterChef updaterChef;

        public string CurrentVersion { get; private set; }

        private static UpdateManager defaultInstance;

        public static UpdateManager Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    string baseDir = Path.Combine(Path.GetDirectoryName(typeof(UpdateManager).Assembly.Location), "..\\");
                    ILocalStructureManager manager = new DefaultLocalStructureManager(baseDir);
                    IUpdateServer updateServer = new DefaultUpdateServer(manager.GetUpdateServerUri());
                    defaultInstance = new UpdateManager(updateServer, manager, new UpdaterChef(manager, updateServer));
                    defaultInstance.Initialize();
                }

                return defaultInstance;
            }
        }

        public UpdateManager(IUpdateServer updateServer, ILocalStructureManager localStructureManager, IUpdaterChef updaterChef)
        {
            this.updateServer = updateServer;
            this.localStructureManager = localStructureManager;
            this.updaterChef = updaterChef;
        }

        public void Initialize()
        {
            this.CurrentVersion = localStructureManager.GetCurrentVersion();
        }

        public UpdateInfo CheckForUpdate()
        {
            string serverCurrentVersion = updateServer.GetCurrentVersion();
            bool hasUpdate = CurrentVersion != serverCurrentVersion;
            return new UpdateInfo(hasUpdate, serverCurrentVersion);
        }

        public void DoUpdate(UpdateInfo updateInfo)
        {
            VersionManifest currentVersionManifest = localStructureManager.LoadManifest(this.CurrentVersion);
            VersionManifest newVersionManifest = updateServer.GetManifest(updateInfo.Version);
            UpdateRecipe recipe = currentVersionManifest.UpdateTo(newVersionManifest);

            updaterChef.Cook(recipe);

            localStructureManager.SetLastValidVersion(localStructureManager.GetExecutingVersion());
            localStructureManager.SetCurrentVersion(updateInfo.Version);
            CurrentVersion = updateInfo.Version;

            DeleteOldVersions();
        }

        private void DeleteOldVersions()
        {
            string executingVersion = localStructureManager.GetExecutingVersion();
            string[] installedVersions = localStructureManager.GetInstalledVersions();
            string[] versionsInUse = new string[] { executingVersion, CurrentVersion };

            foreach (var version in installedVersions.Except(versionsInUse))
            {
                try
                {
                    //localStructureManager.DeleteVersionDir(version);
                }
                catch (Exception err)
                {
                    log.Error("Error deleting old version ({0}). {1}", version, err.Message);
                }
            }
        }
    }
}
