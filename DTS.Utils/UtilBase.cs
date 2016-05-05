using System;
using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public abstract class UtilBase
    {
        private readonly List<ICommand> _commands;

        public UtilBase(string name, string description)
        {
            Name = name;
            Description = description;
            _commands = new List<ICommand>();

            Command<Args, CommandType>()
              .Action(CommandType.Help, "Details the available utils")
              .NoOp(ShowHelp);

            Command<Args, CommandType>()
              .Action(CommandType.Exit, "Exit the application")
              .NoOp(Exit);
        }

        public virtual ReturnValue ShowHelp(Args args, CommandType commandType)
        {
            List<string> lines = new List<string>(new[] { $"{Name} commands:" });

            foreach (ICommand command in _commands)
            {
               // command.
                foreach (var name in command.Names)
                {
                    lines.Add($"{name}");
                }
            }

            return ReturnValue.Ok(String.Join(Environment.NewLine, _commands.SelectMany(x => x.Names).Select(x => x)));
        }

        private ReturnValue Exit(Args arg1, CommandType arg2)
        {
            return new ExitAppReturnValue();
        }

        public enum CommandType
        {
            Exit,
            Help
        }

        public class Args
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        internal IProcessRunner ProcessRunner { get; set; }

        protected Command<A, C> Command<A, C>()
            where A : class, new()
            where C : struct
        {
            Command<A, C> command = new Command<A, C>(this);

            _commands.Add(command);

            return command;
        }

        public ReturnValue Execute(string line)
        {
            var returnValue = GetCommand(line);

            if (returnValue.IsSuccess)
            {
                return returnValue.Data.Command.Execute(returnValue.Data.Args);
            }

            return returnValue;
        }

        public ReturnValue RunProcess(RunProcessDetails runProcessDetails)
        {
            return ProcessRunner.Run(runProcessDetails);
        }

        public void Run(IInput input, IOutput output)
        {
            while (true)
            {
                string line = input.ReadLine();


                if (line == "exit")
                {
                    break;
                }

                output.WriteReturnValue(Execute(line));
            }
        }

        protected ReturnValue<CommandDetails> GetCommand(string line)
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

        protected class CommandDetails
        {
            public ICommand Command { get; set; }
            public string[] Args { get; set; }
        }
    }
}