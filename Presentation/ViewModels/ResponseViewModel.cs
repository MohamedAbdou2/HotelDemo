using Application.Dtos;
using Domain.Enums;
using FluentValidation.Results;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelDemo.ViewModels
{
    public class ResponseViewModel<T>
    {

        public T? Data { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ErrorCode? ErrorCode { get; set; }

        public ResponseViewModel (T? data, bool isSuccess, string message, ErrorCode? errorCode)
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
            ErrorCode = errorCode;
        }

        public static ResponseViewModel<T> Success(T data, string message = null)
        {
            return new ResponseViewModel<T>(data, true, message ?? "Success", null);

        }

        public static ResponseViewModel<T> Fail(ErrorCode? errorcode, string message)
        {
            return new ResponseViewModel<T>(default, false,message,null);
        }

        public static ResponseViewModel<T> ValidationFail(ValidationResult validationResult)
        {
            var errorMessage = string.Join("; ",
                validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            return Fail(
                Domain.Enums.ErrorCode.ValidationError, $"Validation Failed \n {errorMessage}");
        }
    }
}
