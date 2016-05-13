using System;
using System.Collections.Generic;
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
                .GetProcesses(GetProcessesLike)
                .WriteOutput(ListProcesses);

            Command<Args, CommandType, Context>()
                .Action(CommandType.Stop, "Stops the process")
                .Arg("n", x => x.Name)
                .GetProcesses(GetProcessesEqual)
                .If(AnyProcesses)
                .IfSelectOption(GetStopProcessConfirmation)
                .NoOp(StopProcesses);

            Command<Args, CommandType, Context>()
                .Action(CommandType.Start, "Starts the process")
                .Arg("p", x => x.FilePath, true)
                .RunProcess(StartProcess);
        }

        private void GetProcessesLike(IProcess[] processes, Context context)
        {
            context.Processes = processes.Where(x => String.IsNullOrWhiteSpace(context.Args.Name) || x.Name.ToLower().Contains(context.Args.Name.ToLower())).ToArray();
        }

        private void GetProcessesEqual(IProcess[] processes, Context context)
        {
            context.Processes = processes.Where(x => String.IsNullOrWhiteSpace(context.Args.Name) || x.Name.ToLower() == context.Args.Name.ToLower()).ToArray();
        }

        public IfDetails AnyProcesses(Context context)
        {
            return new IfDetails { If = context.Processes.Length > 0, Message = "No processed found" };
        }

        public IfSelectOptionAction GetStopProcessConfirmation(Context context)
        {
            return new IfSelectOptionAction
            {
                Message = $"Stop {context.Processes.Length} {(context.Processes.Length == 1 ? "Process" : "Processes")}?",
                Options = new[] { "No", "Yes" },
                Option = "Yes"
            };
        }

        public RunProcessDetails StartProcess(Context context)
        {
            return new RunProcessDetails
            {
                Args = "",
                Exe = context.Args.FilePath
            };
        }

        public ReturnValue StopProcesses(Context context)
        {
            foreach (var process in context.Processes.Where(x => x.Name.ToLower() == context.Args.Name.ToLower()))
            {
                process.Stop();
            }

            return ReturnValue.Ok();
        }

        public IEnumerable<string> ListProcesses(Context context)
        {
            return context.Processes
                .Select(x => x.Name);
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

        public class Context : Core.Context<Args, CommandType>
        {
            public IProcess[] Processes { get; set; }
        }
    }
}
