using DTS.Utils.Core;

namespace DTS.Utils
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