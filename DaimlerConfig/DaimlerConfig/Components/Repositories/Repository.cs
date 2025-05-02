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
        public Task add()
        {
            throw new NotImplementedException();
        }

        public Task delete()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> getAllOrderedByDate()
        {
            throw new NotImplementedException();
        }

        public Task getByID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
