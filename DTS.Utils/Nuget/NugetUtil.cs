﻿using System;
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

            Command<SessionArgs, Actions, SessionContext>()
                .Action(Actions.Ses, "Sets the current _session args")
                .Arg("s", x => x.SolutionFile)
                .Arg("r", x => x.ReadSessionArgs)
                .Arg("w", x => x.WriteSessionArgs)
                .Arg("l", x => x.ListSessionArgs)
                .Arg("a", x => x.AutoLoad)
                .NoOp(SetSessionArgs);

            //Command<StateArgs, Actions>()
            //    .commandType(Actions.State, "Gets the state of a Solution")
            //    .Arg("n", x => x.Server)
            //    .NoOp(SetServer);
        }

        private class SessionContext : Core.Context<SessionArgs, Actions>
        {
        }

        private ReturnValue SetSessionArgs(SessionContext context)
        {
            if (context.Args.ReadSessionArgs)
            {
                _session = ReadSession();
            }

            if (!String.IsNullOrWhiteSpace(context.Args.SolutionFile))
            {
                _session.SolutionFile = context.Args.SolutionFile;
            }
            else if (context.Args.SolutionFile == "null")
            {
                _session = null;
            }

            if (context.Args.WriteSessionArgs)
            {
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_session));
            }

            List<string> lines = new List<string>();

            if (context.Args.ListSessionArgs)
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
