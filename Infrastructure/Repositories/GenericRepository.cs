using Domain.Models;
using Domain.Repositories;
using HotelDemo.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Polly;
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
                entityEntry.State = EntityState.Unchanged;

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
        //type safe version, not usign reflection so it is faster 
        public async Task<bool> UpdateIncludeAsync(T entity, params Expression<Func<T, object>>[] properties)
        {
            var local = context.Set<T>().Local.FirstOrDefault(x => x.Id == entity.Id);
            EntityEntry<T> entityEntry;

            if (local == null)
            {
                context.Set<T>().Attach(entity);
                entityEntry = context.Entry(entity);
            }
            else
            {
                entityEntry = context.Entry(local);
                entityEntry.CurrentValues.SetValues(entity);
            }

            foreach (var property in properties)
            {
                entityEntry.Property(property).IsModified = true;
            }

            var result = await context.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> IsExist(Expression<Func<T, bool>> creiteria)
        {
            var result = await context.Set<T>().Where(x => !x.IsDeleted).AnyAsync(creiteria);
            return result;
        }
      

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await context.Set<T>()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        }

     
        public async Task<bool> Delete(Guid id)
        {
            var entity = await GetByIdAsync(id); 
            if (entity == null) return false;

            entity.IsDeleted = true;
            return await UpdateIncludeAsync(entity, nameof(BaseModel.IsDeleted));
        }

        public async Task<bool> Update(T entity)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                context.Set<T>().Attach(entity);
            }

            entry.State = EntityState.Modified;

             var result =  await context.SaveChangesAsync();
            return result > 0;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> creiteria)
        {
            return context.Set<T>().Where(m => !m.IsDeleted).Where(creiteria);
        }

        public async Task<bool> SoftDeleteAsync(T entity)
        {
            entity.IsDeleted = true;

            var isDeleted = await UpdateIncludeAsync(entity, nameof(entity.IsDeleted));

            return isDeleted;
        }
    }
}
