using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public class Command<TA, TC, TX> : ICommand
        where TA : class, new()
        where TX : class, new()
    {
        private readonly List<ArgDef> _argDefs;
        private readonly List<Func<TA, TC, TX, ReturnValue>> _funcs;
        private readonly string _argChar;
        private readonly string[] _truthy;
        private readonly string[] _falsy;
        private int _index;

        private TA _args;
        private TC _commandType;
        private TX _context;

        internal Command()
        {
            _funcs = new List<Func<TA, TC, TX, ReturnValue>>();
            _argChar = "/";
            _truthy = new[] { "true", "t", "1" };
            _falsy = new[] { "false", "f", "0" };

            _argDefs = new List<ArgDef>();
            Actions = new List<ActionDef>();
        }

        internal List<ActionDef> Actions { get; }

        public string ArgsDescription
        {
            get { return String.Join(", ", _argDefs.Select(x => $"{x.Name}: {x.Type.Name} {(x.Required ? " required" : "")}")); }
        }

        public Command<TA, TC, TX> Action(TC action, string description)
        {
            Actions.Add(new ActionDef(action, description));
            Acts = Actions.Select(x => new Act( x.Action.ToString().ToLower(), x.Description)).ToArray();
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
            TC commandType = (TC)Enum.Parse(typeof(TC), args[0], true);

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

            ReturnValue returnValue = null;

            _context = new TX();

            _args = a;

            _commandType = commandType;
            
            _index = 0;

            return returnValue;
        }

        public ReturnValue ExecuteFunc()
        {
            if (_index < _funcs.Count)
            {
                return _funcs[_index++](_args, _commandType, _context);
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

        public Command<TA, TC, TX> NoOp(Func<TA, TC, TX, ReturnValue> func)
        {
            _funcs.Add(func);
            return this;
        }

        public Command<TA, TC, TX> RunProcess(Func<TA, TC, TX, RunProcessDetails> getRunProcessDetails)
        {
            _funcs.Add((t, c, x) => new RunProcessReturnValue(getRunProcessDetails(t, c, x)));

            return this;
        }


        public Command<TA, TC, TX> WriteOutput(Func<TA, TC, TX, WriteOutputReturnValue> func)
        {
            _funcs.Add(func);

            return this;
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

        internal class ActionDef
        {
            public ActionDef(TC action, string description)
            {
                Action = action;
                Name = action.ToString().ToLower();
                Description = description;
            }

            public TC Action { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
