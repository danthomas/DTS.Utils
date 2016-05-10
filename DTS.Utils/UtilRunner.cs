using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;
using DTS.Utils.ReturnValues;
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

                    ReturnValue executeReturnValue;

                    bool exitApplication = false;

                    do
                    {
                        executeReturnValue = data.Command.ExecuteFunc();

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
                            break;
                        }

                        IfReturnValue ifReturnValue = executeReturnValue as IfReturnValue;
                        RunProcessReturnValue runProcessReturnValue = executeReturnValue as RunProcessReturnValue;
                        SelectOptionReturnValue selectOptionReturnValue = executeReturnValue as SelectOptionReturnValue;
                        WriteOutputReturnValue writeOutputReturnValue = executeReturnValue as WriteOutputReturnValue;

                        if (ifReturnValue != null)
                        {
                            if (!ifReturnValue.IfDetails.If)
                            {
                                output.WriteLines(ifReturnValue.IfDetails.Message);
                                break;
                            }
                        }
                        else if (runProcessReturnValue != null)
                        {
                            executeReturnValue = _processRunner.Run(runProcessReturnValue.RunProcessDetails);

                            if (executeReturnValue.IsSuccess)
                            {
                                runProcessReturnValue.RunProcessDetails.SetOutput(executeReturnValue.Message);
                            }
                        }
                        else if (writeOutputReturnValue != null)
                        {
                            output.WriteLines(writeOutputReturnValue.Lines);
                        }
                        else if (selectOptionReturnValue != null)
                        {
                            output.WriteLines(selectOptionReturnValue.SelectOptionDetails.Message);
                            output.WriteLines(selectOptionReturnValue.SelectOptionDetails.Options.Select((x, i) => $"{i}: {x}").ToArray());
                            int response = input.ReadLine<int>();
                            selectOptionReturnValue.SelectOptionDetails.OptionSelected(selectOptionReturnValue.SelectOptionDetails.Options[response]);
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