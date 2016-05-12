using System.Diagnostics;
using DTS.Utils.Details;

namespace DTS.Utils.Core
{
    public interface IProcessRunner
    {
        ReturnValue Run(RunProcessAction runProcessAction);
    }

    public class ProcessRunner : IProcessRunner
    {
        public ReturnValue Run(RunProcessAction runProcessAction)
        {
            bool readOutput = runProcessAction.Action != null;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = runProcessAction.RunProcessDetails.Exe,
                    Arguments = runProcessAction.RunProcessDetails.Args,
                    UseShellExecute = !readOutput,
                    RedirectStandardOutput = readOutput,
                    CreateNoWindow = readOutput
                }
            };

            proc.Start();

            if (readOutput)
            {
                return ReturnValue.Ok(proc.StandardOutput.ReadToEnd());
            }
            else
            {
                return ReturnValue.Ok();
            }
        }
    }
}