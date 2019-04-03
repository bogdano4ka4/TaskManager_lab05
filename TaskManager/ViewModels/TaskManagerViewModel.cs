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
        private int _sortIndex;
        private MyProcess _selectedProcess;
        private MyProcess _saveSelectedProcess;
        private string _modules;



        #region Commands

        private ICommand _showInfoCommand;
        private ICommand _stopProcessCommand;
        private ICommand _openCommand;

        #endregion

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

        public int SortIndex
        {
            get => _sortIndex;
            set { _sortIndex = value; }
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            //TODO delete method
        }


        private void ShowModulesImplementation(object obj)
        {
            NavigationManager.Instance.Navigate(ViewType.ShowInfo, _selectedProcess);
        }

        private bool CanExecuteCommand(object obj)
        {
            return _selectedProcess != null;
        }


        internal TaskManagerViewModel()
        {
            Processes = new ObservableCollection<MyProcess>();
            var processes = Process.GetProcesses();
            foreach (Process pr in processes)
            {
                _processes.Add(new MyProcess(pr));
            }

            new Thread(UpdateData).Start();
        }
        private async void SortProcesses(int sortBy, ObservableCollection<MyProcess> collection)
        {
            ObservableCollection<MyProcess> newProcesses = null;
            switch (sortBy)
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
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.Id)));
                    break;
                case 3:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.IsActive)));
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
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.Path)));
                    break;
                case 8:
                    await Task.Run(() =>
                        newProcesses = new ObservableCollection<MyProcess>(collection.OrderBy(i => i.ProcessTime)));
                    break;
            }
            Processes = newProcesses;
        }


        private void UpdateData()
        {
            while (true)
            {
                List<MyProcess> newProcesses = Processes.ToList();

                var processesArray = Process.GetProcesses();
                var tempProcessModels = from pr in processesArray select new MyProcess(pr);
                List<MyProcess> newPrMod = tempProcessModels.ToList();

                for (int i = newProcesses.Count - 1; i >= 0; --i)
                    if (!newPrMod.Contains(newProcesses[i]))
                        newProcesses.RemoveAt(i);

                foreach (MyProcess pr in newPrMod)
                    if (!newProcesses.Contains(pr))
                        newProcesses.Add(pr);

                Processes=new ObservableCollection<MyProcess>(newProcesses);

              //  SortProcesses(SortIndex, curr);
                OnPropertyChanged();
                Thread.Sleep(5000);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
