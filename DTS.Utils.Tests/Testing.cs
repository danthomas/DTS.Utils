using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MPD.ServiceDirectory.UnitTests
{
    [TestFixture]
    class Testing : IOutput, IFileSystem
    {
        private string _text;
        private List<FileDef> _fileDefs;

        [SetUp]
        public void SetUp()
        {
            _text = "";
            _fileDefs = new List<FileDef>();
        }

        [Test]
        [TestCase("xxx", "yyy", "xxxyyy")]
        [TestCase("yyy", "zzz", "yyyzzz")]
        public void WriteText(string first, string second, string expected)
        {
            var command = new Command()
                .NoOp(x => { x.Text = first; })
                .WriteText(WriteContext)
                .NoOp(x => { x.Text = second; })
                .WriteText(WriteContext);

            command.Invoke(this, this);

            Assert.That(_text, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(true, "xxx")]
        [TestCase(false, "")]
        public void IfThen(bool confirm, string expected)
        {
            var command = new Command()
                .IfThen(c => confirm, c =>
                    c.WriteText("xxx"));

            command.Invoke(this, this);

            Assert.That(_text, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(true, "xxx")]
        [TestCase(false, "yyy")]
        public void IfThenElse(bool confirm, string expected)
        {
            var command = new Command()
                .IfThenElse(c => confirm,
                    c => c.WriteText("xxx"),
                    c => c.WriteText("yyy"));

            command.Invoke(this, this);

            Assert.That(_text, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(true, true, "abcde")]
        [TestCase(true, false, "abcd")]
        [TestCase(false, true, "abghe")]
        [TestCase(false, false, "abgh")]
        public void IfThenElse(bool ifThenElse, bool @if, string expected)
        {
            var command = new Command()
                .WriteText("a")
                .WriteText("b")
                .IfThenElse(c => ifThenElse, 
                    c => c
                        .WriteText("c")
                        .WriteText("d")
                    , c => c
                        .WriteText("f")
                        .WriteText("g")
                    )
                .ContinueIf(c => @if)
                .WriteText("e");

            command.Invoke(this, this);

            Assert.That(_text, Is.EqualTo(expected));
        }

        [Test]
        public void WriteFiles()
        {
            var command = new Command()
                .WriteFiles(x => new[]
                {
                    new FileDef("a", "A"),
                    new FileDef("b", "B"),
                    new FileDef("c", "C")
                });

            command.Invoke(this, this);

            Assert.That(_fileDefs.Count, Is.EqualTo(3));
        }

        private string WriteContext(Context context)
        {
            return context.Text;
        }

        public void WriteText(string text)
        {
            _text += text;
        }

        public void WriteFile(FileDef fileDef)
        {
            _fileDefs.Add(fileDef);
        }
    }

    class Command
    {
        private readonly List<Func<Context, Operation>> _funcs;

        public Command()
        {
            _funcs = new List<Func<Context, Operation>>();
        }

        public Command Action(Func<Context, Operation> func)
        {
            _funcs.Add(func);
            return this;
        }

        public Command NoOp(Action<Context> action)
        {
            _funcs.Add(c =>
            {
                action(c);
                return new NoOperation();
            });
            return this;
        }

        public Command WriteFiles(Func<Context, IEnumerable<FileDef>> func)
        {
            _funcs.Add(c => new WriteFilesOperation(func(c)));
            return this;
        }

        public Command WriteText(Func<Context, string> func)
        {
            _funcs.Add(c => new WriteTextOperation(func(c)));
            return this;
        }

        public Command WriteText(string text)
        {
            _funcs.Add(c => new WriteTextOperation(text));
            return this;
        }

        public Command ContinueIf(Func<Context, bool> predicate)
        {
            Func<Context, ContinueIfOperation> func = c => new ContinueIfOperation(predicate);
            _funcs.Add(func);
            return this;
        }

        public Command IfThen(Func<Context, bool> predicate, Action<Command> trueAction)
        {
            Func<Context, IfThenElseOperation> func = c => new IfThenElseOperation(predicate, trueAction);
            _funcs.Add(func);
            return this;
        }

        public Command IfThenElse(Func<Context, bool> predicate, Action<Command> trueAction, Action<Command> falseAction)
        {
            Func<Context, IfThenElseOperation> func = c => new IfThenElseOperation(predicate, trueAction, falseAction);
            _funcs.Add(func);
            return this;
        }

        public void Invoke(IOutput output, IFileSystem fileSystem)
        {
            Context context = new Context();

            foreach (var func in _funcs)
            {
                var operation = func(context);

                if (operation is WriteTextOperation)
                {
                    WriteTextOperation writeTextOperation = (WriteTextOperation)operation;
                    output.WriteText(writeTextOperation.Text);
                }
                else if (operation is ContinueIfOperation)
                {
                    var ifOperation = (ContinueIfOperation)operation;
                    if (!ifOperation.IfFunc(context))
                    {
                        break;
                    }
                }
                else if (operation is IfThenElseOperation)
                {
                    var ifThenOperation = (IfThenElseOperation)operation;
                    if (ifThenOperation.IfFunc(context))
                    {
                        ifThenOperation.TrueCommand.Invoke(output, fileSystem);
                    }
                    else
                    {
                        ifThenOperation.FalseCommand?.Invoke(output, fileSystem);
                    }
                }
                else if (operation is WriteFilesOperation)
                {
                    var writeFilesOperation = (WriteFilesOperation)operation;
                    foreach (var fileDef in writeFilesOperation.FileDefs)
                    {
                        fileSystem.WriteFile(fileDef);
                    }
                }
            }
        }
    }

    internal interface IFileSystem
    {
        void WriteFile(FileDef fileDef);
    }

    internal class FileDef
    {
        public FileDef(string path, string content)
        {
            Path = path;
            Content = content;
        }

        public string Path { get; set; }
        public string Content { get; set; }
    }

    internal class Context
    {
        public string Text { get; set; }
    }

    internal interface IOutput
    {
        void WriteText(string text);
    }

    abstract class Operation
    {
    }

    class NoOperation : Operation
    {

    }

    class WriteFilesOperation : Operation
    {
        public IEnumerable<FileDef> FileDefs { get; set; }

        public WriteFilesOperation(IEnumerable<FileDef> fileDefs)
        {
            FileDefs = fileDefs;
        }
    }

    class WriteTextOperation : Operation
    {
        public string Text { get; set; }

        public WriteTextOperation(string text)
        {
            Text = text;
        }
    }

    class ContinueIfOperation : Operation
    {
        public ContinueIfOperation(Func<Context, bool> ifFunc)
        {
            IfFunc = ifFunc;
        }

        public Func<Context, bool> IfFunc { get; set; }
    }

    class IfThenElseOperation : Operation
    {
        public IfThenElseOperation(Func<Context, bool> ifFunc, Action<Command> trueFunc, Action<Command> falseFunc = null)
        {
            IfFunc = ifFunc;
            TrueCommand = new Command();
            trueFunc(TrueCommand);

            if (falseFunc != null)
            {
                FalseCommand = new Command();
                falseFunc(FalseCommand);

            }
        }

        public Func<Context, bool> IfFunc { get; set; }
        public Command TrueCommand { get; set; }
        public Command FalseCommand { get; set; }
    }
}
