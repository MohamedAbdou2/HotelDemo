using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<bool> Add(T entity);

        Task<IQueryable<T>> GetAll(Expression<Func<T, bool>>? creiteria = null);

        Task<IQueryable<T>> GetbyId(Guid Id);
        Task<bool> UpdateIncludeAsync(T entity, params string[] modifiedParams);
        Task<bool> Update(T entity);
        Task<bool> IsExist(Expression<Func<T, bool>> creiteria);

        Task<bool> Delete(Guid Id);

    }
}
