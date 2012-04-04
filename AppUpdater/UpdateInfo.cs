
namespace AppUpdater
{
    public class UpdateInfo
    {
        public string Version { get; private set; }
        public bool HasUpdate { get; set; }

        public UpdateInfo(bool hasUpdate, string version)
        {
            this.HasUpdate = hasUpdate;
            this.Version = version;
        }
    }
}
