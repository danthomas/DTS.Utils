using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTS.Utils.Processes;
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
        public void Test()
        {
            var selectOptionDetails = _processesUtil.GetStopProcessConfirmation(new ProcessesUtil.Args(), ProcessesUtil.CommandType.Stop, new ProcessesUtil.Context());

            Assert.That(selectOptionDetails.Message, Is.EqualTo(""));
        }
    }
}
