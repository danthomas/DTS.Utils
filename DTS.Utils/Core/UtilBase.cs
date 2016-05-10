using System;
using System.Collections.Generic;
using System.Linq;

namespace DTS.Utils.Core
{
    public abstract class UtilBase
    {
        protected readonly List<ICommand> Commands;

        protected UtilBase(string name, string description)
        {
            Name = name;
            Description = description;

            Commands = new List<ICommand>();

            Command<EmptyArgs, BaseCommandType, EmptyContext>()
              .Action(BaseCommandType.Help, "Details the available utils")
              .NoOp(ShowHelp);
        }

        private ReturnValue ShowHelp(EmptyArgs args, BaseCommandType baseCommandType, EmptyContext baseContext)
        {
            List<string> lines = new List<string>(new[] { $"{Name} commands:" });

            foreach (ICommand command in Commands)
            {
                foreach (var act in command.Acts.Where(x => x.Name != BaseCommandType.Help.ToString().ToLower()))
                {
                    lines.Add($"{act.Name}: {command.ArgsDescription} : {act.Description}");
                }
            }

            return ReturnValue.Ok(String.Join(Environment.NewLine, lines));
        }

        public string Name { get; set; }
        public string Description { get; set; }

        protected Command<A, C, X> Command<A, C, X>()
            where A : class, new()
            where C : struct
            where X : class, new()
        {
            Command<A, C, X> command = new Command<A, C, X>();

            Commands.Add(command);

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

                command = Commands.SingleOrDefault(x => x.Acts.Select(y => y.Name).Contains(name.ToLower()));
            }
            else
            {
                return ReturnValue<CommandDetails>.Error(ErrorType.CommandNotSpecified, @"Command not specified");
            }

            return command == null
                ? ReturnValue<CommandDetails>.Error(ErrorType.CommandNotRecognised, @"Command not recognised")
                : ReturnValue<CommandDetails>.Ok(new CommandDetails { Command = command, Args = args });
        }

        private class BaseContext
        {
        }

        private enum BaseCommandType
        {
            Help
        }
    }
}