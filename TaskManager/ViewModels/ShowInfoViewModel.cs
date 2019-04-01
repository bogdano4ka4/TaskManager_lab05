using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
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
        private MyProcess _selectedProcess { get; set; }
        private ICommand _backCommand;
        private Process _curProcess = StationManager.CurrentProcess;
        private ICommand _openProcess;

        private ProcessModuleCollection _modules;
        private ProcessThreadCollection _threads;

        public ProcessModuleCollection Modules
        {
            get { return _modules; }
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

        public ShowInfoViewModel(MyProcess selectdProcess)
        {
            _selectedProcess = selectdProcess;
            Process pr = Process.GetProcessById(_selectedProcess.Id);
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
        public ICommand OpenFolderCommand => _openProcess ?? (_openProcess = new RelayCommand<object>(OpenFolderImplementation));

        private void OpenFolderImplementation(object obj)
        {

            MessageBox.Show(_curProcess.ProcessName);

        }


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
