using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessContactWebsiteTemplate.Interfaces
{

    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task Commit();
        void Remove(TEntity entity);
        Task Remove(object id);
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(object id);
        IQueryable<TEntity> GetPaged(int top = 20, int skip = 0);
        void Insert(TEntity entity);
        void Update(TEntity entity);
    }

}
