using DTS.Utils.Core;

namespace DTS.Utils
{
    public class RunProcessReturnValue : ReturnValue
    {
        public RunProcessDetails RunProcessDetails { get; set; }
        
        public RunProcessReturnValue(RunProcessDetails runProcessDetails)
            : base(ErrorType.None, "")
        {
            RunProcessDetails = runProcessDetails;
        }
    }
}