using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using AppUpdater.Delta;
using AppUpdater.Manifest;
using AppUpdater.Utils;

namespace AppUpdater.LocalStructure
{
    public class DefaultLocalStructureManager : ILocalStructureManager
    {
        public string baseDir;
        public static Func<string> GetExecutablePath = GetExecutingAssemblyLocation;

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

        public string[] GetInstalledVersions()
        {
            string baseDirectory = PathUtils.AddTrailingSlash(baseDir);

            return Directory.EnumerateDirectories(baseDirectory)
                            .Select(x => x.Remove(0, baseDirectory.Length))
                            .ToArray();
        }

        public VersionManifest LoadManifest(string version)
        {
            string versionDir = GetVersionDir(version);
            return VersionManifest.GenerateFromDirectory(version, versionDir);
        }

        public string GetCurrentVersion()
        {
            return GetConfigValue("version");
        }

        public void SetCurrentVersion(string version)
        {
            SetConfigValue("version", version);
        }

        public string GetLastValidVersion()
        {
            return GetConfigValue("last_version");
        }

        public void SetLastValidVersion(string version)
        {
            SetConfigValue("last_version", version);
        }

        public string GetExecutingVersion()
        {
            return Directory.GetParent(GetExecutablePath()).Name;
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

        public void ApplyDelta(string originalVersion, string newVersion, string filename, byte[] deltaData)
        {
            string originalFile = GetFilename(originalVersion, filename);
            string newFile = GetFilename(newVersion, filename);
            string deltaFile = Path.GetTempFileName();
            File.WriteAllBytes(deltaFile, deltaData);

            DeltaAPI.ApplyDelta(originalFile, newFile, deltaFile);
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

        private static string GetExecutingAssemblyLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        private string GetConfigValue(string name)
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);

            XmlNode configValue = doc.SelectSingleNode("config/" + name);
            return configValue == null ? string.Empty : configValue.InnerText;
        }

        private void SetConfigValue(string name, string value)
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);
            XmlNode lastVersionNode = doc.SelectSingleNode("config/" + name);
            if (lastVersionNode == null)
            {
                lastVersionNode = doc.CreateElement(name);
                doc.SelectSingleNode("config").AppendChild(lastVersionNode);
            }

            lastVersionNode.InnerText = value;
            doc.Save(configFilename);
        }
    }
}
