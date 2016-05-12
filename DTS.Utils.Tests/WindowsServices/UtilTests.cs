/*
using DTS.Utils.WindowsServices;
using NSubstitute;
using NUnit.Framework;

namespace DTS.Utils.Tests.WindowsServices
{
    public class UtilTests
    {
        private WindowsServiceUtil _windowsServiceUtil;
        private IProcessRunner _processRunner;


        [SetUp]
        public void SetUp()
        {
            _processRunner = Substitute.For<IProcessRunner>();
            _windowsServiceUtil = new WindowsServiceUtil();
        }

        [Test]
        [TestCase("state service", "query service", "server Server", "//Server query service")]
        [TestCase("stop service", "stop service", "server Server", "//Server stop service")]
        [TestCase("start service", "start service", "server Server", "//Server start service")]
        public void StateStartStop(string line, string noServerArgs, string serverLine, string serverArgs)
        {
            string exe = "sc.exe";
            StartProcessAction StartProcessAction = null;

            _processRunner.Run(Arg.Do<StartProcessAction>(x => StartProcessAction = x));

            var returnValue = _windowsServiceUtil.Execute(line);

            Assert.That(StartProcessAction.Exe, Is.EqualTo(exe));
            Assert.That(StartProcessAction.Args, Is.EqualTo(noServerArgs));

            returnValue = _windowsServiceUtil.Execute(serverLine);
            
            Assert.That(returnValue.IsSuccess, Is.True);
            
            returnValue = _windowsServiceUtil.Execute(line);
            
            Assert.That(StartProcessAction.Exe, Is.EqualTo(exe));
            Assert.That(StartProcessAction.Args, Is.EqualTo(serverArgs));
        }
    }
}
*/
