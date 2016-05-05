using DTS.Utils.WindowsServices;
using NSubstitute;
using NUnit.Framework;

namespace DTS.Utils.Tests.WindowsServices
{
    public class UtilTests
    {
        private Util _util;
        private IProcessRunner _processRunner;


        [SetUp]
        public void SetUp()
        {
            _processRunner = Substitute.For<IProcessRunner>();
            _util = new Util {ProcessRunner = _processRunner};
        }

        [Test]
        [TestCase("state service", "query service", "server Server", "//Server query service")]
        [TestCase("stop service", "stop service", "server Server", "//Server stop service")]
        [TestCase("start service", "start service", "server Server", "//Server start service")]
        public void StateStartStop(string line, string noServerArgs, string serverLine, string serverArgs)
        {
            string exe = "sc.exe";
            RunProcessDetails runProcessDetails = null;

            _processRunner.Run(Arg.Do<RunProcessDetails>(x => runProcessDetails = x));

            var returnValue = _util.Execute(line);

            Assert.That(runProcessDetails.Exe, Is.EqualTo(exe));
            Assert.That(runProcessDetails.Args, Is.EqualTo(noServerArgs));

            returnValue = _util.Execute(serverLine);
            
            Assert.That(returnValue.IsSuccess, Is.True);
            
            returnValue = _util.Execute(line);
            
            Assert.That(runProcessDetails.Exe, Is.EqualTo(exe));
            Assert.That(runProcessDetails.Args, Is.EqualTo(serverArgs));
        }
    }
}
