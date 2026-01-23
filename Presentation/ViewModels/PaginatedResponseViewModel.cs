namespace HotelDemo.ViewModels
{
    public class PaginatedResponseViewModel<T>: ResponseViewModel<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public int TotalPages { get; set; }
    }
}
