using System;

namespace DTS.Utils.Core
{
    public class ReturnValue
    {
        protected ReturnValue(bool isSuccess, string format, params object[] args)
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

    public class RunRuturnValue : ReturnValue
    {
        public RunRuturnValue(bool isSuccess, string format, params object[] args) 
            : base(isSuccess, format, args)
        {
        }
    }
}