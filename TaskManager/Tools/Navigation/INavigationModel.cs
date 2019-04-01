
using TaskManager.Models;

namespace TaskManager.Tools.Navigation
{
    internal enum ViewType
    {
        TaskManager,
        ShowInfo
    }

    interface INavigationModel
    {
        void Navigate(ViewType viewType,MyProcess selectedProcess);
    }
}
