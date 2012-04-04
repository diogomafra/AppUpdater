using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AppUpdater.Publisher;
using System.IO;
using AppUpdater.Manifest;
using System.Xml;

namespace AppUpdater.Tests.Publisher
{
    [TestFixture]
    public class AppPublisherTests
    {
        private AppPublisher appPublisher;
        private string sourceDir;
        private string destinationDir;

        [SetUp]
        public void Setup()
        {
            sourceDir = Path.GetTempFileName() + "_";
            destinationDir = Path.GetTempFileName() + "_";
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(destinationDir);
            CreateVersionFiles();

            appPublisher = new AppPublisher();
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(sourceDir, true);
            Directory.Delete(destinationDir, true);
        }

        [Test]
        public void Publish_CreatesADirectoryToTheNewVersion()
        {
            appPublisher.Publish(sourceDir, destinationDir, "1.0.0");

            bool directoryExists = Directory.Exists(Path.Combine(destinationDir, "1.0.0"));
            Assert.That(directoryExists, Is.True);
        }

        [Test]
        public void Publish_WithValidInfo_CopiesTheFilesToTargetDir()
        {
            appPublisher.Publish(sourceDir, destinationDir, "1.1.0");

            Assert.That(File.Exists(Path.Combine(destinationDir, "1.1.0\\test1.txt")), Is.True);
            Assert.That(File.Exists(Path.Combine(destinationDir, "1.1.0\\another\\test2.txt")), Is.True);
        }

        [Test]
        public void Publish_WithValidInfo_GeneratesTheManifest()
        {
            string manifestFilename = Path.Combine(destinationDir, "1.1.0\\manifest.xml");

            appPublisher.Publish(sourceDir, destinationDir, "1.1.0");

            Assert.That(File.Exists(manifestFilename), Is.True);
        }

        [Test]
        public void Publish_WithValidInfo_SetsTheManifestData()
        {
            string manifestFilename = Path.Combine(destinationDir, "1.1.0\\manifest.xml");

            appPublisher.Publish(sourceDir, destinationDir, "1.1.0");

            VersionManifest manifest = VersionManifest.LoadVersionData("1.1.0", File.ReadAllText(manifestFilename));
            Assert.That(manifest.Files.Count(), Is.EqualTo(2));
            Assert.That(manifest.Files.ElementAt(0).Name, Is.EqualTo("test1.txt"));
            Assert.That(manifest.Files.ElementAt(1).Name, Is.EqualTo("another\\test2.txt"));
        }

        [Test]
        public void Publish_ChangesTheCurrentVersion()
        {
            appPublisher.Publish(sourceDir, destinationDir, "1.1.0");

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(destinationDir, "config.xml"));
            string version = doc.SelectSingleNode("config/version").InnerText;
            Assert.That(version, Is.EqualTo("1.1.0"));
        }

        private void CreateVersionFiles()
        {
            File.WriteAllText(Path.Combine(sourceDir, "test1.txt"), "test1 content");
            Directory.CreateDirectory(Path.Combine(sourceDir, "another"));
            File.WriteAllText(Path.Combine(sourceDir, "another\\test2.txt"), "test2 content");
        }
    }
}
