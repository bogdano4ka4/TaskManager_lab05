using System;
using System.ComponentModel;
using System.Diagnostics;
using TaskManager.Tools;

namespace TaskManager.Models
{
    public class MyProcess
    {
        #region Properties
        private PerformanceCounter CounterCpu { get; }
        private PerformanceCounter CounterOperationMemory { get; }
        private readonly long _total = PerformanceInfo.GetTotalMemoryInMiB() * 10000;
        private readonly int _processorCount = Environment.ProcessorCount;
        private Process Pr;
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public double Cpu { get; set; }
        public double OperatingMemory { get; set; }
        public int ThreadsCount { get; set; }
        public string Path { get; set; }
        public DateTime? ProcessTime { get; set; }
        #endregion

        internal MyProcess(Process process)
        {
            Pr = process;
            Name = process.ProcessName;
            Id = process.Id;
            ThreadsCount = process.Threads.Count;
            SetPathName(process);
            SetProcessTime(process);
            CounterCpu = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            IsActive = process.Responding;
            CounterOperationMemory = new PerformanceCounter("Process", "Working Set", process.ProcessName, true);
            UpdateFields();
        }
        public void UpdateFields()
        {
            try
            {
                Cpu = Math.Round(CounterCpu.NextValue() / _processorCount, 2);
            }
            catch (InvalidOperationException) { }
            try { 
            OperatingMemory = Math.Round(CounterOperationMemory.NextValue() / _total,2);
            }
            catch (InvalidOperationException) { }
            ThreadsCount = Pr.Threads.Count;
        }
        private void SetProcessTime(Process process)
        {
            try
            {
                ProcessTime = process.StartTime;
            }
            catch (InvalidOperationException)
            {
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
            catch (InvalidOperationException )
            {
            }
            catch (Win32Exception)
            {
                Path = "Access denied.";
            }
        }

        public override bool Equals(object obj)
        {
            return obj is MyProcess other && Id == other.Id;
        }
    }

}
