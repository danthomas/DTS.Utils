using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public abstract class UtilBase
    {
        private readonly List<ICommand> _commands;

        protected UtilBase(string name, string description)
        {
            Name = name;
            Description = description;

            _commands = new List<ICommand>();
        }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProcessOutput { get; set; }

        protected Command<A, C, X> Command<A, C, X>()
            where A : class, new()
            where C : struct
            where X : class, new()
        {
            Command<A, C, X> command = new Command<A, C, X>();

            _commands.Add(command);

            return command;
        }

        public ReturnValue<CommandDetails> GetCommand(string line)
        {
            string[] args = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            ICommand command;

            string name;

            if (args.Length > 0)
            {
                name = args[0];

                command = _commands.SingleOrDefault(x => x.Names.Contains(name.ToLower()));
            }
            else
            {
                return ReturnValue<CommandDetails>.Error(ErrorType.CommandNotSpecified, @"Command not specified");
            }

            return command == null
                ? ReturnValue<CommandDetails>.Error(ErrorType.CommandNotRecognised, @"Command not recognised")
                : ReturnValue<CommandDetails>.Ok(new CommandDetails { Command = command, Args = args });
        }
    }
}