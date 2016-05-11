using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DTS.Utils.Core;
using DTS.Utils.Details;

namespace DTS.Utils.Processes
{
    public class ProcessesUtil : UtilBase
    {
        public ProcessesUtil() : base("proc", "Processes Utility")
        {
            Command<Args, CommandType, Context>()
                .Action(CommandType.List, "Lists the running processes")
                .Arg("n", x => x.Name)
                .WriteOutput(ListProcesses);

            Command<Args, CommandType, Context>()
                .Action(CommandType.Stop, "Stops the process")
                .Arg("n", x => x.Name)
                .If(FindProcesses)
                .SelectOption(GetStopProcessConfirmation)
                .NoOp(StopProcesses);

            Command<Args, CommandType, Context>()
                .Action(CommandType.Start, "Starts the process")
                .Arg("p", x => x.FilePath, true)
                .RunProcess(StartProcess);
        }

        public IfDetails FindProcesses(Args args, CommandType commandType, Context context)
        {
            //ToDo: move this out to UtilRunner
            context.Processes = Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(args.Name.ToLower())).Select(x => new ProcessWrapper(x)).ToArray();

            return new IfDetails { If = context.Processes.Length > 0, Message = "No processed found" };
        }

        public SelectOptionAction GetStopProcessConfirmation(Args args, CommandType commandType, Context context)
        {
            return new SelectOptionAction
            {
                Message = $"Stop {context.Processes.Length} {(context.Processes.Length == 1 ? "Process" : "Processes")}?",
                Options = new[] { "No", "Yes" },
                OptionSelected = x => { context.StopConfirmed = x == "Yes"; }
            };
        }

        public RunProcessDetails StartProcess(Args args, CommandType commandType, Context context)
        {
            return new RunProcessDetails
            {
                Args = "",
                Exe = args.FilePath
            };
        }

        public ReturnValue StopProcesses(Args args, CommandType commandType, Context context)
        {
            if (context.StopConfirmed)
            {
                foreach (var process in context.Processes)
                {
                    process.Stop();
                }
            }

            return ReturnValue.Ok();
        }

        public IEnumerable<string> ListProcesses(Args args, CommandType commandType, Context context)
        {
            return Process.GetProcesses()
                .Select(x => x.ProcessName)
                .Where(x => String.IsNullOrWhiteSpace(args.Name) || x.ToLower().Contains(args.Name.ToLower()));
        }

        public enum CommandType
        {
            List,
            Start,
            Stop
        }

        public class Args
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
        }

        public class Context
        {
            public IProcess[] Processes { get; set; }
            public bool StopConfirmed { get; set; }
        }

        public interface IProcess
        {
            void Stop();
        }

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
        }
    }
}
