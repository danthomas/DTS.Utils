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
            Command<SessionArgs>("server")
                .Arg("n", x => x.Server)
                .NoOp(x =>
                {
                    _server = x.Server;
                    return ReturnValue.Ok();
                });

            Command<StateArgs>("state")
                .Arg("n", x => x.Service)
                .Arg("s", x => x.Server)
                .Run(x => new RunDetails
                {
                    Exe = "sc.exe",
                    Args = GetStateArgs(x)
                });
        }

        private string GetStateArgs(StateArgs x)
        {
            string server = String.IsNullOrWhiteSpace(x.Server) ?_server  : x.Server;

            string ret = String.IsNullOrWhiteSpace(server) ? "" : "//" + server + " ";

            ret += x.Service;

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
    }
}
