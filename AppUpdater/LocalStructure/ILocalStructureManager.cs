using AppUpdater.Manifest;
using System;

namespace AppUpdater.LocalStructure
{
    public interface ILocalStructureManager
    {
        void CreateVersionDir(string version);
        void DeleteVersionDir(string version);
        VersionManifest LoadManifest(string version);
        string GetCurrentVersion();
        void SetCurrentVersion(string version);
        bool HasVersionFolder(string version);
        void CopyFile(string originVersion, string destinationVersion, string filename);
        void SaveFile(string version, string filename, byte[] data);
        Uri GetUpdateServerUri();
    }
}
