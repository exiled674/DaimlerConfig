using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task delete();

        public Task add();

        public Task getByID(int id);

        public Task<IEnumerable<TEntity>> getAllOrderedByDate();

        public Task<IEnumerable<TEntity>> find(Expression<Func<TEntity, bool>> predicate);
    }
}
