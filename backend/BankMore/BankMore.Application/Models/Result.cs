using BankMore.Domain.Enums;

namespace BankMore.Application.Models
{
    public class Result
    {
        public bool IsSuccess { get; protected set; } = true;
        public string Message { get; protected set; } = string.Empty;
        public string ErrorType { get; protected set; } = string.Empty;

        public static Result Success() => new();

        public static Result Error(ErrorType errorType, string message)
        {
            return new Result
            {
                IsSuccess = false,
                ErrorType = errorType.ToString(),
                Message = message
            };
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> Success(T data) => new() { Data = data };
        public static new Result<T> Error(ErrorType errorType, string message)
        {
            return new()
            {
                IsSuccess = false,
                ErrorType = errorType.ToString(),
                Message = message
            };
        }
    }

    public static class ResultExtensions
    {
        public static async Task<Result> Then(this Task<Result> task, Func<Result> next)
        {
            var result = await task;

            if (!result.IsSuccess)
                return result;

            return next();
        }

        public static async Task<Result<T>> Then<T>(this Task<Result> task, Func<Task<Result<T>>> next)
        {
            var result = await task;

            if (!result.IsSuccess)
                return Result<T>.Error(result.ErrorType.ToEnum<ErrorType>(), result.Message);

            return await next();
        }
    }
}
