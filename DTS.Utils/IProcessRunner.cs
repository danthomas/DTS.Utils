using System.Diagnostics;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public interface IProcessRunner
    {
        ReturnValue Run(RunProcessDetails runProcessDetails);
    }

    public class ProcessRunner : IProcessRunner
    {
        public ReturnValue Run(RunProcessDetails runProcessDetails)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = runProcessDetails.Exe,
                    Arguments = runProcessDetails.Args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            return ReturnValue.Ok(proc.StandardOutput.ReadToEnd());
        }
    }
}