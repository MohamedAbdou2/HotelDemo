namespace HotelDemo.ViewModels
{
    public class PaginatedListResponseViewModel<T>(IEnumerable<T> items, int pageNumber, int count, int pageSize)
    {

        public IEnumerable<T> Items { get; } = items;
        public int PageNumber { get; } = pageNumber;

        public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public async static Task<PaginatedListResponseViewModel<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedListResponseViewModel<T>(items, pageNumber, count, pageSize);
        }
    }
}
