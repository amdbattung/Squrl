using Squrl.App.Enums;

namespace Squrl.App.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public IReadOnlyList<string> Errors { get; }
    public Exception? Exception { get; }
    public FailureType? FailureType { get; }

    protected Result(
        bool isSuccess,
        string? message = null,
        IReadOnlyList<string>? errors = null,
        Exception? exception = null,
        FailureType? failureType = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? Array.Empty<string>();
        Exception = exception;
        FailureType = failureType;
    }

    public static SuccessBuilder Ok()
    {
        return new SuccessBuilder(
            new Result(isSuccess: true));
    }

    public static FailureBuilder Fail(params string[] errors)
    {
        return new FailureBuilder(
            new Result(
                isSuccess: false,
                errors: errors,
                message: errors.FirstOrDefault()));
    }

    public static FailureBuilder Fail(IEnumerable<string> errors)
    {
        string[] errorArray = errors as string[] ?? errors.ToArray();

        return new FailureBuilder(
            new Result(
                isSuccess: false,
                errors: errorArray,
                message: errorArray.FirstOrDefault()));
    }

    public Result WithoutException()
    {
        return new Result(
            isSuccess: IsSuccess,
            message: Message,
            errors: Errors,
            exception: null,
            failureType: FailureType);
    }

    // Success Builder
    public sealed class SuccessBuilder
    {
        private Result _result;

        internal SuccessBuilder(Result result)
        {
            _result = result;
        }

        public SuccessBuilder WithMessage(string message)
        {
            _result = new Result(
                _result.IsSuccess,
                message,
                _result.Errors,
                _result.Exception,
                _result.FailureType);

            return this;
        }

        private Result Build()
        {
            return _result;
        }

        public static implicit operator Result(SuccessBuilder builder)
        {
            return builder.Build();
        }
    }

    // Failure Builder
    public sealed class FailureBuilder
    {
        private Result _result;

        internal FailureBuilder(Result result)
        {
            _result = result;
        }

        public FailureBuilder WithMessage(string message)
        {
            _result = new Result(
                _result.IsSuccess,
                message,
                _result.Errors,
                _result.Exception,
                _result.FailureType);

            return this;
        }

        public FailureBuilder WithException(Exception? e)
        {
            _result = new Result(
                _result.IsSuccess,
                _result.Message,
                _result.Errors,
                e,
                _result.FailureType);

            return this;
        }

        public FailureBuilder WithFailureType(FailureType? failureType)
        {
            _result = new Result(
                _result.IsSuccess,
                _result.Message,
                _result.Errors,
                _result.Exception,
                failureType);

            return this;
        }

        private Result Build()
        {
            return _result;
        }

        public static implicit operator Result(FailureBuilder builder)
        {
            return builder.Build();
        }
    }
}