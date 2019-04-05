using System.Collections.Generic;
using TaskManager.Models;


namespace TaskManager.Tools.Navigation
{
    internal abstract class BaseNavigationModel : INavigationModel
    {
        protected BaseNavigationModel(IContentOwner contentOwner)
        {
            ContentOwner = contentOwner;
            ViewsDictionary = new Dictionary<ViewType, INavigatable>();
        }

        protected IContentOwner ContentOwner { get; }

        protected Dictionary<ViewType, INavigatable> ViewsDictionary { get; }

        public void Navigate(ViewType viewType, MyProcess selectedProcess)
        {
            if ((!ViewsDictionary.ContainsKey(viewType))||viewType==ViewType.ShowInfo)
                InitializeView(viewType, selectedProcess);
            ContentOwner.ContentControl.Content = ViewsDictionary[viewType];
        }

        protected abstract void InitializeView(ViewType viewType,MyProcess selectedProcess);

    }
}
