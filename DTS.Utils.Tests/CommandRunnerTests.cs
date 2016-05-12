/*
using System;
using System.Collections.Generic;
using DTS.Utils.Core;
using NUnit.Framework;

namespace DTS.Utils.Tests
{
    public class CommandRunnerTests
    {
        private TestUtilBase _testUtilBase;

        [SetUp]
        public void SetUp()
        {
            _testUtilBase = new TestUtilBase();
        }

        [Test]
        [TestCase("command1 a true 123", "a", true, 123)]//positional
        [TestCase("command1 /s b /b false /i 456", "b", false, 456)]//named
        [TestCase("command1 cde /i 789 /b true ", "cde", true, 789)]//positional && named
        public void PositionalAndNamed(string line, string s, bool b, int i)
        {
            //var returnValue = _testUtilBase.Execute(line);
            //
            //Assert.That(returnValue.IsSuccess);
            //Assert.That(_testUtilBase.Command1Args.String, Is.EqualTo(s));
            //Assert.That(_testUtilBase.Command1Args.Bool, Is.EqualTo(b));
            //Assert.That(_testUtilBase.Command1Args.Int32, Is.EqualTo(i));
        }

        [Test]
        [TestCase("command2 /i 999 /s abc", "Required arguments not set: /b")]
        [TestCase("command2 /b /i 999", "Required arguments not set: /s")]
        [TestCase("command2 /i 999", "Required arguments not set: /s, /b")]
        public void RequiredArgumentsNotSet(string line, string message)
        {
            var returnValue = _testUtilBase.Execute(line);

            Assert.That(returnValue.IsSuccess, Is.False);
            Assert.That(returnValue.Message, Is.EqualTo(message));
        }

        [Test]
        [TestCase("command1 /b", true)]
        [TestCase("command1 /b t", true)]
        [TestCase("command1 /b true", true)]
        [TestCase("command1 /b True", true)]
        [TestCase("command1 /b 1", true)]
        [TestCase("command1 /b false", false)]
        [TestCase("command1 /b False", false)]
        [TestCase("command1 /b f", false)]
        [TestCase("command1 /b 0", false)]
        public void ValidBoolArgs(string line, bool expected)
        {
            var returnValue = _testUtilBase.Execute(line);

            Assert.That(returnValue.IsSuccess, Is.True);
            Assert.That(_testUtilBase.Command1Args.Bool, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("command1 /b xxx", "Invalid arguments : /b = xxx")]
        [TestCase("command1 /b 2", "Invalid arguments : /b = 2")]
        public void InvalidBoolArgs(string line, string message)
        {
            var returnValue = _testUtilBase.Execute(line);

            Assert.That(returnValue.IsSuccess, Is.False);
            Assert.That(returnValue.Message, Is.EqualTo(message));
        }

        [Test]
        [TestCase("command1 /i 2147483647", Int32.MaxValue)]
        [TestCase("command1 /i -1", -1)]
        [TestCase("command1 /i 0", 0)]
        [TestCase("command1 /i 1", 1)]
        [TestCase("command1 /i -2147483648", Int32.MinValue)]
        public void ValidInt32Args(string line, int expected)
        {
            var returnValue = _testUtilBase.Execute(line);

            Assert.That(returnValue.IsSuccess, Is.True);
            Assert.That(_testUtilBase.Command1Args.Int32, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("command1 /i xxx", "Invalid arguments : /i = xxx")]
        [TestCase("command1 /i 2147483648", "Invalid arguments : /i = 2147483648")]
        [TestCase("command1 /i -2147483649", "Invalid arguments : /i = -2147483649")]
        public void InvalidInt32Args(string line, string message)
        {
            var returnValue = _testUtilBase.Execute(line);

            Assert.That(returnValue.IsSuccess, Is.False);
            Assert.That(returnValue.Message, Is.EqualTo(message));
        }
    }

    internal class TestUtilBase : UtilBase
    {
        public TestUtilBase() : base(null, null)
        {
            Command<Command1Args, commandType, Context>()
                .commandType(commandType.Command1, "")
                .Arg("s", x => x.String)
                .Arg("b", x => x.Bool)
                .Arg("i", x => x.Int32)
                .NoOp((args, a, x) =>
                {
                    Command1Args = args;
                    return ReturnValue.Ok();
                });

            Command<Command2Args, commandType, Context>()
                .commandType(commandType.Command2, "")
                .Arg("s", x => x.String, true)
                .Arg("b", x => x.Bool, true)
                .Arg("i", x => x.Int32)
                .NoOp((args, a, x) =>
                {
                    Command2Args = args;
                    return ReturnValue.Ok();
                });
        }

        public Command2Args Command2Args { get; set; }

        public Command1Args Command1Args { get; set; }

        public enum commandType
        {
            Command1,
            Command2
        };
    }

    internal class Command2Args
    {
        public string String { get; set; }
        public bool Bool { get; set; }
        public int Int32 { get; set; }
    }

    class Command1Args
    {
        public string String { get; set; }
        public bool Bool { get; set; }
        public int Int32 { get; set; }
    }

    class Context
    {
    }
}
*/
