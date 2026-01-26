
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

        public static ResponseDto<T> Success(T data, string message = null)
        {
            return new ResponseDto<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                ErrorCode = null
            };

        }

        public static ResponseDto<T>  Fail(ErrorCode? errorcode, string message)
        {
            return new ResponseDto<T>
            {
                Data = default(T),
                IsSuccess = false,
                Message = message,
                ErrorCode = errorcode
            };
        }
        public static ResponseDto<T> ValidaitonFial(ValidationResult validationResult)
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
