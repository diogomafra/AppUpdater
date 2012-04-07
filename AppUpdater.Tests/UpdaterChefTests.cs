using AppUpdater.LocalStructure;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using NUnit.Framework;
using Rhino.Mocks;
using AppUpdater.Utils;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class UpdaterChefTests
    {
        private UpdaterChef updaterChef;
        private ILocalStructureManager localStructureManager;
        private IUpdateServer updateServer;

        [SetUp]
        public void Setup()
        {
            localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
            updateServer = MockRepository.GenerateStub<IUpdateServer>();
            updaterChef = new UpdaterChef(localStructureManager, updateServer);
        }

        [Test]
        public void Cook_WithAVersionAlreadyDownloaded_CreatesTheVersionDirectory()
        {
            localStructureManager.Stub(x => x.HasVersionFolder("2.0.0")).Return(true);
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[0]);
            updaterChef.Cook(updateRecipe);

            localStructureManager.AssertWasCalled(x => x.DeleteVersionDir("2.0.0"));
        }

        [Test]
        public void Cook_CreatesTheVersionDirectory()
        {
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[0]);
            updaterChef.Cook(updateRecipe);

            localStructureManager.AssertWasCalled(x => x.CreateVersionDir("2.0.0"));
        }

        [Test]
        public void Cook_CopyExistingFiles()
        {
            UpdateRecipeFile file1 = new UpdateRecipeFile("test1.txt", "123", 100, FileUpdateAction.Copy, null);
            UpdateRecipeFile file2 = new UpdateRecipeFile("test2.txt", "123", 100, FileUpdateAction.Download, "test2.txt.deploy");
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[] { file1, file2 });
            updaterChef.Cook(updateRecipe);

            localStructureManager.AssertWasCalled(x => x.CopyFile("1.0.0", "2.0.0", "test1.txt"));
        }

        [Test]
        public void Cook_SavesNewFiles()
        {
            byte[] data = new byte[]{1,2,3,4,5};
            updateServer.Stub(x => x.DownloadFile("2.0.0", "test2.txt.deploy")).Return(DataCompressor.Compress(data));
            UpdateRecipeFile file1 = new UpdateRecipeFile("test1.txt", "123", 100, FileUpdateAction.Copy, null);
            UpdateRecipeFile file2 = new UpdateRecipeFile("test2.txt", "123", 100, FileUpdateAction.Download, "test2.txt.deploy");
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[] { file1, file2 });

            updaterChef.Cook(updateRecipe);

            localStructureManager.AssertWasCalled(x => x.SaveFile("2.0.0", "test2.txt", data));
        }

        [Test]
        public void Cook_ApplyDeltaInModifiedFiles()
        {
            byte[] data = new byte[] { 1, 2, 3, 4, 5 };
            updateServer.Stub(x => x.DownloadFile("2.0.0", "test2.txt.deploy")).Return(data);
            UpdateRecipeFile file1 = new UpdateRecipeFile("test1.txt", "123", 100, FileUpdateAction.Copy, null);
            UpdateRecipeFile file2 = new UpdateRecipeFile("test2.txt", "123", 100, FileUpdateAction.DownloadDelta, "test2.txt.deploy");
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[] { file1, file2 });

            updaterChef.Cook(updateRecipe);

            localStructureManager.AssertWasCalled(x => x.ApplyDelta("1.0.0", "2.0.0", "test2.txt", data));
        }
    }
}
