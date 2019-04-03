
using System;
using TaskManager.Models;
using TaskManager.ViewModels;
using TaskManager.Views;

namespace TaskManager.Tools.Navigation
{
    internal class InitializationNavigationModel : BaseNavigationModel
    {
        public InitializationNavigationModel(IContentOwner contentOwner) : base(contentOwner)
        {

        }

        protected override void InitializeView(ViewType viewType,MyProcess selectedProcess)
        {
            switch (viewType)
            {
                case ViewType.TaskManager:
                    ViewsDictionary.Add(viewType, new TaskManagerView());
                    break;
                case ViewType.ShowInfo:
                    if (ViewsDictionary.ContainsKey(viewType))
                        ViewsDictionary[viewType] = new ShowInfoView(selectedProcess);
                       else
                        ViewsDictionary.Add(viewType, new ShowInfoView(selectedProcess));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null);
            }
        }
    }
}
