using DTS.Utils.Core;
using DTS.Utils.Details;

namespace DTS.Utils.ReturnValues
{
    public class SelectOptionReturnValue : ReturnValue
    {
        public SelectOptionDetails SelectOptionDetails { get; set; }
        
        public SelectOptionReturnValue(SelectOptionDetails selectOptionDetails)
            : base(ErrorType.None, "")
        {
            SelectOptionDetails = selectOptionDetails;
        }
    }
}