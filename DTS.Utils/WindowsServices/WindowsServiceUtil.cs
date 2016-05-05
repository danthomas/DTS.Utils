using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DTS.Utils.Core;

namespace DTS.Utils.WindowsServices
{
    public class WindowsServiceUtil : UtilBase
    {
        private string _server;

        public WindowsServiceUtil()
            : base("svc", "Windows Service NugetUtil")
        {
            Command<SessionArgs, Action>()
                .Action(Action.Server, "Sets the server for the current session")
                .Arg("n", x => x.Server)
                .NoOp(SetServer);

            Command<ListArgs, Action>()
                .Action(Action.List, "Lists the services filtered by name")
                .Arg("n", x => x.Name)
                .RunProcess(GetListRunProcessDetails, ProcessListOutput);

            Command<StateArgs, Action>()
                .Action(Action.State, "Gets the action of the specified service")
                .Action(Action.Start, "Starts the specified service")
                .Action(Action.Stop, "Stops the specified service")
                .Arg("n", x => x.Service)
                .Arg("s", x => x.Server)
                .RunProcess(GetStateStopStartRunProcessDetails, ProcessStateStopStartOutput);
        }

        private ReturnValue SetServer(SessionArgs args, Action action)
        {
            _server = args.Server;
            return ReturnValue.Ok();
        }

        private RunProcessDetails GetListRunProcessDetails(ListArgs listArgs, Action action)
        {
            return new RunProcessDetails
            {
                Exe = "sc.exe",
                Args = "query state= all"
            };
        }

        private ReturnValue ProcessListOutput(ListArgs listArgs, Action action, string output)
        {
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.StartsWith("SERVICE_NAME: "))
                .Select(x => x.Replace("SERVICE_NAME: ", ""))
                .Where(x => String.IsNullOrWhiteSpace(listArgs.Name) || x.ToLower().Contains(listArgs.Name.ToLower())).ToList();

            lines.Add($"{lines.Count()} services found");

            return ReturnValue.Ok(String.Join(Environment.NewLine, lines));
        }

        private RunProcessDetails GetStateStopStartRunProcessDetails(StateArgs stateArgs, Action action)
        {
            return new RunProcessDetails
            {
                Exe = "sc.exe",
                Args = GetArgs(stateArgs, action)
            };
        }

        private ReturnValue ProcessStateStopStartOutput(StateArgs stateArgs, Action action, string output)
        {
            string state = output.SplitAndTrim(Environment.NewLine)
                .Where(x => x.StartsWith("STATE"))
                .Select(x => x.EverythingAfterLast(" "))
                .FirstOrDefault();

            ReturnValue returnValue = ReturnValue.Ok();

            if (action == Action.Start)
            {
                returnValue = CheckOutputForErrors(output, 1056);
            }
            else if (action == Action.Stop)
            {
                returnValue = CheckOutputForErrors(output, 1062);
            }
            else if (action == Action.State && state == null)
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

        private string GetArgs(StateArgs x, Action action)
        {
            string server = String.IsNullOrWhiteSpace(x.Server) ? _server : x.Server;

            string ret = String.IsNullOrWhiteSpace(server) ? "" : "//" + server + " ";

            ret += $"{(action == Action.State ? "query" : action.ToString().ToLower())} {x.Service}";

            return ret;
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

        enum Action
        {
            State,
            Stop,
            Start,
            Server,
            List
        }
    }
}
