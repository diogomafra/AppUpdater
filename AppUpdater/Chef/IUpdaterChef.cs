using AppUpdater.Recipe;

namespace AppUpdater.Chef
{
    public interface IUpdaterChef
    {
        void Cook(UpdateRecipe recipe);
    }
}
