using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        
        public Task Add(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

      



        public Task Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }



        

        public Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> getAllOrderedByDate()
        {
            throw new NotImplementedException();
        }

      
    }
}
