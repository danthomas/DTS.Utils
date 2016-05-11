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
        public void GetStopProcessConfirmation_SingleProcess()
        {
            var args = ProcessesUtilDotArgsBuilder.New.Build();
            var context = ProcessesUtilDotContextBuilder.New
                .WithProcesses(new ProcessesUtil.IProcess[] { new Process() })
                .Build();

            var selectOptionDetails = _processesUtil.GetStopProcessConfirmation(args, ProcessesUtil.CommandType.Stop, context);
            
            Assert.That(selectOptionDetails.Message, Is.EqualTo("Stop 1 Process?"));
            Assert.That(selectOptionDetails.Options[0], Is.EqualTo("No"));
            Assert.That(selectOptionDetails.Options[1], Is.EqualTo("Yes"));
        }

        [Test]
        public void GetStopProcessConfirmation_MultipleProcesses()
        {
            var args = ProcessesUtilDotArgsBuilder.New.Build();
            var context = ProcessesUtilDotContextBuilder.New
                .WithProcesses(new ProcessesUtil.IProcess[] { new Process(), new Process() })
                .Build();

            var selectOptionDetails = _processesUtil.GetStopProcessConfirmation(args, ProcessesUtil.CommandType.Stop, context);
            
            Assert.That(selectOptionDetails.Message, Is.EqualTo("Stop 2 Processes?"));
            Assert.That(selectOptionDetails.Options[0], Is.EqualTo("No"));
            Assert.That(selectOptionDetails.Options[1], Is.EqualTo("Yes"));
        }

        class Process : ProcessesUtil.IProcess
        {
            public void Stop()
            {

            }
        }
    }
}
