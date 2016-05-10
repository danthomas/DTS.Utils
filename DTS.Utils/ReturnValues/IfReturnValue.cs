using DTS.Utils.Core;
using DTS.Utils.Details;

namespace DTS.Utils.ReturnValues
{
    public class IfReturnValue : ReturnValue
    {
        public IfDetails IfDetails { get; set; }

        public IfReturnValue(IfDetails ifDetails)
            : base(ErrorType.None, "")
        {
            IfDetails = ifDetails;
        }
    }
}