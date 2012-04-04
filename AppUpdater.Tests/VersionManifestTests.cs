using System.Linq;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using NUnit.Framework;
using System.IO;

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

        [Test]
        public void GenerateFromDirectory_GeneratesTheManifest()
        {
            string dir = Path.GetTempFileName() + "_";
            Directory.CreateDirectory(dir);
            Directory.CreateDirectory(Path.Combine(dir, "abc"));
            File.WriteAllText(Path.Combine(dir, "test1.txt"), "some text");
            File.WriteAllText(Path.Combine(dir, "abc\\test2.txt"), "another text");

            VersionManifest manifest = VersionManifest.GenerateFromDirectory("1.0.0", dir);

            Assert.That(manifest, Is.Not.Null);
            Assert.That(manifest.Version, Is.EqualTo("1.0.0"));
            Assert.That(manifest.Files, Has.Count.EqualTo(2));
            Assert.That(manifest.Files.ElementAt(0).Name, Is.EqualTo("test1.txt"));
            Assert.That(manifest.Files.ElementAt(0).Checksum, Is.EqualTo("B94F6F125C79E3A5FFAA826F584C10D52ADA669E6762051B826B55776D05AED2"));
            Assert.That(manifest.Files.ElementAt(0).Size, Is.EqualTo(9));
            Assert.That(manifest.Files.ElementAt(1).Name, Is.EqualTo("abc\\test2.txt"));
            Assert.That(manifest.Files.ElementAt(1).Checksum, Is.EqualTo("4895ECC6F0C011072AF486EA30A1239CAA1B297FB61ECACA8AC94D9C2071BE22"));
            Assert.That(manifest.Files.ElementAt(1).Size, Is.EqualTo(12));
        }

        [Test]
        public void SaveToFile_CreatesTheFile()
        {
            string filename = Path.GetTempFileName();
            string data = @"<manifest></manifest>";

            VersionManifest manifest = VersionManifest.LoadVersionData("1.0.0", data);
            manifest.SaveToFile(filename);

            Assert.That(File.Exists(filename), Is.True);
        }

        [Test]
        public void SaveToFile_SavesAllTheInfoToTheFile()
        {
            string filename = Path.GetTempFileName();
            string data = @"<manifest>
                                <files>
                                    <file name=""test1.txt"" checksum=""algo111"" size=""1000"" />
                                    <file name=""test2.txt"" checksum=""algo222"" size=""2000"" />
                                </files>
                            </manifest>";

            VersionManifest manifest = VersionManifest.LoadVersionData("1.0.0", data);
            manifest.SaveToFile(filename);

            VersionManifest savedManifest = VersionManifest.LoadVersionData("1.0.0", File.ReadAllText(filename));
            Assert.That(manifest, Is.Not.Null);
            Assert.That(manifest.Files, Has.Count.EqualTo(2));
            Assert.That(manifest.Files.ElementAt(0).Name, Is.EqualTo("test1.txt"));
            Assert.That(manifest.Files.ElementAt(0).Checksum, Is.EqualTo("algo111"));
            Assert.That(manifest.Files.ElementAt(0).Size, Is.EqualTo(1000));
            Assert.That(manifest.Files.ElementAt(1).Name, Is.EqualTo("test2.txt"));
            Assert.That(manifest.Files.ElementAt(1).Checksum, Is.EqualTo("algo222"));
            Assert.That(manifest.Files.ElementAt(1).Size, Is.EqualTo(2000));
        }
    }
}
