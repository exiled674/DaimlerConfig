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
        public Task<bool> Delete(TEntity entity);
        public Task<bool> DeleteRange(IEnumerable<TEntity> entities);


        public Task Add(TEntity entity);
        public Task AddRange(IEnumerable<TEntity> entities);

        public Task Update(TEntity entity);



        public Task<TEntity?> Get(int? id);

        public Task<TEntity?> GetByName(string name);
        public Task<IEnumerable<TEntity>> GetAll();
        public Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        public Task<IEnumerable<TEntity>> getAllOrderedByDate();

        public Task<bool> ExistsByName(string name);


    }
}
