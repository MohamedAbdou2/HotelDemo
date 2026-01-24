using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    public  interface IGenericRepository<T> where T : BaseModel
    {
        Task<bool> Add(T entity);

        Task<IQueryable<T>> GetAll(Expression<Func<T, bool>>? creiteria = null);

        Task<IQueryable<T>> GetbyId(Guid Id);

        Task<bool> UpdateIncludeAsync(T entity, params string[] modifiedParams);

        Task<bool> IsExist(Guid Id);

        Task<bool> Delete(Guid Id);

    }
}
