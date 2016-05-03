﻿using System;
using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
   public class CommandRunner
    {
       private readonly IRunner _runner;
       private readonly List<ICommand> _commands;

        public CommandRunner(IRunner runner)
        {
            _runner = runner;
            _commands = new List<ICommand>();
        }

       protected Command<T> Command<T>(string name) where T : class, new()
        {
            Command<T> command = new Command<T>(this, name);

            _commands.Add(command);

            return command;
        }

        public ReturnValue Execute(string line)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var command = _commands.SingleOrDefault(x => x.Name == parts[0]);

            if (command == null)
            {
                return ReturnValue.Error($@"Command {parts[0]} not recognised");
            }

            return command.Execute(parts.Skip(1).ToArray());
        }

       public ReturnValue Run(RunDetails runDetails)
       {
           return _runner.Run(runDetails);
       }
    }
}