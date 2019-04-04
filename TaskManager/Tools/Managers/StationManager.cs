using System;
using System.Windows;

namespace TaskManager.Tools.Managers
{
    internal static class StationManager
    {
        public static event Action StopThreads;
   
        internal static void CloseApp()
        {
            MessageBox.Show("ShutDown");
            StopThreads?.Invoke();
            Environment.Exit(1);
        }

    }

}
