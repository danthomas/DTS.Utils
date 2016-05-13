using System;
using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;
using DTS.Utils.Details;

namespace DTS.Utils.WindowsServices
{
    public class WindowsServiceUtil : UtilBase
    {
        private string _server;

        public WindowsServiceUtil()
            : base("svc", "Windows Service NugetUtil")
        {
            Command<SessionArgs, CommandType, SessionContext>()
                .Action(CommandType.Server, "Sets the server for the current session")
                .Arg("n", x => x.Server)
                .NoOp(SetServer);

            Command<ListArgs, CommandType, ListContext>()
                .Action(CommandType.List, "Lists the services filtered by name")
                .Arg("n", x => x.Name)
                .RunProcess(GetListRunProcessDetails, (s, y) => y.Output = s)
                .WriteOutput(ProcessListOutput);

            Command<StateArgs, CommandType, StateContext>()
                .Action(CommandType.State, "Gets the CommandType of the specified service")
                .Action(CommandType.Start, "Starts the specified service")
                .Action(CommandType.Stop, "Stops the specified service")
                .Arg("n", x => x.Service)
                .Arg("s", x => x.Server)
                .RunProcess(GetStateStopStartRunProcessDetails, (s, y) => y.Output = s)
                .NoOp(ProcessStateStopStartOutput);
        }

        private ReturnValue SetServer(SessionContext context)
        {
            _server = context.Args.Server;
            return ReturnValue.Ok();
        }

        private RunProcessDetails GetListRunProcessDetails(ListContext context)
        {
            return new RunProcessDetails
            {
                Exe = "sc.exe",
                Args = "query state= all"
            };
        }

        private IEnumerable<string> ProcessListOutput(ListContext context)
        {
            var lines = context.Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.StartsWith("SERVICE_NAME: "))
                .Select(x => x.Replace("SERVICE_NAME: ", ""))
                .Where(x => String.IsNullOrWhiteSpace(context.Args.Name) || x.ToLower().Contains(context.Args.Name.ToLower())).ToList();

            var count = lines.Count;

            lines.Add($"{count} service{(count == 1 ? "" : "s")} found");

            return  lines;
        }

        private RunProcessDetails GetStateStopStartRunProcessDetails(StateContext context)
        {
            return new RunProcessDetails
            {
                Exe = "sc.exe",
                Args = GetArgs(context.Args, context.CommandType)
            };
        }

        private ReturnValue ProcessStateStopStartOutput(StateContext context)
        {
            string state = context.Output.SplitAndTrim(Environment.NewLine)
                .Where(x => x.StartsWith("STATE"))
                .Select(x => x.EverythingAfterLast(" "))
                .FirstOrDefault();

            ReturnValue returnValue = ReturnValue.Ok();

            if (context.CommandType == CommandType.Start)
            {
                returnValue = CheckOutputForErrors(context.Output, 1056);
            }
            else if (context.CommandType == CommandType.Stop)
            {
                returnValue = CheckOutputForErrors(context.Output, 1062);
            }
            else if (context.CommandType == CommandType.State && state == null)
            {
                returnValue = ReturnValue.Error(ErrorType.ServiceStateNotFound, "Failed to find the state");
            }

            return returnValue.IsSuccess
                ? ReturnValue.Ok(state)
                : returnValue;
        }

        private ReturnValue CheckOutputForErrors(string output, int errorCode)
        {
            var lines = output.SplitAndTrim(Environment.NewLine);

            if (lines.Count > 0 && lines[0].Contains(errorCode.ToString()))
            {
                ErrorType errorType = ErrorType.ScError;
                
                switch (errorCode)
                {
                    case 1056:
                        errorType = ErrorType.StartServiceFailed;
                        break;
                    case 1062:
                        errorType = ErrorType.StopServiceFailed;
                        break;
                }

                var errorMessage = lines.Skip(1).FirstOrDefault() ?? "Unknown error";

                return ReturnValue.Error(errorType, errorMessage);
            }

            return ReturnValue.Ok();
        }

        private string GetArgs(StateArgs x, CommandType commandType)
        {
            string server = String.IsNullOrWhiteSpace(x.Server) ? _server : x.Server;

            string ret = String.IsNullOrWhiteSpace(server) ? "" : "//" + server + " ";

            ret += $"{(commandType == CommandType.State ? "query" : commandType.ToString().ToLower())} {x.Service}";

            return ret;
        }

        private class SessionContext : Context<SessionArgs, CommandType>
        {
        }

        private class ListContext : Context<ListArgs, CommandType>
        {
            public string Output { get; set; }
        }

        private class StateContext : Context<StateArgs, CommandType>
        {
            public string Output { get; set; }
        }

        class ListArgs
        {
            public string Name { get; set; }
        }

        class SessionArgs
        {
            public string Server { get; set; }
        }

        class StateArgs
        {
            public string Server { get; set; }
            public string Service { get; set; }
        }

        enum CommandType
        {
            State,
            Stop,
            Start,
            Server,
            List
        }
    }
}
