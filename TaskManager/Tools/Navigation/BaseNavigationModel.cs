using System.Collections.Generic;
using TaskManager.Models;


namespace TaskManager.Tools.Navigation
{
    internal abstract class BaseNavigationModel : INavigationModel
    {
        private readonly IContentOwner _contentOwner;

        protected BaseNavigationModel(IContentOwner contentOwner)
        {
            _contentOwner = contentOwner;
            ViewsDictionary = new Dictionary<ViewType, INavigatable>();
        }

        protected IContentOwner ContentOwner
        {
            get { return _contentOwner; }
        }

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
