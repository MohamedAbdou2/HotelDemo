using Domain.Models;
using Domain.Repositories;
using HotelDemo.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private readonly ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public GenericRepository()
        {
        }


        public async Task<bool> Add(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IQueryable<T>> GetAll(Expression<Func<T, bool>>? creiteria = null)
        {
            var query = context.Set<T>().Where(x => !x.IsDeleted);

            if (creiteria != null)
            {
                query = query.Where(creiteria);
            }

            return query;
        }

        public async Task<IQueryable<T>> GetbyId(Guid Id)
        {
            var query = context.Set<T>().AsQueryable();

            query = query.Where(x => !x.IsDeleted && x.Id == Id);


            return query;
        }

        public async Task<bool> UpdateIncludeAsync(T entity, params string[] modifiedParams)
        {
            var local = context.Set<T>().Local.FirstOrDefault(x => x.Id == entity.Id);
            EntityEntry entityEntry;

            if (local == null)
            {
                context.Set<T>().Attach(entity);
                entityEntry = context.Set<T>().Entry(entity);
            }
            else
            {
                entityEntry = context.ChangeTracker.Entries<T>()
                    .First(x => x.Entity.Id == entity.Id);
            }

            foreach (var propName in modifiedParams)
            {
                var propInfo = entity.GetType().GetProperty(propName);
                if (propInfo != null)
                {
                    entityEntry.Property(propName).CurrentValue = propInfo.GetValue(entity);
                    entityEntry.Property(propName).IsModified = true;
                }
            }

            var result = await context.SaveChangesAsync();
            return result > 0;

        }

        public async Task<bool> IsExist(Expression<Func<T, bool>> creiteria)
        {
            var result = await context.Set<T>().Where(x => !x.IsDeleted).AnyAsync(creiteria);
            return result;
        }
        public async Task<bool> Delete(Guid Id)
        {
            // make it to remove 
            var entity = this.GetbyId(Id).Result.FirstOrDefault();
            var result = false;
            if (entity != null)
            {
                entity.IsDeleted = true;
                result = await this.UpdateIncludeAsync(entity, nameof(entity.IsDeleted));
            }
            return result;

        }
    }
}
