using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Infrastructure;
using Dapper;


namespace DaimlerConfig.Components.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly string _tableName;


        public Repository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _tableName = typeof(TEntity).Name;
        }

        public async Task Add(TEntity entity)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            
            var properties = typeof(TEntity).GetProperties();

            
            var columnNames = properties.Select(p => p.Name);
            var columns = string.Join(", ", columnNames);
            var paramNames = string.Join(", ", columnNames.Select(n => "@" + n));
            var sql = $@"
        INSERT INTO {_tableName}
        ({columns})
        VALUES
        ({paramNames});
    ";

           
            var dp = new DynamicParameters();
            foreach (var p in properties)
            {
                var val = p.GetValue(entity);
                if (p.Name.Equals("lastModified", StringComparison.OrdinalIgnoreCase)
                    && p.PropertyType == typeof(DateTime))
                {
                    
                    var dt = (DateTime)val;
                    dp.Add(p.Name, dt.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    dp.Add(p.Name, val);
                }
            }

            await conn.ExecuteAsync(sql, dp);
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

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var sql = $"SELECT * FROM {_tableName}";
            return await conn.QueryAsync<TEntity>(sql);
        }


        public Task<IEnumerable<TEntity>> getAllOrderedByDate()
        {
            throw new NotImplementedException();
        }

      
    }
}
