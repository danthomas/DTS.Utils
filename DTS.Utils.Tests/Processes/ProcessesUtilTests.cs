using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTS.Utils.Processes;
using DTS.Utils.Processes.Builders;
using NUnit.Framework;

namespace DTS.Utils.Tests.Processes
{
    [TestFixture]
    class ProcessesUtilTests
    {
        private ProcessesUtil _processesUtil;

        [SetUp]
        public void SetUp()
        {
            _processesUtil = new ProcessesUtil();
        }

        [Test]
        [TestCase(1, "Stop 1 Process?", new[] { "No", "Yes" })]
        [TestCase(5, "Stop 5 Processes?", new[] { "No", "Yes" })]
        public void GetStopProcessConfirmation(int noProcesses, string message, string[] options)
        {
            var args = ProcessesUtilDotArgsBuilder.New.Build();

            var context = ProcessesUtilDotContextBuilder.New
                .WithProcesses(Enumerable.Range(1, noProcesses).Select(x => new Process()).ToArray())
                .Build();

            var selectOptionDetails = _processesUtil.GetStopProcessConfirmation(args, ProcessesUtil.CommandType.Stop, context);
            
            Assert.That(selectOptionDetails.Message, Is.EqualTo(message));
            Assert.That(selectOptionDetails.Options.Length, Is.EqualTo(options.Length));

            for (var i = 0; i < options.Length; i++)
            {
                Assert.That(selectOptionDetails.Options[i], Is.EqualTo(options[i]));
            }
        }

        class Process : ProcessesUtil.IProcess
        {
            public void Stop()
            {
            }
        }
    }
}
