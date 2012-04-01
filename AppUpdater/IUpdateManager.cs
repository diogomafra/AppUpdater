
namespace AppUpdater
{
    public interface IUpdateManager
    {
        void Initialize();
        bool CheckForUpdate(out UpdateInfo updateInfo);
        void DoUpdate(UpdateInfo updateInfo);
    }
}
