using DTS.Utils.Core;

namespace DTS.Utils
{
    public interface IRunner
    {
        ReturnValue Run(RunDetails exe);
    }
}