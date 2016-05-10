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
                .NoOp(StartProcess);
        }

        private SelectOptionDetails GetStopProcessConfirmation(Args args, CommandType commandType, Context context)
        {
            return new SelectOptionDetails
            {
                Message = $"Stop {context.Processes.Length} Processes?",
                Options = new[] { "No", "Yes" },
                OptionSelected = x =>
                { context.StopConfirmed = x == "Yes"; }
            };
        }

        private ReturnValue StartProcess(Args args, CommandType commandType, Context context)
        {
            Process.Start(args.FilePath);

            return ReturnValue.Ok();
        }

        private IfDetails FindProcesses(Args args, CommandType commandType, Context context)
        {
            context.Processes = Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(args.Name.ToLower())).ToArray();

            return new IfDetails { If = context.Processes.Length > 0, Message = "No processed found" };
        }

        private ReturnValue StopProcesses(Args args, CommandType commandType, Context context)
        {
            if (context.StopConfirmed)
            {
                foreach (var process in context.Processes)
                {
                    process.Kill();
                }
            }

            return ReturnValue.Ok();
        }

        private IEnumerable<string> ListProcesses(Args args, CommandType commandType, Context context)
        {
            return Process.GetProcesses()
                .Select(x => x.ProcessName)
                .Where(x => String.IsNullOrWhiteSpace(args.Name) || x.ToLower().Contains(args.Name.ToLower()));
        }

        private enum CommandType
        {
            List,
            Start,
            Stop
        }

        private class Args
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
        }

        private class Context
        {
            public Process[] Processes { get; set; }
            public bool StopConfirmed { get; set; }
        }
    }
}
