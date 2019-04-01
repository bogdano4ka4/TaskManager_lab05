
namespace TaskManager.Tools.Navigation
{
    internal enum ViewType
    {
        TaskManager
    }

    interface INavigationModel
    {
        void Navigate(ViewType viewType);
    }
}
