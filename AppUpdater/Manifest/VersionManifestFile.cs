
namespace AppUpdater.Manifest
{
    public class VersionManifestFile
    {
        public string Name { get; private set; }
        public string Checksum { get; private set; }
        public long Size { get; private set; }

        public string DeployedName
        {
            get
            {
                return Name == null ? null : Name + ".deploy";
            }
        }

        public VersionManifestFile(string name, string checksum, long size)
        {
            this.Name = name;
            this.Checksum = checksum;
            this.Size = size;
        }
    }
}
