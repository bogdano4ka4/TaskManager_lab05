using System;
using System.Windows;

namespace TaskManager.Tools.Managers
{
    internal static class StationManager
    {
        public static event Action StopThreads;
        public static bool Stop = true;
        
        internal static void CloseApp()
        {
            Stop = false;
            MessageBox.Show("ShutDown");
            StopThreads?.Invoke();
            Environment.Exit(1);
        }

    }

}
