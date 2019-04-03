using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TaskManager.Annotations;
using TaskManager.Models;
using TaskManager.Tools;
using TaskManager.Tools.Managers;
using TaskManager.Tools.Navigation;

namespace TaskManager.ViewModels
{
    class ShowInfoViewModel : INotifyPropertyChanged
    {
        #region Fields
        private MyProcess SelectedProcess { get;}
        private ICommand _backCommand;

        private ProcessModuleCollection _modules;
        private ProcessThreadCollection _threads;
        #endregion


        public ProcessModuleCollection Modules
        {
            get => _modules;
            set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }
        public ProcessThreadCollection Threads
        {
            get { return _threads; }
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public ShowInfoViewModel(MyProcess selectedProcess)
        {
            SelectedProcess = selectedProcess;
            Process pr = Process.GetProcessById(SelectedProcess.Id);
            try
            {
                Modules = pr.Modules;
            }
            catch (Win32Exception)
            {
                Modules = null;
            }

            try
            {
                Threads = pr.Threads;
            }
            catch (Win32Exception)
            {
                Threads = null;
            }
        }
        public ICommand BackCommand => _backCommand ?? (_backCommand = new RelayCommand<object>(BackImplementation));
      
       
        private void BackImplementation(object obj)
        {
            NavigationManager.Instance.Navigate(ViewType.TaskManager,null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
