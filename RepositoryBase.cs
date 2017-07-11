using BusinessContactWebsiteTemplate.DAL;
using BusinessContactWebsiteTemplate.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessContactWebsiteTemplate.Repositories
{
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        internal letsgrow_databaseEntities Context;
        internal DbSet<TEntity> DbSet;

        public RepositoryBase(letsgrow_databaseEntities context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public async Task Commit()
        {
            await Context.SaveChangesAsync();
        }

        public async Task Remove(object id)
        {
            var entity = await DbSet.FindAsync(id);
            Remove(entity);
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public async Task<TEntity> GetById(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual IQueryable<TEntity> GetPaged(int pagesize = 20, int skip = 0)
        {
            return DbSet.Skip(skip).Take(pagesize);
        }

        public void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }
    }
}