

using System;
using System.Diagnostics;
using TaskManager.Tools;
using TaskManager.Tools.Managers;

namespace TaskManager.Models
{
    class MyProcess : BaseViewModel
    {
        #region Fields
        private MyProcess _selectedProcess;
        private string _name;
        private bool _isActive;
        private int _id;
        private double _cpu=-1;
        private double _opMemory=-1;//VirtualMemorySize64
        private int _threadsCount;//Process.GetCurrentProcess().Threads.Count ?
        private string _userName;
        private string _path;
        private DateTime _proccesTime;

        private ProcessModuleCollection _modules;
        private ProcessThreadCollection _processThreads;
        #endregion

        #region Properties

      
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }

        }

        public int Id { get; set; }

        public double CPU
        {
            get
            {
                //TODO method that counts CPU
                return 0.0;
            }
            set
            {
                _cpu = value;
                OnPropertyChanged();
            }

        }
        public double OperatingMemory
        {
            get { return _opMemory; }
            set
            {
                _opMemory = value;
                OnPropertyChanged();
            }

        }

        public int ThreadsCount
        {
            get { return _threadsCount; }
            set
            {
                _threadsCount = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        public DateTime ProccesTime
        {
            get { return _proccesTime; }
            set
            {
                _proccesTime = value;
                OnPropertyChanged();
            }
        }

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
            get { return _processThreads; }
            set
            {
                _processThreads = value;
                OnPropertyChanged();
            }
        }

        #endregion

        void StopProcess()
        {
            StationManager.CurrentProcess.Kill();
        }
        //void openFolder;

    }
}
