using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using AppUpdater.LocalStructure;
using AppUpdater.Manifest;
using System.Xml;
using AppUpdater.Delta;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class DefaultLocalStructureManagerTests
    {
        private DefaultLocalStructureManager structureManager;
        private string baseDir;

        [SetUp]
        public void Setup()
        {
            baseDir = Path.GetTempFileName() + "_";
            Directory.CreateDirectory(baseDir);
            structureManager = new DefaultLocalStructureManager(baseDir);
        }

        [Test]
        public void CreateVersionDir_CreatesADirWithTheVersionName()
        {
            structureManager.CreateVersionDir("1.0.0");

            bool exists = Directory.Exists(Path.Combine(baseDir, "1.0.0"));

            Assert.That(exists, Is.True);
        }

        [Test]
        public void GetInstalledVersions_ReturnsAllInstalledVersions()
        {
            string[] expectedVersions = {"1.0.0", "2.0.0", "3.1.1"};
            structureManager.CreateVersionDir("1.0.0");
            structureManager.CreateVersionDir("2.0.0");
            structureManager.CreateVersionDir("3.1.1");

            string[] versions = structureManager.GetInstalledVersions();

            Assert.That(versions, Is.EqualTo(expectedVersions));
        }

        [Test]
        public void DeleteVersionDir_DeletesTheDirectory()
        {
            string dir = Path.Combine(baseDir, "1.0.0");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "a.txt"), "test");

            structureManager.DeleteVersionDir("1.0.0");

            bool exists = Directory.Exists(Path.Combine(baseDir, "1.0.0"));
            Assert.That(exists, Is.False);
        }

        [Test]
        public void LoadManifest_GeneratesTheManifest()
        {
            string dir = Path.Combine(baseDir, "1.0.0");
            Directory.CreateDirectory(dir);
            Directory.CreateDirectory(Path.Combine(dir, "abc")); 
            File.WriteAllText(Path.Combine(dir, "test1.txt"), "some text");
            File.WriteAllText(Path.Combine(dir, "abc\\test2.txt"), "another text");

            VersionManifest manifest = structureManager.LoadManifest("1.0.0");

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
        public void GetCurrentVersion_ReturnsTheVersion()
        {
            string data = @"<config><version>1.2.3</version></config>";
            File.WriteAllText(Path.Combine(baseDir, "config.xml"), data);

            string currentVersion = structureManager.GetCurrentVersion();

            Assert.That(currentVersion, Is.EqualTo("1.2.3"));
        }

        [Test]
        public void SetCurrentVersion_UpdatesTheConfigFile()
        {
            string data = @"<config><version>1.2.3</version></config>";
            string configFilename = Path.Combine(baseDir, "config.xml");
            File.WriteAllText(configFilename, data);

            structureManager.SetCurrentVersion("3.4.5");

            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);
            string version = doc.SelectSingleNode("config/version").InnerText;
            Assert.That(version, Is.EqualTo("3.4.5"));
        }

        [Test]
        public void GetExecutingVersion_ReturnsTheVersionThatIsBeingExecuted()
        {
            DefaultLocalStructureManager.GetExecutablePath = () => @"C:\Test\AppRoot\1.4.5\app.exe";

            string executingVersion = structureManager.GetExecutingVersion();

            Assert.That(executingVersion, Is.EqualTo("1.4.5"));
        }

        [Test]
        public void HasVersionFolder_WithTheFolder_ReturnsTrue()
        {
            Directory.CreateDirectory(Path.Combine(baseDir, "4.5.6"));

            bool hasFolder = structureManager.HasVersionFolder("4.5.6");

            Assert.That(hasFolder, Is.True);
        }

        [Test]
        public void HasVersionFolder_WithoutTheFolder_ReturnsFalse()
        {
            bool hasFolder = structureManager.HasVersionFolder("9.9.9");

            Assert.That(hasFolder, Is.False);
        }

        [Test]
        public void CopyFile_CopyTheFileFromTheOriginalVersion()
        {
            Directory.CreateDirectory(Path.Combine(baseDir, "1.2.3"));
            Directory.CreateDirectory(Path.Combine(baseDir, "4.5.6"));
            File.WriteAllText(Path.Combine(baseDir, "1.2.3\\test.txt"), "some value");

            structureManager.CopyFile("1.2.3", "4.5.6", "test.txt");

            string destinationFile = Path.Combine(baseDir, "4.5.6\\test.txt");
            Assert.That(File.Exists(destinationFile), Is.True);
            Assert.That(File.ReadAllText(destinationFile), Is.EqualTo("some value"));
        }

        [Test]
        public void SaveFile_SavesTheFileInTheVersionDirectory()
        {
            byte[] data = new byte[] { 4, 5, 6 };
            Directory.CreateDirectory(Path.Combine(baseDir, "1.2.3"));

            structureManager.SaveFile("1.2.3", "test.txt", data);

            string destinationFile = Path.Combine(baseDir, "1.2.3\\test.txt");
            Assert.That(File.Exists(destinationFile), Is.True);
            Assert.That(File.ReadAllBytes(destinationFile), Is.EqualTo(data));
        }

        [Test]
        public void ApplyDelta_SavesThePatchedFile()
        {
            Directory.CreateDirectory(Path.Combine(baseDir, "1.2.3"));
            Directory.CreateDirectory(Path.Combine(baseDir, "2.0.0"));
            byte[] originalData = new byte[] { 4, 5, 6, 5, 4 };
            byte[] newData = new byte[] { 4, 5, 6, 5, 4 };
            string originalFile = Path.Combine(baseDir, "1.2.3\\test1.dat");
            string newFile = Path.GetTempFileName();
            string deltaFile = Path.GetTempFileName();
            string patchedFile = Path.GetTempFileName();
            File.WriteAllBytes(originalFile, originalData);
            File.WriteAllBytes(newFile, newData);
            DeltaAPI.CreateDelta(originalFile, newFile, deltaFile, true);
            byte[] deltaData = File.ReadAllBytes(deltaFile);

            structureManager.ApplyDelta("1.2.3", "2.0.0", "test1.dat", deltaData);

            Assert.That(File.Exists(Path.Combine(baseDir, "2.0.0\\test1.dat")), Is.True);
            byte[] patchedData = File.ReadAllBytes(Path.Combine(baseDir, "2.0.0\\test1.dat"));
            Assert.That(patchedData, Is.EqualTo(newData));
        }

        [Test]
        public void GetUpdateServerUri_ReturnsTheUri()
        {
            string data = @"<config><version>1.2.3</version><updateServer>http://localhost:8080/update/</updateServer></config>";
            string configFilename = Path.Combine(baseDir, "config.xml");
            File.WriteAllText(configFilename, data);
            
            Uri uri = structureManager.GetUpdateServerUri();

            Assert.That(uri.ToString(), Is.EqualTo("http://localhost:8080/update/"));
        }
    }
}
