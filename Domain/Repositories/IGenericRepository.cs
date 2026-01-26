using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.VisualBasic;

namespace Domain.Repositories
{
    public  interface IGenericRepository<T> where T : BaseModel 
    {
        Task<bool> Add(T entity);

        IQueryable<T> GetAll(Expression<Func<T, bool>>? creiteria = null);

        IQueryable<T> GetbyId(Guid Id);
        Task<bool> UpdateIncludeAsync(T entity, params string[] modifiedParams);

        Task<bool> IsExist(Expression<Func<T,bool>> creiteria);

        Task<bool> Delete(Guid Id);


    }
}
