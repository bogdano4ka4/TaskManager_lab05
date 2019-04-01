
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Tools;
using TaskManager.Tools.Managers;

namespace TaskManager.ViewModels
{
    class TaskManagerViewModel : INotifyPropertyChanged
    {
        private List<Process> _processes=StationManager.Processes;
        private Thread _workingThread;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private Process _selectedProcess;

        #region Commands
        private ICommand _showModulesCommand;
        private ICommand _showThreadsCommand;
        private ICommand _stopProccesCommand;
        private ICommand _openFolderCommand;
        #endregion

        public IEnumerable<MyProcess> Processes
        {
            get { return showMyProcesses(); }
        }
        public Process SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<MyProcess> showMyProcesses()
        {
            _processes = new List<Process>(StationManager.Processes);
            var proccesList = (from item in _processes
                select new MyProcess
                {
                    Name = item.ProcessName,
                    Id = item.Id,
                    //- Ідентифікатор того чи процес активний і відповідає на запити
                    //CPU
                    //  OperatingMemory =(double) item.PrivateMemorySize64/1024, //operatingMemory
                    ThreadsCount = item.Threads.Count,
                    UserName = item.MachineName,

                    // -Ім'я і шлях до файлу, звідки процес запущено
                    //- Дата та час запуску процесу


                });
            return proccesList;
        }
        public ICommand StopProcessCommand => _stopProccesCommand ?? (_stopProccesCommand = new RelayCommand<object>(StopProcessImplementation, CanExecuteCommand));

        private void StopProcessImplementation(object obj)
        {
            //TODO delete method
        }

        private bool CanExecuteCommand(object obj)
        {
            return _selectedProcess != null;
        }


        internal TaskManagerViewModel()
        {

            //_processes = new List<Process>(StationManager.Processes);
            //_tokenSource = new CancellationTokenSource();
            //_token = _tokenSource.Token;
            //StartWorkingThread();
            //StationManager.StopThreads += StopWorkingThread;
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
                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }

                if (_token.IsCancellationRequested)
                    break;
                LoaderManager.Instance.HideLoader();
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
