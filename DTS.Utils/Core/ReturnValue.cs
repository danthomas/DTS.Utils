using System;

namespace DTS.Utils.Core
{
    public class ReturnValue
    {
        protected ReturnValue(ReturnValueType returnValueType, ErrorType errorType, string format, params object[] args)
        {
            IsSuccess = errorType == ErrorType.None;
            ReturnValueType = returnValueType;
            ErrorType = errorType;
            Message = String.Format(format ?? "", args);
        }

        public bool IsSuccess { get; protected set; }
        public string Message { get; private set; }
        public ReturnValueType ReturnValueType { get; set; }
        public ErrorType ErrorType { get; private set; }

        public static ReturnValue Ok(string format = "", params object[] args)
        {
            return new ReturnValue(ReturnValueType.None, ErrorType.None, format, args);
        }

        public static ReturnValue Error(ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue(ReturnValueType.None, errorType, format, args);
        }

        public static ReturnValue Ok(ReturnValueType returnValueType, string format = "", params object[] args)
        {
            return new ReturnValue(returnValueType, ErrorType.None, format, args);
        }

        public static ReturnValue Error(ReturnValueType returnValueType, ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue(returnValueType, errorType, format, args);
        }
    }

    public class ReturnValue<T> : ReturnValue
    {
        public ReturnValue(T data, ReturnValueType returnValueType, ErrorType errorType, string format, params object[] args)
            : base(returnValueType, errorType, format, args)
        {
            Data = data;
        }

        public T Data { get; private set; }

        public new static ReturnValue<T> Ok(string format = "", params object[] args)
        {
            return new ReturnValue<T>(default(T), ReturnValueType.None, ErrorType.None, format, args);
        }

        public static ReturnValue<T> Ok(T data, string format = "", params object[] args)
        {
            return new ReturnValue<T>(data, ReturnValueType.None, ErrorType.None, format, args);
        }

        public new static ReturnValue<T> Error(ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue<T>(default(T), ReturnValueType.None, errorType, format, args);
        }

        public static ReturnValue<T> Error(T data, ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue<T>(data, ReturnValueType.None, errorType, format, args);
        }

        public new static ReturnValue<T> Ok(ReturnValueType returnValueType, string format = "", params object[] args)
        {
            return new ReturnValue<T>(default(T), returnValueType, ErrorType.None, format, args);
        }

        public static ReturnValue<T> Ok(T data, ReturnValueType returnValueType, string format = "", params object[] args)
        {
            return new ReturnValue<T>(data, returnValueType, ErrorType.None, format, args);
        }

        public new static ReturnValue<T> Error(ReturnValueType returnValueType, ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue<T>(default(T), returnValueType, errorType, format, args);
        }

        public static ReturnValue<T> Error(T data, ReturnValueType returnValueType, ErrorType errorType, string format, params object[] args)
        {
            return new ReturnValue<T>(data, returnValueType, errorType, format, args);
        }
    }
}