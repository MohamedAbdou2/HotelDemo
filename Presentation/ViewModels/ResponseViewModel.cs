using Domain.Enums;

namespace HotelDemo.ViewModels
{
    public class ResponseViewModel<T>
    {
        public T? Data { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ErrorCode? ErrorCode { get; set; }

        public static ResponseViewModel<T> Success(T data, string message = null)
        {
            return new ResponseViewModel<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                ErrorCode = null
            };

        }

        public static ResponseViewModel<T> Fail(ErrorCode? errorcode, string message)
        {
            return new ResponseViewModel<T>
            {
                Data = default(T),
                IsSuccess = false,
                Message = message,
                ErrorCode = errorcode
            };
        }
    }
}
