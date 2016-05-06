using DTS.Utils.Core;

namespace DTS.Utils
{
    public class RunProcessReturnValue : ReturnValue
    {
        public RunProcessDetails RunProcessDetails { get; set; }
        
        public RunProcessReturnValue(RunProcessDetails getRunProcessDetails)
            : base(ErrorType.None, "")
        {
            RunProcessDetails = getRunProcessDetails;
        }
    }
}