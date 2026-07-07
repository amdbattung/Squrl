using Squrl.App.Enums;

namespace Squrl.App.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Message { get; }
    public IReadOnlyList<string> Errors { get; }
    public Exception? Exception { get; }
    public FailureType? FailureType { get; }

    private Result(
        bool isSuccess,
        T? value = default,
        string? message = null,
        IReadOnlyList<string>? errors = null,
        Exception? exception = null,
        FailureType? failureType = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        Errors = errors ?? Array.Empty<string>();
        Exception = exception;
        FailureType = failureType;
    }

    public static SuccessBuilder<T> Ok(T value)
    {
        return new SuccessBuilder<T>(
            new Result<T>(isSuccess: true, value: value));
    }

    public static FailureBuilder<T> Fail(params string[] errors)
    {
        return new FailureBuilder<T>(
            new Result<T>(
                isSuccess: false,
                errors: errors,
                message: errors.FirstOrDefault()));
    }
    
    // Success Builder
    public sealed class SuccessBuilder<TValue>
    {
        private Result<TValue> _result;

        internal SuccessBuilder(Result<TValue> result)
        {
            _result = result;
        }

        public SuccessBuilder<TValue> WithMessage(string message)
        {
            _result = new Result<TValue>(
                _result.IsSuccess,
                _result.Value,
                message,
                _result.Errors,
                _result.Exception,
                _result.FailureType);

            return this;
        }

        private Result<TValue> Build()
        {
            return _result;
        }

        public static implicit operator Result<TValue>(SuccessBuilder<TValue> builder)
        {
            return builder.Build();
        }
    }
    
    // Failure Builder
    public sealed class FailureBuilder<TValue>
    {
        private Result<TValue> _result;

        internal FailureBuilder(Result<TValue> result)
        {
            _result = result;
        }

        public FailureBuilder<TValue> WithMessage(string message)
        {
            _result = new Result<TValue>(
                _result.IsSuccess,
                _result.Value,
                message,
                _result.Errors,
                _result.Exception,
                _result.FailureType);

            return this;
        }
        
        public FailureBuilder<TValue> WithException(Exception e)
        {
            _result = new Result<TValue>(
                _result.IsSuccess,
                _result.Value,
                _result.Message,
                _result.Errors,
                e,
                _result.FailureType);

            return this;
        }

        public FailureBuilder<TValue> WithFailureType(FailureType failureType)
        {
            _result = new Result<TValue>(
                _result.IsSuccess,
                _result.Value,
                _result.Message,
                _result.Errors,
                _result.Exception,
                failureType);

            return this;
        }

        private Result<TValue> Build()
        {
            return _result;
        }

        public static implicit operator Result<TValue>(FailureBuilder<TValue> builder)
        {
            return builder.Build();
        }
    }
}