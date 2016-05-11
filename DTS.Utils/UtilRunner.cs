using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;
using DTS.Utils.Details;
using DTS.Utils.Runner;

namespace DTS.Utils
{
    public class UtilRunner
    {
        private readonly IProcessRunner _processRunner;
        private readonly List<UtilBase> _utils;
        private readonly RunnerUtil _runnerUtil;
        private UtilBase _currentUtil;

        public UtilRunner(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
            _utils = new List<UtilBase>();
            _currentUtil = _runnerUtil = new RunnerUtil();
        }

        public UtilRunner Util<T>() where T : UtilBase, new()
        {
            T t = new T();
            _utils.Add(t);
            return this;
        }

        public void Run(IInput input, IOutput output)
        {
            while (true)
            {
                var line = input.ReadLine<string>();

                if (TrySetCurrentUtil(output, line))
                    continue;

                var getCommandReturnValue = GetCommand(line);

                if (getCommandReturnValue.IsSuccess)
                {
                    CommandDetails data = getCommandReturnValue.Data;

                    data.Command.Init(data.Args);

                    bool exitApplication = false;

                    do
                    {
                        var executeReturnValue = data.Command.ExecuteFunc();

                        if (executeReturnValue.ErrorType == ErrorType.EndOfList)
                        {
                            break;
                        }


                        if (executeReturnValue.ReturnValueType == ReturnValueType.ExitApplication)
                        {
                            exitApplication = true;
                            break;
                        }

                        if (executeReturnValue.ReturnValueType == ReturnValueType.Clear)
                        {
                            output.Clear();
                        }
                        else if (executeReturnValue.ReturnValueType == ReturnValueType.If)
                        {
                            var ifDetails = ((ReturnValue<IfDetails>) executeReturnValue).Data;
                            if (!ifDetails.If)
                            {
                                output.WriteLines(ifDetails.Message);
                                break;
                            }
                        }
                        else if (executeReturnValue.ReturnValueType == ReturnValueType.RunProcess)
                        {
                            var runProcessDetails = ((ReturnValue<RunProcessDetails>)executeReturnValue).Data;

                            executeReturnValue = _processRunner.Run(runProcessDetails);

                            if (executeReturnValue.IsSuccess)
                            {
                                runProcessDetails.SetOutput(executeReturnValue.Message);
                            }
                        }
                        else if (executeReturnValue.ReturnValueType == ReturnValueType.WriteOutput)
                        {
                            var lines = ((ReturnValue<IEnumerable<string>>)executeReturnValue).Data;
                            output.WriteLines(lines.ToArray());
                        }
                        else if (executeReturnValue.ReturnValueType == ReturnValueType.SelectOption)
                        {
                            var selectOptionDetails = ((ReturnValue<SelectOptionDetails>)executeReturnValue).Data;
                            output.WriteLines(selectOptionDetails.Message);
                            output.WriteLines(selectOptionDetails.Options.Select((x, i) => $"{i}: {x}").ToArray());
                            int response = input.ReadLine<int>();
                            selectOptionDetails.OptionSelected(selectOptionDetails.Options[response]);
                        }
                        else
                        {
                            output.WriteReturnValue(executeReturnValue);
                        }

                    } while (true);

                    if (exitApplication)
                    {
                        break;
                    }
                }
                else
                {
                    output.WriteLines($"Util or Command {line} not recognised");
                }
            }
        }

        private bool TrySetCurrentUtil(IOutput output, string line)
        {
            var util = GetUtil(line);

            if (util != null)
            {
                _currentUtil = util;
                output.WriteLines($"--->{util.Name}: {util.Description}");
                return true;
            }
            return false;
        }

        private ReturnValue<CommandDetails> GetCommand(string line)
        {
            var returnValue = _currentUtil.GetCommand(line);

            if (!returnValue.IsSuccess)
            {
                returnValue = _runnerUtil.GetCommand(line);
            }

            return returnValue;
        }

        private UtilBase GetUtil(string line)
        {
            return _utils.FirstOrDefault(x => x.Name == line);
        }
    }
}