using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PaginatedListResponseDto<T>(IEnumerable<T> items, int pageNumber, int count, int pageSize)
    {

        public IEnumerable<T> Items { get; } = items;
        public int PageNumber { get; } = pageNumber;

        public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public async static Task<PaginatedListResponseDto<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedListResponseDto<T>(items, pageNumber, count, pageSize);
        }
    }
}
