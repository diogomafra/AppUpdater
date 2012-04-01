
namespace AppUpdater
{
    public class UpdateInfo
    {
        public string Version { get; private set; }

        public UpdateInfo(string version)
        {
            this.Version = version;
        }
    }
}
