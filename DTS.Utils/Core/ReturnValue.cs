using System;

namespace DTS.Utils.Core
{
    public class ReturnValue
    {
        private ReturnValue(bool isSuccess, string format, params object[] args)
        {
            IsSuccess = isSuccess;
            Message = String.Format(format, args);
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ReturnValue Ok(string format = "", params object[] args)
        {
            return new ReturnValue(true, format, args);
        }

        public static ReturnValue Error(string format, params object[] args)
        {
            return new ReturnValue(false, format, args);
        }
    }
}