using System;
using System.ComponentModel;
using System.Diagnostics;
using TaskManager.Tools;

namespace TaskManager.Models
{
    public class MyProcess
    {

        #region Fields

        private PerformanceCounter CounterCpu { get; }
        private PerformanceCounter CounterOperationMemory { get; }
        private long Total = PerformanceInfo.GetTotalMemoryInMiB() * 10000;
        private Process Pr;
        #endregion

        #region Properties

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
            Cpu = CounterCpu.NextValue() / Environment.ProcessorCount;
            OperatingMemory = CounterOperationMemory.NextValue() / Total;

        }

        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }

        public double Cpu { get; set; }

        public double OperatingMemory { get; set; }
       
        public int ThreadsCount { get; set; }
        public string Path { get; set; }
        public DateTime? ProcessTime { get; set; }
        #endregion

        public void Relaod()
        {
            try
            {
                Cpu = CounterCpu.NextValue() / Environment.ProcessorCount;
            }
            catch (InvalidOperationException) { }
            try { 
            OperatingMemory = CounterOperationMemory.NextValue() / Total;
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
            return !(obj is MyProcess other) ? false : Id == other.Id;
        }
    }
}
