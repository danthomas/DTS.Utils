using DTS.Utils.Core;
using DTS.Utils.Details;

namespace DTS.Utils.ReturnValues
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