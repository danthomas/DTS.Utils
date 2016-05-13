using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DTS.Utils.Details;
using DTS.Utils.Processes;

namespace DTS.Utils.Core
{
    public class Command<TA, TC, TX> : ICommand
        where TA : class, new()
        where TX : Context<TA, TC>, new()
    {
        private readonly List<ArgDef> _argDefs;
        private readonly List<Func<TX, ReturnValue>> _funcs;
        private readonly string _argChar;
        private readonly string[] _truthy;
        private readonly string[] _falsy;
        private int _index;

        private TA _args;
        private TC _commandType;
        private TX _context;

        internal Command()
        {
            _funcs = new List<Func<TX, ReturnValue>>();
            _argChar = "/";
            _truthy = new[] { "true", "t", "1" };
            _falsy = new[] { "false", "f", "0" };

            _argDefs = new List<ArgDef>();
            Actions = new List<Operation>();
        }

        internal List<Operation> Actions { get; }

        public string ArgsDescription
        {
            get { return String.Join(", ", _argDefs.Select(x => $"{x.Name}: {x.Type.Name} {(x.Required ? " required" : "")}")); }
        }

        public Command<TA, TC, TX> Action(TC commandType, string description)
        {
            Actions.Add(new Operation(commandType, description));
            Acts = Actions.Select(x => new Act(x.CommandType.ToString().ToLower(), x.Description)).ToArray();
            return this;
        }

        public Act[] Acts { get; set; }

        public Command<TA, TC, TX> Arg<P>(string name, Expression<Func<TA, P>> expression, bool required = false)
        {
            var member = ((MemberExpression)expression.Body).Member;

            ParameterExpression sourceParam = Expression.Parameter(typeof(TA));
            ParameterExpression valueParam = Expression.Parameter(typeof(string));

            Expression propertyExpression = Expression.Property(sourceParam, member.Name, null);
            var type = propertyExpression.Type;

            List<ParameterExpression> variables = new List<ParameterExpression>();
            List<Expression> expressions = new List<Expression>();

            if (type == typeof(string))
            {
                expressions.Add(Expression.Assign(propertyExpression, valueParam));
                expressions.Add(Expression.Constant(true));
            }
            else
            {
                var tryParseMethod = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });

                if (tryParseMethod != null)
                {
                    ParameterExpression variable = Expression.Variable(type);
                    variables.Add(variable);
                    ParameterExpression ret = Expression.Variable(typeof(bool));
                    variables.Add(ret);
                    expressions.Add(Expression.Assign(ret, Expression.Call(tryParseMethod, valueParam, variable)));
                    expressions.Add(Expression.IfThen(ret, Expression.Assign(propertyExpression, variable)));
                    expressions.Add(ret);
                }
                else
                {
                    throw new Exception($@"Unsupported Type {type.FullName}");
                }
            }

            _argDefs.Add(new ArgDef(_argChar + name, required, type, Expression
                .Lambda<Func<TA, string, bool>>(Expression.Block(variables, expressions), sourceParam, valueParam)
                .Compile()));

            return this;
        }

        public ReturnValue Init(string[] args)
        {
            TA a = new TA();

            _argDefs.ForEach(x =>
            {
                x.State = State.NotSet;
                x.Value = null;
            });

            bool positional = true;

            args = args.Skip(1).ToArray();

            for (int i = 0; i < args.Length;)
            {
                var argDef = _argDefs.SingleOrDefault(x => x.Name == args[i]);

                if (argDef != null)
                {
                    i++;
                    positional = false;
                }
                else if (positional)
                {
                    argDef = _argDefs[i];
                }
                else
                {
                    throw new Exception();
                }

                if (argDef.Type == typeof(bool))
                {
                    var value = i < args.Length && !args[i].StartsWith(_argChar)
                        ? GetBoolValue(args[i++])
                        : true.ToString();

                    argDef.SetValue(a, value);
                }
                else if (i < args.Length)
                {
                    argDef.SetValue(a, args[i++]);
                }
                else
                {
                    return ReturnValue.Error(ErrorType.ValueExpected, $"Value expected for argument {argDef.Name}");
                }
            }

            var requiredArgsWithNoValue = _argDefs.Where(x => x.Required && x.State == State.NotSet).ToArray();

            if (requiredArgsWithNoValue.Any())
            {
                return ReturnValue.Error(ErrorType.RequiredArgumentsMissing,
                    $@"Required arguments not set: {String.Join(", ", requiredArgsWithNoValue.Select(x => x.Name))}");
            }

            var invalidArgs = _argDefs.Where(x => x.State == State.Invalid).ToArray();

            if (invalidArgs.Any())
            {
                return ReturnValue.Error(ErrorType.InvalidArguments,
                    $@"Invalid arguments : {String.Join(", ", invalidArgs.Select(x => $"{x.Name} = {x.Value}"))}");
            }

            _context = new TX
            {
                Args = a,
                CommandType = (TC)Enum.Parse(typeof(TC), args[0], true)
            };

            _index = 0;

            return ReturnValue.Ok();
        }

        public ReturnValue ExecuteFunc()
        {
            if (_index < _funcs.Count)
            {
                return _funcs[_index++](_context);
            }

            //ToDo   dont return an error
            return ReturnValue.Error(ErrorType.EndOfList, "");
        }

        private string GetBoolValue(string value)
        {
            string ret = value;
            if (_truthy.Contains(value))
            {
                ret = true.ToString();
            }
            else if (_falsy.Contains(value))
            {
                ret = false.ToString();
            }

            return ret;
        }

        public Command<TA, TC, TX> NoOp(Func<TX, ReturnValue> func)
        {
            _funcs.Add(func);
            return this;
        }

        public Command<TA, TC, TX> If(Func<TX, IfDetails> func)
        {
            //_funcs.Add((t, c, x) => ReturnValue<IfDetails>.Ok(getIfDetails(t, c, x), ReturnValueType.If));
            AddFunc(func, ReturnValueType.If);
            return this;
        }

        public Command<TA, TC, TX> GetProcesses(Action<IProcess[], TX> action = null)
        {
            //ToDo: pass expression to set propery
            _funcs.Add(c => ReturnValue<GetProcessesAction>.Ok(new GetProcessesAction
            {
                Action = processes =>
                {
                    action?.Invoke(processes, c);
                }
            }, ReturnValueType.GetProcesses));
            return this;
        }

        public Command<TA, TC, TX> RunProcess(Func<TX, RunProcessDetails> getRunProcessDetails, Action<string, TX> action = null)
        {
            _funcs.Add(c =>
            {
                Action<string> action1 = null;
                
                if (action != null)
                {
                    action1 = a =>
                    {
                        action.Invoke(a, c);
                    };
                }

                return ReturnValue<RunProcessAction>.Ok(new RunProcessAction
                {
                    RunProcessDetails = getRunProcessDetails(c),
                    Action = action1
                }, ReturnValueType.RunProcess);
            }
                );

            return this;
        }

        public Command<TA, TC, TX> LoadAssembly(Func<TX, string> func, Action<Assembly, TX> action)
        {
            _funcs.Add(c => ReturnValue<LoadAssemblyAction>.Ok(new LoadAssemblyAction
            {
                FilePath = func(c),
                Action = a => action(a, c)
            }, ReturnValueType.LoadAssembly));
            return this;
        }

        public Command<TA, TC, TX> IfSelectOption(Func<TX, IfSelectOptionAction> func)
        {
            _funcs.Add(c => ReturnValue<IfSelectOptionAction>.Ok(func(c), ReturnValueType.SelectOption));
            //AddFunc(func, ReturnValueType.IfSelectOption);
            return this;
        }

        public Command<TA, TC, TX> SelectOption(Func<TX, SelectOptionAction> func)
        {
            //_funcs.Add((t, c, x) => ReturnValue<SelectOptionAction>.Ok(func(t, c, x), ReturnValueType.SelectOption));
            AddFunc(func, ReturnValueType.SelectOption);
            return this;
        }

        public Command<TA, TC, TX> WriteOutput(Func<TX, IEnumerable<string>> func)
        {
            //_funcs.Add((t, c, x) => ReturnValue<IEnumerable<string>>.Ok(func(t, c, x).ToArray(), ReturnValueType.WriteOutput));
            AddFunc(func, ReturnValueType.WriteOutput);
            return this;
        }

        public Command<TA, TC, TX> WriteFiles(Func<TX, WriteFilesAction> func)
        {
            AddFunc(func, ReturnValueType.WriteFiles);
            return this;
        }

        private void AddFunc<T>(Func<TX, T> func, ReturnValueType returnValueType)
        {
            _funcs.Add(c => ReturnValue<T>.Ok(func(c), returnValueType));
        }

        private class ArgDef
        {
            public string Name { get; }
            public bool Required { get; }
            public Type Type { get; }
            private Func<TA, string, bool> Func { get; }
            public State State { get; set; }

            public ArgDef(string name, bool required, Type type, Func<TA, string, bool> func)
            {
                Name = name;
                Required = required;
                Type = type;
                Func = func;
            }

            public void SetValue(TA t, string value)
            {
                Value = value;
                State = Func(t, value)
                    ? State.Set
                    : State.Invalid;
            }

            public string Value { get; set; }
        }

        private enum State
        {
            NotSet,
            Set,
            Invalid
        }

        internal class Operation
        {
            public Operation(TC commandType, string description)
            {
                CommandType = commandType;
                Name = commandType.ToString().ToLower();
                Description = description;
            }

            public TC CommandType { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
