using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Repositories;
using HotelDemo.Persistence;

namespace Infrastructure.Repositories
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public ReadOnlyRepository(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }
        public async Task<IQueryable<T>> GetAll(Expression<Func<T, bool>>? creiteria = null)
        {
            var query = context.Set<T>().AsQueryable();

            if (creiteria != null)
            {

                query = query.Where(creiteria);

            }

            return query;
        }

      
    }
}
