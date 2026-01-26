using Domain.Enums;

namespace HotelDemo.ViewModels
{
    public class PaginatedResponseViewModel<T>: ResponseViewModel<T>
    {
        public PaginatedResponseViewModel(T? data, bool isSuccess, string message, ErrorCode? errorCode) 
            :base(data, isSuccess, message, errorCode) 
        {
            
        }
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public int TotalPages { get; set; }
    }
}
