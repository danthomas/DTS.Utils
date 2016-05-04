using System;
using DTS.Utils.Core;

namespace DTS.Utils.WindowsServices
{
    public class Util : CommandRunner
    {
        private string _server;

        public Util(IRunner runner) 
            : base(runner)
        {
            Command<SessionArgs, Action>(Action.Server)
                .Arg("n", x => x.Server)
                .NoOp((x, a) =>
                {
                    _server = x.Server;
                    return ReturnValue.Ok();
                });

            Command<StateArgs, Action>(Action.State, Action.Stop, Action.Start)
                .Arg("n", x => x.Service)
                .Arg("s", x => x.Server)
                .Run((x, a) => new RunDetails
                {
                    Exe = "sc.exe",
                    Args = GetArgs(x, a)
                });
        }

        private string GetArgs(StateArgs x, Action state)
        {
            string server = String.IsNullOrWhiteSpace(x.Server) ?_server  : x.Server;

            string ret = String.IsNullOrWhiteSpace(server) ? "" : "//" + server + " ";

            ret += $"{(state == Action.State ? "query" : state.ToString().ToLower())} {x.Service}";

            return ret;
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
            Server
        }
    }
}
