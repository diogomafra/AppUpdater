using AppUpdater.LocalStructure;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Log;
using AppUpdater.Utils;

namespace AppUpdater.Chef
{
    public class UpdaterChef : IUpdaterChef
    {
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
                    localStructureManager.CopyFile(recipe.CurrentVersion, recipe.NewVersion, file.Name);
                }
                else if (file.Action == FileUpdateAction.Download)
                {
                    byte[] data = updateServer.DownloadFile(recipe.NewVersion, file.DeployedName);
                    data = DataCompressor.Decompress(data);
                    localStructureManager.SaveFile(recipe.NewVersion, file.Name, data);
                }
            }
        }
    }
}
