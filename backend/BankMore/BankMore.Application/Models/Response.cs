using BankMore.Domain.Enums;

namespace BankMore.Application.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = String.Empty;
        public string ErrorType { get; set; } = String.Empty;

        public static Response Success() => new();

        public static Response Error(ErrorType errorType, string message)
        {
            return new()
            {
                IsSuccess = false,
                ErrorType = errorType.ToString(),
                Message = message
            };
        }
    }

    public class Response<T> : Response
    {
        public T? Data { get; set; }

        public static Response<T> Success(T data) => new() { Data = data };
        public static new Response<T> Error(ErrorType errorType, string message)
        {
            return new()
            {
                IsSuccess = false,
                ErrorType = errorType.ToString(),
                Message = message
            };
        }
    }
}
