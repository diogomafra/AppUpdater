using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AppUpdater.Utils;
using AppUpdater.Manifest;
using System.Xml;

namespace AppUpdater.Publisher
{
    public class AppPublisher
    {
        public void Publish(string sourceDirectory, string destionationDirectory, string version)
        {
            sourceDirectory = PathUtils.AddTrailingSlash(sourceDirectory);
            destionationDirectory = PathUtils.AddTrailingSlash(destionationDirectory);
            string destinationVersionDirectory = PathUtils.AddTrailingSlash(Path.Combine(destionationDirectory, version));

            CopyFiles(sourceDirectory, destinationVersionDirectory);
            SaveManifest(sourceDirectory, version, destinationVersionDirectory);
            SaveConfigFile(destionationDirectory, version);
        }

        private static void CopyFiles(string sourceDirectory, string destinationVersionDirectory)
        {
            Directory.CreateDirectory(destinationVersionDirectory);

            foreach (string sourceFile in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string sourceFileRelativePath = sourceFile.Remove(0, sourceDirectory.Length);
                string destinationFile = Path.Combine(destinationVersionDirectory, sourceFileRelativePath + ".deploy");

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                CreateDeployFile(sourceFile, destinationFile);
            }
        }

        private static void SaveManifest(string sourceDirectory, string version, string destinationVersionDirectory)
        {
            VersionManifest manifest = VersionManifest.GenerateFromDirectory(version, sourceDirectory);
            manifest.SaveToFile(Path.Combine(destinationVersionDirectory, "manifest.xml"));
        }

        private static void SaveConfigFile(string destionationDirectory, string version)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<config><version></version></config>");
            doc.SelectSingleNode("config/version").InnerText = version;
            doc.Save(Path.Combine(destionationDirectory, "config.xml"));
        }

        private static void CreateDeployFile(string sourceFile, string destinationFile)
        {
            using (FileStream streamSource = File.OpenRead(sourceFile))
            {
                using (FileStream streamDestination = File.OpenWrite(destinationFile))
                {
                    DataCompressor.Compress(streamSource, streamDestination);
                }
            }
        }
    }
}
