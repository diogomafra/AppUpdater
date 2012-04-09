using AppUpdater.LocalStructure;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Log;
using AppUpdater.Utils;
using System.IO;
using AppUpdater.Delta;

namespace AppUpdater.Chef
{
    public class UpdaterChef : IUpdaterChef
    {
        private readonly ILog log = Logger.For<UpdaterChef>();
        private readonly ILocalStructureManager localStructureManager;
        private readonly IUpdateServer updateServer;

        public UpdaterChef(ILocalStructureManager localStructureManager, IUpdateServer updateServer)
        {
            this.localStructureManager = localStructureManager;
            this.updateServer = updateServer;
        }

        public void Cook(UpdateRecipe recipe)
        {
            if (localStructureManager.HasVersionFolder(recipe.NewVersion))
            {
                localStructureManager.DeleteVersionDir(recipe.NewVersion);
            }

            localStructureManager.CreateVersionDir(recipe.NewVersion);

            foreach (var file in recipe.Files)
            {
                if (file.Action == FileUpdateAction.Copy)
                {
                    log.Debug("Copying file \"{0}\" from version \"{1}\".", file.Name, recipe.CurrentVersion);
                    localStructureManager.CopyFile(recipe.CurrentVersion, recipe.NewVersion, file.Name);
                }
                else if (file.Action == FileUpdateAction.Download)
                {
                    log.Debug("Downloading file \"{0}\".", file.FileToDownload);
                    byte[] data = updateServer.DownloadFile(recipe.NewVersion, file.FileToDownload);
                    log.Debug("Decompressing the file.");
                    data = DataCompressor.Decompress(data);
                    log.Debug("Saving the file \"{0}\".", file.Name);
                    localStructureManager.SaveFile(recipe.NewVersion, file.Name, data);
                }
                else if (file.Action == FileUpdateAction.DownloadDelta)
                {
                    log.Debug("Downloading patch file \"{0}\".", file.FileToDownload);
                    byte[] data = updateServer.DownloadFile(recipe.NewVersion, file.FileToDownload);
                    log.Debug("Applying patch file.");
                    localStructureManager.ApplyDelta(recipe.CurrentVersion, recipe.NewVersion, file.Name, data);
                }
            }
        }
    }
}
