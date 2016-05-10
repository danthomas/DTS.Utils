using DTS.Utils.Core;

namespace DTS.Utils
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