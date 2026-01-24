using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dtos
{
    public class ResponseDto<T>
    {
        public T? Data { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ErrorCode? ErrorCode { get; set; }

        public ResponseDto<T> Success(T data, string message = null)
        {
            return new ResponseDto<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                ErrorCode = null
            };

        }

        public ResponseDto<T> Fail(ErrorCode? errorcode, string message)
        {
            return new ResponseDto<T>
            {
                Data = default(T),
                IsSuccess = false,
                Message = message,
                ErrorCode = errorcode
            };
        }
    }
}
