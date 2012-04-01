using System.Linq;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using NUnit.Framework;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class VersionManifestTests
    {
        [Test]
        public void LoadVersionData_WithValidData_LoadsTheData()
        {
            string data = @"<manifest>
                                <files>
                                    <file name=""teste1.txt"" checksum=""algo111"" size=""1000"" />
                                    <file name=""teste2.txt"" checksum=""algo222"" size=""2000"" />
                                </files>
                            </manifest>";

            VersionManifest manifest = VersionManifest.LoadVersionData("1.2.3", data);

            Assert.That(manifest, Is.Not.Null);
            Assert.That(manifest.Version, Is.EqualTo("1.2.3"));
            Assert.That(manifest.Files, Has.Count.EqualTo(2));
            Assert.That(manifest.Files.ElementAt(0).Name, Is.EqualTo("teste1.txt"));
            Assert.That(manifest.Files.ElementAt(0).Checksum, Is.EqualTo("algo111"));
            Assert.That(manifest.Files.ElementAt(0).Size, Is.EqualTo(1000));
        }

        [Test]
        public void UpdateTo_ReturnsARecipe()
        {
            VersionManifestFile fileUpdate = new VersionManifestFile("arquivo1.txt", "123", 1000);
            VersionManifest currentManifest = new VersionManifest("1.0.0", new VersionManifestFile[] {  });
            VersionManifest updateManifest = new VersionManifest("2.0.0", new VersionManifestFile[] { fileUpdate });

            UpdateRecipe recipe = currentManifest.UpdateTo(updateManifest);

            Assert.That(recipe, Is.Not.Null);
            Assert.That(recipe.CurrentVersion, Is.EqualTo("1.0.0"));
            Assert.That(recipe.NewVersion, Is.EqualTo("2.0.0"));
            Assert.That(recipe.Files, Has.Count.EqualTo(1));
        }

        [Test]
        public void UpdateTo_VersionWithEqualFile_SetTheActionAsCopy()
        {
            VersionManifestFile fileUpdate = new VersionManifestFile("arquivo1.txt", "123", 1000);
            VersionManifest currentManifest = new VersionManifest("1.0.0", new VersionManifestFile[] { fileUpdate });
            VersionManifest updateManifest = new VersionManifest("2.0.0", new VersionManifestFile[] { fileUpdate });

            UpdateRecipe recipe = currentManifest.UpdateTo(updateManifest);

            Assert.That(recipe.Files, Has.Count.EqualTo(1));
            Assert.That(recipe.Files.First().Action, Is.EqualTo(FileUpdateAction.Copy));
        }

        [Test]
        public void UpdateTo_VersionWithoutTheFile_SetTheActionAsDownload()
        {
            VersionManifestFile fileUpdate = new VersionManifestFile("arquivo1.txt", "123", 1000);
            VersionManifest currentManifest = new VersionManifest("1.0.0", new VersionManifestFile[] {  });
            VersionManifest updateManifest = new VersionManifest("2.0.0", new VersionManifestFile[] { fileUpdate });

            UpdateRecipe recipe = currentManifest.UpdateTo(updateManifest);

            Assert.That(recipe.Files, Has.Count.EqualTo(1));
            Assert.That(recipe.Files.First().Action, Is.EqualTo(FileUpdateAction.Download));
        }

        [Test]
        public void UpdateTo_VersionWithTheFileWithIncorrectChecksum_SetTheActionAsDownload()
        {
            VersionManifestFile fileOriginal = new VersionManifestFile("arquivo1.txt", "333", 1000);
            VersionManifestFile fileUpdate = new VersionManifestFile("arquivo1.txt", "123", 1000);
            VersionManifest currentManifest = new VersionManifest("1.0.0", new VersionManifestFile[] { fileOriginal });
            VersionManifest updateManifest = new VersionManifest("2.0.0", new VersionManifestFile[] { fileUpdate });

            UpdateRecipe recipe = currentManifest.UpdateTo(updateManifest);

            Assert.That(recipe.Files, Has.Count.EqualTo(1));
            Assert.That(recipe.Files.First().Action, Is.EqualTo(FileUpdateAction.Download));
        }
    }
}
