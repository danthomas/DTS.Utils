using System;
using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public class UtilRunner : UtilBase
    {
        private readonly List<UtilBase> _utils;

        public UtilRunner(IProcessRunner processRunner) : base(null, null)
        {
            ProcessRunner = processRunner;

            _utils = new List<UtilBase>();
        }

        public UtilRunner Util<T>() where T : UtilBase, new()
        {
            T t = new T { ProcessRunner = ProcessRunner };
            _utils.Add(t);
            return this;
        }

        public override ReturnValue ShowHelp(UtilBase.Args args, UtilBase.CommandType commandType)
        {
            string message = String.Join(Environment.NewLine, _utils.Select(x => $"{x.Name}: {x.Description}"));
            return ReturnValue.Ok(message);
        }

        public void Run(IInput input, IOutput output)
        {
            while (true)
            {
                var line = input.ReadLine();

                var getCommandReturnValue = GetCommand(line);

                if (getCommandReturnValue.IsSuccess)
                {
                    var executeReturnValue = getCommandReturnValue.Data.Command.Execute(getCommandReturnValue.Data.Args);

                    if (executeReturnValue.ReturnValueType == ReturnValueType.ExitApplication)
                    {
                        break;
                    }

                    output.WriteReturnValue(executeReturnValue);
                }
                else
                {
                    var util = _utils.SingleOrDefault(x => x.Name == line);

                    if (util == null)
                    {
                        output.WriteLine($"Unrecognised utility: {line}");
                    }
                    else
                    {
                        util.Run(input, output);
                    }
                }
            }
        }

        internal enum CommandType
        {
            Exit,
            Help
        }

        internal class Args
        {
        }
    }

    public interface IInput
    {
        string ReadLine();
    }
}