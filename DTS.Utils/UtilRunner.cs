using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DTS.Utils.Core;
using DTS.Utils.Details;
using DTS.Utils.Processes;
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
            bool readLines = true;

            while (readLines)
            {
                var line = input.ReadLine<string>();

                if (TrySetCurrentUtil(output, line))
                    continue;

                var getCommandReturnValue = GetCommand(line);

                if (getCommandReturnValue.IsSuccess)
                {
                    CommandDetails data = getCommandReturnValue.Data;

                   var initReturnValue = data.Command.Init(data.Args);

                    if (initReturnValue.IsSuccess)
                    {
                        bool runActions = true;

                        while (runActions)
                        {
                            var executeReturnValue = data.Command.ExecuteFunc();

                            if (executeReturnValue.ErrorType == ErrorType.EndOfList)
                            {
                                break;
                            }

                            switch (executeReturnValue.ReturnValueType)
                            {
                                case ReturnValueType.Clear:
                                    output.Clear();
                                    break;
                                case ReturnValueType.ExitApplication:
                                    readLines = false;
                                    break;
                                case ReturnValueType.RunProcess:
                                    RunProcess(executeReturnValue);
                                    break;
                                case ReturnValueType.WriteFiles:
                                    WriteFiles(executeReturnValue);
                                    break;
                                case ReturnValueType.If:
                                    runActions = If(output, executeReturnValue);
                                    break;
                                case ReturnValueType.IfSelectOption:
                                    SelectOption(input, output, executeReturnValue);
                                    break;
                                case ReturnValueType.SelectOption:
                                    runActions = IfSelectOption(input, output, executeReturnValue);
                                    break;
                                case ReturnValueType.WriteOutput:
                                    WriteOutput(output, executeReturnValue);
                                    break;
                                case ReturnValueType.LoadAssembly:
                                    runActions = LoadAssembly(executeReturnValue);
                                    break;
                                case ReturnValueType.GetProcesses:
                                    GetProcesses(executeReturnValue);
                                    break;
                                default:
                                    output.WriteReturnValue(executeReturnValue);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        output.WriteReturnValue(initReturnValue);
                    }
                }
                else
                {
                    output.WriteLines($"Util or Command {line} not recognised");
                }
            }
        }

        private void GetProcesses(ReturnValue returnValue)
        {
            var getProcessesAction = ((ReturnValue<GetProcessesAction>)returnValue).Data;

            getProcessesAction.Action(Process.GetProcesses().Select(x => new ProcessWrapper(x)).Cast<IProcess>().ToArray());
        }

        private bool LoadAssembly(ReturnValue returnValue)
        {
            var loadAssemblyAction = ((ReturnValue<LoadAssemblyAction>)returnValue).Data;
            if (File.Exists(loadAssemblyAction.FilePath))
            {
                try
                {
                    loadAssemblyAction.Action(Assembly.LoadFile(loadAssemblyAction.FilePath));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        private static void WriteOutput(IOutput output, ReturnValue executeReturnValue)
        {
            var lines = ((ReturnValue<IEnumerable<string>>) executeReturnValue).Data;
            output.WriteLines(lines.ToArray());
        }

        private static void SelectOption(IInput input, IOutput output, ReturnValue executeReturnValue)
        {
            var selectOptionDetails = ((ReturnValue<SelectOptionAction>) executeReturnValue).Data;
            output.WriteLines(selectOptionDetails.Message);
            output.WriteLines(selectOptionDetails.Options.Select((x, i) => $"{i}: {x}").ToArray());
            int response = input.ReadLine<int>();
            selectOptionDetails.OptionSelected(selectOptionDetails.Options[response]);
        }
        
        private static bool IfSelectOption(IInput input, IOutput output, ReturnValue executeReturnValue)
        {
            var ifSelectOptionAction = ((ReturnValue<IfSelectOptionAction>)executeReturnValue).Data;
            output.WriteLines(ifSelectOptionAction.Message);
            output.WriteLines(ifSelectOptionAction.Options.Select((x, i) => $"{i}: {x}").ToArray());
            int response = input.ReadLine<int>();
            return ifSelectOptionAction.Options[response] == ifSelectOptionAction.Option;
        }

        private static bool If(IOutput output, ReturnValue executeReturnValue)
        {
            var ifDetails = ((ReturnValue<IfDetails>) executeReturnValue).Data;
            if (!ifDetails.If)
            {
                output.WriteLines(ifDetails.Message);
                return false;
            }
            return true;
        }

        private static void WriteFiles(ReturnValue executeReturnValue)
        {
            var writeFilesDetails = ((ReturnValue<WriteFilesAction>) executeReturnValue).Data;

            foreach (var genFile in writeFilesDetails.GenFiles)
            {
                string filePath = Path.Combine(writeFilesDetails.DirPath, genFile.RelativeFilePath);
                string dirPath = Path.GetDirectoryName(filePath);

                Directory.CreateDirectory(dirPath);
                File.WriteAllText(filePath, genFile.Text);
            }
        }

        private void RunProcess(ReturnValue executeReturnValue)
        {
            var runProcessAction = ((ReturnValue<RunProcessAction>) executeReturnValue).Data;

            executeReturnValue = _processRunner.Run(runProcessAction);

            if (executeReturnValue.IsSuccess)
            {
                runProcessAction.Action?.Invoke(executeReturnValue.Message);
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