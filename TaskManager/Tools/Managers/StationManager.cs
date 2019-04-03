

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using TaskManager.ViewModels;

namespace TaskManager.Tools.Managers
{
    internal static class StationManager
    {
        public static event Action StopThreads;

        private static List<Process> _processes;
        
        internal static void Initialize()
        {
        }
        
        internal static List<Process> GetProcesses()
        {
            _processes = new List<Process>(Process.GetProcesses());
            return _processes;
        }
        internal static void CloseApp()
        {
            MessageBox.Show("ShutDown");
            StopThreads?.Invoke();
            Environment.Exit(1);
        }

    }

}
