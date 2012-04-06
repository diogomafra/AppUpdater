
namespace AppUpdater.Recipe
{
    public enum FileUpdateAction
    {
        Copy,
        Download
    }

    public class UpdateRecipeFile
    {
        public string Name { get; private set; }
        public string Checksum { get; private set; }
        public long Size { get; private set; }
        public FileUpdateAction Action { get; private set; }

        public string DeployedName
        {
            get
            {
                return Name == null ? null : Name + ".deploy";
            }
        }

        public UpdateRecipeFile(string name, string checksum, long size, FileUpdateAction action)
        {
            this.Name = name;
            this.Checksum = checksum;
            this.Size = size;
            this.Action = action;
        }
    }
}
