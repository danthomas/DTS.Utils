using System;

namespace DTS.Utils.Core
{
    public class ReturnValue
    {
        protected ReturnValue(ErrorType errorType, string format, params object[] args)
        {
            IsSuccess = errorType == ErrorType.None;
            ErrorType = errorType;
            Message = String.Format(format ?? "", args);
        }

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public ErrorType ErrorType { get; private set; }

        public static ReturnValue Ok(string format = "", params object[] args)
        {
            return new ReturnValue(ErrorType.None, format, args);
        }

        public static ReturnValue Error(ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue(errorType, format, args);
        }

        public virtual ReturnValueType ReturnValueType => ReturnValueType.Standard;
    }

    public class ReturnValue<T> : ReturnValue
    {
        public ReturnValue(T data, bool isSuccess, ErrorType errorType, string format, params object[] args)
            : base(errorType, format, args)
        {
            Data = data;
        }

        public T Data { get; private set; }
        
        public new static ReturnValue<T> Ok(string format = "", params object[] args)
        {
            return new ReturnValue<T>(default(T), true, ErrorType.None, format, args);
        }

        public static ReturnValue<T> Ok(T data, string format = "", params object[] args)
        {
            return new ReturnValue<T>(data, true, ErrorType.None, format, args);
        }

        public new static ReturnValue<T> Error(ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue<T>(default(T), false, errorType, format, args);
        }

        public static ReturnValue<T> Error(T data, ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue<T>(data, false, errorType, format, args);
        }
    }
}