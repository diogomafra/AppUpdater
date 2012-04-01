using System;
using System.IO;
using System.Net;
using System.Xml;
using AppUpdater.Manifest;

namespace AppUpdater.Server
{
    public class DefaultUpdateServer : IUpdateServer
    {
        private readonly Uri updateServerUrl;

        public DefaultUpdateServer(Uri updateServerUrl)
        {
            this.updateServerUrl = updateServerUrl;
        }

        public string GetCurrentVersion()
        {
            string xmlData = DownloadString("config.xml");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);
            return doc.SelectSingleNode("config/version").InnerText;
        }

        public VersionManifest GetManifest(string version)
        {
            string xmlData = DownloadString(GetVersionFilename(version, "manifest.xml"));
            return VersionManifest.LoadVersionData(version, xmlData);
        }

        public byte[] DownloadFile(string version, string filename)
        {
            return DownloadBinary(GetVersionFilename(version, filename));
        }

        private string DownloadString(string filename)
        {
            Uri versionUrl = new Uri(updateServerUrl, filename);
            WebClient client = new WebClient();
            return client.DownloadString(versionUrl);
        }

        private byte[] DownloadBinary(string filename)
        {
            Uri versionUrl = new Uri(updateServerUrl, filename);
            WebClient client = new WebClient();
            return client.DownloadData(versionUrl);
        }

        private string GetVersionFilename(string version, string filename)
        {
            return new Uri(updateServerUrl, Path.Combine(version, filename)).ToString();
        }
    }
}
