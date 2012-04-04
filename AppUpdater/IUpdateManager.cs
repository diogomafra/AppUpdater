
namespace AppUpdater
{
    public interface IUpdateManager
    {
        void Initialize();
        UpdateInfo CheckForUpdate();
        void DoUpdate(UpdateInfo updateInfo);
    }
}
