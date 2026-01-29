
using Domain.Enums;
using FluentValidation.Results;

namespace Application.Dtos
{
    public class ResponseDto<T>
    {


        public T? Data { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ErrorCode? ErrorCode { get; set; }
        protected ResponseDto(T? data, bool isSuccess, string message, ErrorCode? errorCode)
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
            ErrorCode = errorCode;
        }
        public static ResponseDto<T> Success(T data, string? message = null)
        {
            return new ResponseDto<T>(data, true, message ?? "Success", null);
        }

        public static ResponseDto<T> Fail(ErrorCode? errorCode, string message)
        {
            return new ResponseDto<T>(default, false, message, errorCode);
        }
        public static ResponseDto<T> ValidaitonFail(ValidationResult validationResult)
        {
            var errorMessage = string.Join("; ",
                validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            return Fail(
                Domain.Enums.ErrorCode.ValidationError, "Validation Failed: " +
                "\n" +
                errorMessage);
        }

    }
}
