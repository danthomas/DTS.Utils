using System.Diagnostics;

namespace DTS.Utils.Processes
{
    public class ProcessWrapper : IProcess
    {
        public Process Process { get; set; }

        public ProcessWrapper(Process process)
        {
            Process = process;
        }

        public void Stop()
        {
            Process.Kill();
        }

        public string Name { get { return Process.ProcessName; } }
    }
}