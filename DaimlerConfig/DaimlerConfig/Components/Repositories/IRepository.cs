using System.Linq.Expressions;

namespace DaimlerConfig.Components.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task Delete(TEntity entity);
        public Task DeleteRange(IEnumerable<TEntity> entities);


        public Task Add(TEntity entity);
        public Task AddRange(IEnumerable<TEntity> entities);



        public Task<TEntity?> Get(int id);
        public Task<IEnumerable<TEntity>> GetAll();
        public Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        public Task<IEnumerable<TEntity>> GetAllOrderedByDate();

        
    }
}
