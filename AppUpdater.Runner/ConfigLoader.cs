using System;
using System.IO;
using System.Xml;

namespace AppUpdater
{
    public class ConfigInfo
    {
        public string Version { get; set; }
        public string LastVersion { get; set; }
        public string Executable { get; set; }
        public string BaseDirectory { get; set; }
    }

    public static class ConfigLoader
    {
        public static ConfigInfo LoadConfig(string baseDirectory)
        {
            // Ensure we have the full path
            baseDirectory = Path.GetFullPath(baseDirectory);

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(baseDirectory, "config.xml"));

            var info = new ConfigInfo
            {
                Version = GetConfigValue(doc, "version"),
                LastVersion = GetConfigValue(doc, "last_version"),
                Executable = GetConfigValue(doc, "executable"),
                BaseDirectory = GetConfigValue(doc, "base_directory"),
            };

            if (!String.IsNullOrEmpty(info.BaseDirectory))
            {
                // Ensure we have the full path
                info.BaseDirectory = Path.GetFullPath(info.BaseDirectory);
                
                // If the base directory is defined, and is not the current one, load the config in that directory.
                if (!info.BaseDirectory.Equals(baseDirectory, StringComparison.CurrentCultureIgnoreCase))
                {
                    return LoadConfig(info.BaseDirectory);
                }
            }

            // The base directory was not defined or is the current one
            info.BaseDirectory = baseDirectory;

            return info;
        }

        private static string GetConfigValue(XmlDocument doc, string name)
        {
            var node = doc.SelectSingleNode("config/" + name);
            return node == null ? null : node.InnerText;
        }
    }
}
