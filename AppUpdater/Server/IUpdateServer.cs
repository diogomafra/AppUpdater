using AppUpdater.Manifest;

namespace AppUpdater.Server
{
    public interface IUpdateServer
    {
        string GetCurrentVersion();
        VersionManifest GetManifest(string version);
        byte[] DownloadFile(string version, string filename);
    }
}
