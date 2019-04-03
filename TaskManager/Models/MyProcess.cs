using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TaskManager.Models
{
    public class MyProcess
    {

        #region Fields
       
        private PerformanceCounter CounterCpu { get; }
        private PerformanceCounter CounterOperationMemory { get; }
        #endregion

        #region Properties

        internal MyProcess(Process process)
        {
            Name = process.ProcessName;
            Id = process.Id;
            ThreadsCount = process.Threads.Count;
            SetPathName(process);
            SetProcessTime(process);
            CounterCpu= new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            IsActive = process.Responding;
            CounterOperationMemory = new PerformanceCounter("Process", "Working Set", process.ProcessName, true);

        }

        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }

        public double Cpu
        {
            get
            {
               return  CounterCpu.NextValue() / Environment.ProcessorCount;
            }
            set
            {

            }
        }

        public double OperatingMemory
        {
            get
            {
                return CounterOperationMemory.NextValue() / Environment.ProcessorCount;
            }
            set { }
        }

        public int ThreadsCount { get; set; }
        public string Path { get; set; }
        public DateTime? ProcessTime { get; set; }
        #endregion
       
        public void Relaod()
        {
            Cpu= CounterCpu.NextValue() / Environment.ProcessorCount;
            OperatingMemory= CounterOperationMemory.NextValue() / Environment.ProcessorCount;

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

        private void SetPathName(Process process)
        {
            try
            {
                Path = process.MainModule.FileName;
            }
            catch (Win32Exception)
            {
                Path = "Access denied.";

            }
        }

    }
}
