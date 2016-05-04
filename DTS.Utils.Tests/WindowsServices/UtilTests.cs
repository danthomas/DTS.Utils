using DTS.Utils.WindowsServices;
using NSubstitute;
using NUnit.Framework;

namespace DTS.Utils.Tests.WindowsServices
{
    public class UtilTests
    {
        private Util _util;
        private IRunner _runner;


        [SetUp]
        public void SetUp()
        {
            _runner = Substitute.For<IRunner>();
            _util = new Util(_runner);
        }

        [Test]
        public void State()
        {
            RunDetails runDetails = null;

            _runner.Run(Arg.Do<RunDetails>(x => runDetails = x));

            var returnValue = _util.Execute("state service");

            Assert.That(runDetails.Exe, Is.EqualTo("sc.exe"));
            Assert.That(runDetails.Args, Is.EqualTo("query service"));

            returnValue = _util.Execute("server Server");

            Assert.That(returnValue.IsSuccess, Is.True);

            returnValue = _util.Execute("state service");

            Assert.That(runDetails.Exe, Is.EqualTo("sc.exe"));
            Assert.That(runDetails.Args, Is.EqualTo("//Server query service"));
        }

        [Test]
        public void Stop()
        {
            RunDetails runDetails = null;

            _runner.Run(Arg.Do<RunDetails>(x => runDetails = x));

            var returnValue = _util.Execute("stop service");

            Assert.That(runDetails.Exe, Is.EqualTo("sc.exe"));
            Assert.That(runDetails.Args, Is.EqualTo("stop service"));

            returnValue = _util.Execute("server Server");

            Assert.That(returnValue.IsSuccess, Is.True);

            returnValue = _util.Execute("stop service");

            Assert.That(runDetails.Exe, Is.EqualTo("sc.exe"));
            Assert.That(runDetails.Args, Is.EqualTo("//Server stop service"));
        }
    }
}
