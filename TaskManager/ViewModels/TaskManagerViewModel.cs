
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private List<Process> _processes=StationManager.Processes;
        private Thread _workingThread;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private int _choiceIndex;
        private MyProcess _selectedProcess;
        private MyProcess _saveSelectedProcess;
        private string _modules;

        #region Commands
        private ICommand _showInfoCommand;
        private ICommand _stopProccesCommand;
        private ICommand _openCommand;
        #endregion

        public int Choice
        {
            get => _choiceIndex;
            set
            {
                _choiceIndex = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<MyProcess> Processes
        {
            get { return ShowMyProcesses(); }
           
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

        private IEnumerable<MyProcess> ShowMyProcesses()
        {
            _processes = new List<Process>(StationManager.Processes);
            var proccesList = (from item in _processes
                select new MyProcess(item));
           return proccesList;
        }
       
        public ICommand StopProcessCommand => _stopProccesCommand ?? (_stopProccesCommand = new RelayCommand<object>(StopProcessImplementation, CanExecuteCommand));
        public ICommand ShowInfoCommand => _showInfoCommand ?? (_showInfoCommand = new RelayCommand<object>(ShowModulesImplementation, CanExecuteCommand));
        public ICommand OpenFolderCommand => _openCommand ?? (_openCommand = new RelayCommand<object>(OpenFolderImplementation, CanExecuteCommand));

        private void OpenFolderImplementation(object obj)
        {
            //TODO create better method
            Process pr = Process.GetProcessById(_selectedProcess.Id);
            try
            {
                Process.Start(SelectedProcess.Path);
                string str = pr.MainModule.FileName;
                string re = '\\' + pr.MainModule.ModuleName;
                str = str.Replace(re, "");
                Process.Start(str);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            Process pr = Process.GetProcessById(_selectedProcess.Id);
            switch (Choice)
            {
                case 0:
                    //  MessageBox.Show(pr.ProcessName);
                    int i = 0;
                    try
                    {
                        Modules = "";
                        Modules += $"List of modules : ({pr.ProcessName}) \n";
                        ProcessModuleCollection modules = pr.Modules;
                        foreach (ProcessModule module in modules)
                            Modules += (i++) + ". " + module.ModuleName + " - " + module.FileName + "\n";
                            
                    }
                    catch (Win32Exception ex)
                    {
                        Modules += ex.Message;
                    }
                    // MessageBox.Show(Modules);
                    break;
                case 1:
                    try
                    {
                        Modules = "";
                        Modules += $"List of threads : ({pr.ProcessName}) \n";
                        ProcessThreadCollection th = pr.Threads;
                        foreach (ProcessThread thread in th)
                            Modules += thread.Id + " | " + thread.ThreadState + " | " + thread.StartTime+"\n";
                    }
                    catch (Win32Exception ex1)
                    {
                        Modules += ex1.Message;
                    }
                    break;
                default:
                    MessageBox.Show("Default");
                    break;
            }
            
            
       

        }


        private bool CanExecuteCommand(object obj)
        {
            return _selectedProcess != null;
        }


        internal TaskManagerViewModel()
        {

            _processes = new List<Process>(StationManager.GetProcesses());
        
           _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartWorkingThread();
            StationManager.StopThreads += StopWorkingThread;
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
