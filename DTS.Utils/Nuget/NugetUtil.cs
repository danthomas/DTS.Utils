using System;
using System.Collections.Generic;
using System.IO;
using DTS.Utils.Core;
using Newtonsoft.Json;

namespace DTS.Utils.Nuget
{
    public class NugetUtil : UtilBase
    {
        private Session _session;
        private readonly string _filePath;

        public NugetUtil() : base("nuget", "Nuget utils")
        {
            _filePath = $@"C:\temp\nuget.session.json";

            _session = ReadSession();

            if (_session == null || !_session.AutoLoad)
            {
                _session = new Session();
            }

            Command<SessionArgs, Actions, Context>()
                .Action(Actions.Ses, "Sets the current _session args")
                .Arg("s", x => x.SolutionFile)
                .Arg("r", x => x.ReadSessionArgs)
                .Arg("w", x => x.WriteSessionArgs)
                .Arg("l", x => x.ListSessionArgs)
                .Arg("a", x => x.AutoLoad)
                .NoOp(SetSessionArgs);

            //Command<StateArgs, Actions>()
            //    .Action(Actions.State, "Gets the state of a Solution")
            //    .Arg("n", x => x.Server)
            //    .NoOp(SetServer);
        }

        public class Context
        {
        }

        private ReturnValue SetSessionArgs(SessionArgs args, Actions action, Context context)
        {
            if (args.ReadSessionArgs)
            {
                _session = ReadSession();
            }

            if (!String.IsNullOrWhiteSpace(args.SolutionFile))
            {
                _session.SolutionFile = args.SolutionFile;
            }
            else if (args.SolutionFile == "null")
            {
                _session = null;
            }

            if (args.WriteSessionArgs)
            {
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_session));
            }

            List<string> lines = new List<string>();

            if (args.ListSessionArgs)
            {
                lines.Add("Solution File: " + _session.SolutionFile);
            }

            return ReturnValue.Ok(String.Join(Environment.NewLine, lines));
        }

        private Session ReadSession()
        {
            if (File.Exists(_filePath))
            {
                return JsonConvert.DeserializeObject<Session>(File.ReadAllText(_filePath));
            }

            return null;
        }

        private class Session
        {
            public string SolutionFile { get; set; }
            public bool AutoLoad { get; set; }
        }

        public class SessionArgs
        {
            public string SolutionFile { get; set; }
            public bool ReadSessionArgs { get; set; }
            public bool ListSessionArgs { get; set; }
            public bool WriteSessionArgs { get; set; }
            public bool AutoLoad { get; set; }
        }

        public class StateArgs
        {
        }

        enum Actions
        {
            Ses
        }
    }
}
