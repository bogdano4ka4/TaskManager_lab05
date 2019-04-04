using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Tools;
using TaskManager.Tools.Managers;
using TaskManager.Tools.Navigation;

namespace TaskManager.ViewModels
{
    class TaskManagerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MyProcess> _processes;
        private MyProcess _selectedProcess;
      
        #region Commands

        private ICommand _showInfoCommand;
        private ICommand _stopProcessCommand;
        private ICommand _openCommand;

        #endregion

        internal TaskManagerViewModel()
        {
            Processes = new ObservableCollection<MyProcess>();
            var processes = Process.GetProcesses();
            foreach (Process pr in processes)
            {
                _processes.Add(new MyProcess(pr));
            }

            new Thread(UpdateTaskManager).Start();
            new Thread(UpdateModules).Start();
        }

        public ObservableCollection<MyProcess> Processes
        {
            get { return _processes; }
            set
            {
                _processes = value;
                OnPropertyChanged();
            }

        }

        public MyProcess SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        private int _sortIndex;
        public int SortIndex
        {
            get { return _sortIndex; }
            set
            {
                _sortIndex = value;
                Sort(Processes);
            }
        }



        public ICommand StopProcessCommand => _stopProcessCommand ??
                                              (_stopProcessCommand = new RelayCommand<object>(StopProcessImplementation,
                                                  CanExecuteCommand));

        public ICommand ShowInfoCommand => _showInfoCommand ??
                                           (_showInfoCommand = new RelayCommand<object>(ShowModulesImplementation,
                                               CanExecuteCommand));

        public ICommand OpenFolderCommand => _openCommand ??
                                             (_openCommand = new RelayCommand<object>(OpenFolderImplementation,
                                                 CanExecuteCommand));

        private void OpenFolderImplementation(object obj)
        {
            if (_selectedProcess.Path != "Access denied.")
            {
                try
                {
                    //TODO catch all exceptions
                    Process.Start(Path.GetDirectoryName(_selectedProcess.Path));
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (PathTooLongException ex1)
                {
                    MessageBox.Show(ex1.Message);
                }
            }
            else
                MessageBox.Show("Access denied.");
        }

        private void StopProcessImplementation(object obj)
        {
            Process pr = Process.GetProcessById(_selectedProcess.Id);
            try
            {
                pr.Kill();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }


        private void ShowModulesImplementation(object obj)
        {
            NavigationManager.Instance.Navigate(ViewType.ShowInfo, SelectedProcess);
        }

        private bool CanExecuteCommand(object obj)
        {
            return SelectedProcess != null;
        }


        
        private async void Sort(ObservableCollection<MyProcess> collection)
        {
            ObservableCollection<MyProcess> newProcesses = null;
            switch (SortIndex)
            {
                case 0:
                    Processes = collection;
                    return;
                case 1:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.Name)));
                    break;
                case 2:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.IsActive)));
                    break;
                   
                case 3:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.Id)));
                    break;

                case 4:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.Cpu)));
                    break;
                case 5:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.OperatingMemory)));
                    break;
                case 6:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.ThreadsCount)));
                    break;
                case 7:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.ProcessTime)));
                    break;
                case 8:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.Path)));
                    break;
            }
            Processes = newProcesses;
        }


        private List<MyProcess> ReturnNewProcessList()
        {
            List<MyProcess> newProcesses = Processes.ToList();

            var processesArray = Process.GetProcesses();
            List<MyProcess> newMyProcess = new List<MyProcess>();

            foreach (var pr in processesArray)
                newMyProcess.Add(new MyProcess(pr));
            for (int i = newProcesses.Count - 1; i >= 0; --i)
                if (!newMyProcess.Contains(newProcesses[i]))
                    newProcesses.RemoveAt(i);

            foreach (MyProcess pr in newMyProcess)
                if (!newProcesses.Contains(pr))
                    newProcesses.Add(pr);

            return newProcesses;
        }

        private void UpdateTaskManager()
        {
            while (true)
            {
                Sort(new ObservableCollection<MyProcess>(ReturnNewProcessList()));
                Thread.Sleep(5000);
            }
        }
        private void UpdateModules()
        {
            while (true)
            {
                
                foreach (MyProcess pr in Processes)
                    pr.Relaod();

                Processes=new ObservableCollection<MyProcess>(Processes);
                Thread.Sleep(2000);
            }
        }





        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
