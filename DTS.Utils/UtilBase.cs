using System;
using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public class UtilBase
    {
        private readonly IProcessRunner _processRunner;
        private readonly List<ICommand> _commands;

        public UtilBase(string name, IProcessRunner processRunner)
        {
            Name = name;
            _processRunner = processRunner;
            _commands = new List<ICommand>();
        }

        public string Name { get; set; }

        protected Command<T, A> Command<T, A>() where T : class, new()
        {
            Command<T, A> command = new Command<T, A>(this);

            _commands.Add(command);

            return command;
        }

        public ReturnValue Execute(string line)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            
            if (parts.Length > 0)
            {
                var name = parts[0];

                var command = _commands.SingleOrDefault(x => x.Names.Contains(name.ToLower()));

                if (command == null)
                {
                    return ReturnValue.Error($@"Command {name} not recognised");
                }

                return command.Execute(parts);
            }
            
            //ToDo : list available commands
            return ReturnValue.Error($@"Enter a command");
        }

        public ReturnValue RunProcess(RunProcessDetails runProcessDetails)
        {
            return _processRunner.Run(runProcessDetails);
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
    }
}