using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PaginatedResponseDto<T> : ResponseDto<T>
    {
        public PaginatedResponseDto(T? data, bool isSuccess, string message, ErrorCode? errorCode)
            : base(data, isSuccess, message, errorCode)
        {

        }
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public int TotalPages { get; set; }
    }
}
