using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    public interface IReadOnlyRepository<T> where T : class
    {
        public Task<IQueryable<T>> GetAll(Expression<Func<T,bool>> creiteria);
       

    }
}
