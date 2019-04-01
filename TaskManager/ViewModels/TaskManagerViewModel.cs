
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
        private List<MyProcess> _processes;
        private Thread _workingThread;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private MyProcess _selectedProcess;
        private MyProcess _saveSelectedProcess;
        private string _modules;

        #region Commands
        private ICommand _showInfoCommand;
        private ICommand _stopProccesCommand;
        private ICommand _openCommand;
        #endregion

        public List<MyProcess> Processes
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

        public string Modules
        {
            get => _modules;
            set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

      
       
        public ICommand StopProcessCommand => _stopProccesCommand ?? (_stopProccesCommand = new RelayCommand<object>(StopProcessImplementation, CanExecuteCommand));
        public ICommand ShowInfoCommand => _showInfoCommand ?? (_showInfoCommand = new RelayCommand<object>(ShowModulesImplementation, CanExecuteCommand));
        public ICommand OpenFolderCommand => _openCommand ?? (_openCommand = new RelayCommand<object>(OpenFolderImplementation, CanExecuteCommand));

        private void OpenFolderImplementation(object obj)
        {
            if (_selectedProcess.Path != "Access denied.")
            {
                try
                {
                    Process.Start(Path.GetDirectoryName(_selectedProcess.Path));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ex");
                }
            }
            else
            {
                MessageBox.Show("ss");

            }
          
        }
        
        private void StopProcessImplementation(object obj)
        {
            Process pr= Process.GetProcessById(_selectedProcess.Id);
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
            //Process pr = Process.GetProcessById(_selectedProcess.Id);
            //switch (Choice)
            //{
            //    case 0:
            //        //  MessageBox.Show(pr.ProcessName);
            //        int i = 0;
            //        try
            //        {
            //            Modules = "";
            //            Modules += $"List of modules : ({pr.ProcessName}) \n";
            //            ProcessModuleCollection modules = pr.Modules;
            //            foreach (ProcessModule module in modules)
            //                Modules += (i++) + ". " + module.ModuleName + " - " + module.FileName + "\n";

            //        }
            //        catch (Win32Exception ex)
            //        {
            //            Modules += ex.Message;
            //        }
            //        break;
            //    case 1:
            //        try
            //        {
            //            Modules = "";
            //            Modules += $"List of threads : ({pr.ProcessName}) \n";
            //            ProcessThreadCollection th = pr.Threads;
            //            foreach (ProcessThread thread in th)
            //                Modules += thread.Id + " | " + thread.ThreadState + " | " + thread.StartTime + "\n";
            //        }
            //        catch (Win32Exception ex1)
            //        {
            //            Modules += ex1.Message;
            //        }

            //        break;
            //    default:
            //        MessageBox.Show("Default");
            //        break;
            //}
        }

        private bool CanExecuteCommand(object obj)
        {
            return _selectedProcess != null;
        }


        internal TaskManagerViewModel()
        {
            Processes = new List<MyProcess>();
            var processes = Process.GetProcesses();
            foreach (Process pr in processes)
            {
                _processes.Add(new MyProcess(pr));
            }

            UpdateData();

            //  _tokenSource = new CancellationTokenSource();
            // _token = _tokenSource.Token;
            //StartWorkingThread();
            //StationManager.StopThreads += StopWorkingThread;
        }

        private async void UpdateData()
        {
            while (true)
            {
                List<MyProcess> newProcesses = new List<MyProcess>();
                await Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    var processesArray = Process.GetProcesses();
                    foreach (Process pr in processesArray)
                        newProcesses.Add(new MyProcess(pr));
                    
                });
                Processes = newProcesses;
            }
        }

        private void StartWorkingThread()
        {
            _workingThread = new Thread(WorkingThreadProcess);
            _workingThread.Start();
        }

      
        private void WorkingThreadProcess()
        {
           
            int i = 0;
            while (!_token.IsCancellationRequested)
            {

                
                var process = _processes.ToList();
                LoaderManager.Instance.ShowLoader();
                if (SelectedProcess != null)
                {
                    _saveSelectedProcess = SelectedProcess;
                    MessageBox.Show(_saveSelectedProcess.Name);
                }

                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }

                if (_token.IsCancellationRequested)
                    break;
                LoaderManager.Instance.HideLoader();

                if (_saveSelectedProcess != null)
                    SelectedProcess = _saveSelectedProcess;
                
                   
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }

                if (_token.IsCancellationRequested)
                    break;
                i++;
                
            }
        }

        internal void StopWorkingThread()
        {
            _tokenSource.Cancel();
            _workingThread.Join(2000);
            _workingThread.Abort();
            _workingThread = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
