using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public class Command<TA, TC> : ICommand where TA : class, new()
    {
        private readonly UtilBase _utilBase;
        private readonly List<ArgDef> _argDefs;
        private Func<TA, TC, ReturnValue> _func;
        private readonly string _argChar;
        private readonly string[] _truthy;
        private readonly string[] _falsy;

        internal Command(UtilBase utilBase)
        {
            _utilBase = utilBase;
            _argChar = "/";
            _truthy = new[] { "true", "t", "1" };
            _falsy = new[] { "false", "f", "0" };

            _argDefs = new List<ArgDef>();
            Actions = new List<ActionDef>();
        }

        private List<ActionDef> Actions { get; }

        public string[] Names { get; set; }

        public string ArgsDescription
        {
            get { return String.Join(", ", _argDefs.Select(x => $"{x.Name}: {x.Type.Name} {(x.Required ? " required" : "")}")); }
        }

        public Command<TA, TC> Action(TC action, string description)
        {
            Actions.Add(new ActionDef(action, description));
            Names = Actions.Select(x => x.Action.ToString().ToLower()).ToArray();
            return this;
        }

        public Command<TA, TC> Arg<P>(string name, Expression<Func<TA, P>> expression, bool required = false)
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

        public ReturnValue Execute(string[] args)
        {
            TC action = (TC)Enum.Parse(typeof(TC), args[0], true);

            TA t = new TA();

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

                    argDef.SetValue(t, value);
                }
                else if (i < args.Length)
                {
                    argDef.SetValue(t, args[i++]);
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

            return _func(t, action);
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

        public Command<TA, TC> NoOp(Func<TA, TC, ReturnValue> func)
        {
            _func = func;
            return this;
        }

        public Command<TA, TC> RunProcess(Func<TA, TC, RunProcessDetails> getRunProcessDetails, Func<TA, TC, string, ReturnValue> processOutput)
        {
            _func = (t, a) =>
            {
                var returnValue = _utilBase.RunProcess(getRunProcessDetails(t, a));

                return processOutput(t, a, returnValue.Message);
            };

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

        private class ActionDef
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
