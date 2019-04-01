﻿

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TaskManager.Annotations;
using TaskManager.Tools.Managers;

namespace TaskManager.Models
{
    class MyProcess :  Process, INotifyPropertyChanged
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
        private DateTime? _processTime;

        private ProcessModuleCollection _modules;
        private ProcessThreadCollection _processThreads;
        #endregion

        #region Properties

        internal MyProcess(Process process)
        {
           
            Name = process.ProcessName;
            Id = process.Id;
            ThreadsCount = process.Threads.Count;
            UserName = process.MachineName; //UserName = Environment.UserName,
            SetPathName(process);
            SetProcessTime(process);
            // IsActive = false;

        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }

        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

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
        public DateTime? ProcessTime
        {
            get { return _processTime; }
            set
            {
                _processTime = value;
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

        private void SetPathName(Process process)
        {
            try
            {
                _path = process.MainModule.FileName;
            }
            catch (Win32Exception)
            {
                _path = "Access denied.";

            }

        }

        private void SetProcessTime(Process process)
        {
            try
            {
                ProcessTime = process.StartTime;
            }
            catch (Win32Exception)
            {

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
