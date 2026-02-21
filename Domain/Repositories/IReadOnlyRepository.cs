using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IReadOnlyRepository<T> where T : class
    {
        public IQueryable<T>  GetAll(Expression<Func<T, bool>> creiteria);

    }
}
