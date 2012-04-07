using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AppUpdater.Recipe;
using AppUpdater.Utils;
using System.IO;
using AppUpdater.Delta;

namespace AppUpdater.Manifest
{
    public class VersionManifest
    {
        public string Version { get; private set; }
        public IEnumerable<VersionManifestFile> Files { get; private set; }

        public VersionManifest(string version, IEnumerable<VersionManifestFile> files)
        {
            this.Version = version;
            this.Files = files;
        }

        public static VersionManifest LoadVersionFile(string version, string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            return LoadData(version, doc);
        }

        public static VersionManifest LoadVersionData(string version, string data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);

            return LoadData(version, doc);
        }

        private static VersionManifest LoadData(string version, XmlDocument doc)
        {
            List<VersionManifestFile> files = new List<VersionManifestFile>();
            foreach (XmlNode fileNode in doc.SelectNodes("manifest/files/file"))
            {
                string filename = fileNode.Attributes["name"].Value;
                string checksum = fileNode.Attributes["checksum"].Value;
                long size = long.Parse(fileNode.Attributes["size"].Value);
                List<VersionManifestDeltaFile> deltas = new List<VersionManifestDeltaFile>();
                foreach (XmlNode deltaNode in fileNode.SelectNodes("delta"))
                {
                    string deltaFilename = deltaNode.Attributes["file"].Value;
                    string deltaChecksum = deltaNode.Attributes["from"].Value;
                    long deltaSize = long.Parse(deltaNode.Attributes["size"].Value);
                    deltas.Add(new VersionManifestDeltaFile(deltaFilename, deltaChecksum, deltaSize));
                }

                files.Add(new VersionManifestFile(filename, checksum, size, deltas));
            }

            return new VersionManifest(version, files);
        }

        public UpdateRecipe UpdateTo(VersionManifest newVersionManifest)
        {
            List<UpdateRecipeFile> recipeFiles = new List<UpdateRecipeFile>();
            foreach (var file in newVersionManifest.Files)
            {
                VersionManifestFile originalFile = this.Files.FirstOrDefault(x => x.Name.Equals(file.Name, StringComparison.CurrentCultureIgnoreCase));
                FileUpdateAction action = FileUpdateAction.Download;
                string fileToDownload = file.DeployedName;
                long size = file.Size;
                if (originalFile != null)
                {
                    if (originalFile.Checksum == file.Checksum)
                    {
                        action = FileUpdateAction.Copy;
                    }
                    else if (DeltaAPI.IsSupported())
                    {
                        VersionManifestDeltaFile delta = file.GetDeltaFrom(originalFile.Checksum);
                        if (delta != null)
                        {
                            action = FileUpdateAction.DownloadDelta;
                            fileToDownload = delta.Filename;
                            size = delta.Size;
                        }
                    }
                }

                recipeFiles.Add(new UpdateRecipeFile(file.Name, file.Checksum, size, action, fileToDownload));
            }

            return new UpdateRecipe(newVersionManifest.Version, this.Version, recipeFiles);
        }

        public static VersionManifest GenerateFromDirectory(string version, string directory)
        {
            directory = PathUtils.AddTrailingSlash(directory);

            List<VersionManifestFile> files = new List<VersionManifestFile>();
            foreach (string filename in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    string checksum = Checksum.Calculate(fs);
                    VersionManifestFile file = new VersionManifestFile(filename.Remove(0, directory.Length), checksum, fs.Length);
                    files.Add(file);
                }
            }

            return new VersionManifest(version, files);
        }

        public void SaveToFile(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter xml = XmlWriter.Create(filename, settings))
            {
                xml.WriteStartElement("manifest");
                xml.WriteStartElement("files");
                foreach (var file in Files)
                {
                    xml.WriteStartElement("file");
                    xml.WriteAttributeString("name", file.Name);
                    xml.WriteAttributeString("checksum", file.Checksum);
                    xml.WriteAttributeString("size", file.Size.ToString());
                    foreach (var delta in file.Deltas)
                    {
                        xml.WriteStartElement("delta");
                        xml.WriteAttributeString("from", delta.Checksum);
                        xml.WriteAttributeString("size", delta.Size.ToString());
                        xml.WriteAttributeString("file", delta.Filename);
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
                xml.WriteEndElement();
            }
        }
    }
}
