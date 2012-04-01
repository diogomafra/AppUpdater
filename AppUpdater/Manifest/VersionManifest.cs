using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AppUpdater.Recipe;

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

        public static VersionManifest LoadVersionData(string version, string data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);

            List<VersionManifestFile> files = new List<VersionManifestFile>();
            foreach (XmlNode fileNode in doc.SelectNodes("manifest/files/file"))
            {
                string filename = fileNode.Attributes["name"].Value;
                string checksum = fileNode.Attributes["checksum"].Value;
                long size = long.Parse(fileNode.Attributes["size"].Value);

                files.Add(new VersionManifestFile(filename, checksum, size));
            }

            return new VersionManifest(version, files);
        }

        public UpdateRecipe UpdateTo(VersionManifest newVersionManifest)
        {
            List<UpdateRecipeFile> recipeFiles = new List<UpdateRecipeFile>();
            foreach (var file in newVersionManifest.Files)
            {
                VersionManifestFile originalFile = this.Files.FirstOrDefault(x => x.Name.Equals(file.Name, StringComparison.CurrentCultureIgnoreCase));
                FileUpdateAction action = originalFile != null && originalFile.Checksum == file.Checksum ? FileUpdateAction.Copy : FileUpdateAction.Download;
                recipeFiles.Add(new UpdateRecipeFile(file.Name, file.Checksum, file.Size, action));
            }

            return new UpdateRecipe(newVersionManifest.Version, this.Version, recipeFiles);
        }
    }
}
