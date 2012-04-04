using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using AppUpdater.Manifest;
using AppUpdater.Utils;

namespace AppUpdater.LocalStructure
{
    public class DefaultLocalStructureManager : ILocalStructureManager
    {
        public string baseDir;

        public DefaultLocalStructureManager(string baseDir)
        {
            this.baseDir = baseDir;
        }

        public void CreateVersionDir(string version)
        {
            Directory.CreateDirectory(GetVersionDir(version));
        }

        public void DeleteVersionDir(string version)
        {
            Directory.Delete(GetVersionDir(version), true);
        }

        public VersionManifest LoadManifest(string version)
        {
            string versionDir = GetVersionDir(version);
            return VersionManifest.GenerateFromDirectory(version, versionDir);
        }

        public string GetCurrentVersion()
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);

            return doc.SelectSingleNode("config/version").InnerText;
        }

        public void SetCurrentVersion(string version)
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);
            doc.SelectSingleNode("config/version").InnerText = version;
            doc.Save(configFilename);
        }

        public bool HasVersionFolder(string version)
        {
            return Directory.Exists(GetVersionDir(version));
        }

        public void CopyFile(string originVersion, string destinationVersion, string filename)
        {
            string originFilename = Path.Combine(GetVersionDir(originVersion), filename);
            string destinationFilename = Path.Combine(GetVersionDir(destinationVersion), filename);

            File.Copy(originFilename, destinationFilename, true);
        }

        public void SaveFile(string version, string filename, byte[] data)
        {
            string destinationFilename = Path.Combine(GetVersionDir(version), filename);
            File.WriteAllBytes(destinationFilename, data);
        }

        public Uri GetUpdateServerUri()
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);

            return new Uri(doc.SelectSingleNode("config/updateServer").InnerText);
        }

        private string GetVersionDir(string version)
        {
            return Path.Combine(baseDir, version);
        }

        private string GetFilename(string version, string filename)
        {
            return Path.Combine(GetVersionDir(version), filename);
        }
    }
}
