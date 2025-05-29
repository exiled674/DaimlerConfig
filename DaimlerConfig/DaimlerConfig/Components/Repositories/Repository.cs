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
        protected readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly string _tableName;

        public Repository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _tableName = typeof(TEntity).Name;
        }

        public async Task<bool> ExistsByName(string name)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            // Dynamisch den Spaltennamen basierend auf der Tabelle setzen
            string nameProperty = _tableName switch
            {
                "Station" => "assemblystation",
                "Tool" => "toolShortname",
                "Operation" => "operationShortname",
                "Line" => "lineName",
                _ => throw new InvalidOperationException($"Unbekannte Tabelle: {_tableName}")
            };

            var sql = $"SELECT COUNT(1) FROM [{_tableName}] WHERE [{nameProperty}] = @name";
            var result = await conn.ExecuteScalarAsync<int>(sql, new { name });

            return result > 0;
        }

        public async Task Add(TEntity entity)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var type = typeof(TEntity);
            var primaryKey = type.Name + "ID";

            var properties = type
                .GetProperties()
                .Where(p => !string.Equals(p.Name, primaryKey, StringComparison.OrdinalIgnoreCase));

            // Bestimme den Namen, der überprüft werden muss
            var nameProperty = char.ToLowerInvariant(_tableName[0]) + _tableName.Substring(1) + "Name";
            var nameValue = type.GetProperty(nameProperty)?.GetValue(entity)?.ToString();

            if (nameValue != null && await ExistsByName(nameValue))
            {
                throw new InvalidOperationException($"{nameProperty} '{nameValue}' existiert bereits.");
            }

            var columnNames = properties.Select(p => p.Name);
            var columns = string.Join(", ", columnNames.Select(c => $"[{c}]"));
            var paramNames = string.Join(", ", columnNames.Select(n => "@" + n));
            var sql = $@"
            INSERT INTO [{_tableName}]
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
                    dp.Add(p.Name, (DateTime)val);
                }
                else
                {
                    dp.Add(p.Name, val);
                }
            }

            await conn.ExecuteAsync(sql, dp);
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await Add(entity);
            }
        }

        public async Task<bool> Delete(TEntity entity)
        {
            try
            {
                using var conn = _dbConnectionFactory.CreateConnection();
                conn.Open();

                var keyProp = typeof(TEntity)
                    .GetProperties()
                    .FirstOrDefault(p => p.Name.Equals($"{_tableName}ID", StringComparison.OrdinalIgnoreCase));

                if (keyProp == null)
                    throw new InvalidOperationException($"Keine Primärschlüssel-Property für {_tableName} gefunden.");

                var keyName = keyProp.Name;
                var keyValue = keyProp.GetValue(entity);

                var sql = $"DELETE FROM [{_tableName}] WHERE [{keyName}] = @{keyName}";

                var dp = new DynamicParameters();
                dp.Add(keyName, keyValue);

                var number = await conn.ExecuteAsync(sql, dp);
                return number > 0;
            }
            catch 
            {
                return false;
            }
           
        }

        public async Task<bool> DeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                if (!await Delete(entity)) return false;

            return true;
        }



        public async Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            var all = await GetAll(); 
            return all.AsQueryable().Where(predicate); 
        }


        public async Task<TEntity?> Get(int? id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var type = typeof(TEntity);
            var keyProp = type.GetProperties()
    .FirstOrDefault(p =>
        p.Name.Equals($"{_tableName}ID", StringComparison.OrdinalIgnoreCase) ||  // ← ORIGINAL (bleibt)
        p.Name.Equals("ID", StringComparison.OrdinalIgnoreCase));

            if (keyProp == null)
                throw new InvalidOperationException($"Keine Primärschlüssel-Property für {_tableName} gefunden.");

            var keyName = keyProp.Name;
            var sql = $"SELECT * FROM [{_tableName}] WHERE [{keyName}] = @{keyName}";

            var dp = new DynamicParameters();
            dp.Add(keyName, id);

            return await conn.QuerySingleOrDefaultAsync<TEntity>(sql, dp);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var sql = $"SELECT * FROM [{_tableName}]";
            return await conn.QueryAsync<TEntity>(sql);
        }

        public async Task<IEnumerable<TEntity>> getAllOrderedByDate()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var props = typeof(TEntity).GetProperties();
            var hasLastModified = props.Any(p => p.Name.Equals("lastModified", StringComparison.OrdinalIgnoreCase));

            if (!hasLastModified)
                throw new InvalidOperationException("TEntity hat keine 'lastModified'-Eigenschaft.");

            var sql = $"SELECT * FROM [{_tableName}] ORDER BY [lastModified] DESC";
            return await conn.QueryAsync<TEntity>(sql);
        }

        public async Task<IEnumerable<TEntity>> GetAllSortedByName(bool descending = false)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var nameProperty = char.ToLowerInvariant(_tableName[0]) + _tableName.Substring(1) + "Name";
            var props = typeof(TEntity).GetProperties();
            var hasNameProp = props.Any(p => p.Name.Equals(nameProperty, StringComparison.OrdinalIgnoreCase));

            if (!hasNameProp)
                throw new InvalidOperationException($"TEntity hat keine '{nameProperty}'-Eigenschaft.");

            var direction = descending ? "DESC" : "ASC";
            var sql = $"SELECT * FROM [{_tableName}] ORDER BY [{nameProperty}] {direction}";
            return await conn.QueryAsync<TEntity>(sql);
        }

        public async Task Update(TEntity entity)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            var type = typeof(TEntity);
            var keyProp = type.GetProperties()
                .FirstOrDefault(p => p.Name.Equals($"{_tableName}ID", StringComparison.OrdinalIgnoreCase));

            if (keyProp == null)
                throw new InvalidOperationException($"Keine Primärschlüssel-Property für {_tableName} gefunden.");

            var keyName = keyProp.Name;
            var keyValue = keyProp.GetValue(entity);

            var props = type.GetProperties()
                .Where(p => !string.Equals(p.Name, keyName, StringComparison.OrdinalIgnoreCase));

            var setClause = string.Join(", ", props.Select(p => $"[{p.Name}] = @{p.Name}"));

            var sql = $@"
UPDATE [{_tableName}]
SET {setClause}
WHERE [{keyName}] = @{keyName};
";

            var dp = new DynamicParameters();
            foreach (var p in props)
            {
                var val = p.GetValue(entity);
                if (p.Name.Equals("lastModified", StringComparison.OrdinalIgnoreCase)
                    && p.PropertyType == typeof(DateTime))
                {
                    dp.Add(p.Name, (DateTime)val);
                }
                else
                {
                    dp.Add(p.Name, val);
                }
            }
            dp.Add(keyName, keyValue);

            await conn.ExecuteAsync(sql, dp);
        }

        public async Task<TEntity?> GetByName(string name)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            string nameProperty = _tableName.ToLower() switch
            {
                "station" => "stationName",
                "tool" => "toolShortname",
                "operation" => "operationShortname",
                "line" => "lineName",
                _ => throw new InvalidOperationException($"Kein Name-Mapping für Tabelle '{_tableName}'.")
            };

            var props = typeof(TEntity).GetProperties();
            var hasNameProp = props.Any(p => p.Name.Equals(nameProperty, StringComparison.OrdinalIgnoreCase));
            if (!hasNameProp)
                throw new InvalidOperationException($"TEntity hat keine '{nameProperty}'-Eigenschaft.");

            var sql = $"SELECT TOP 1 * FROM [{_tableName}] WHERE [{nameProperty}] = @name";
            return await conn.QueryFirstOrDefaultAsync<TEntity>(sql, new { name });
        }

        public async Task<bool> ExistsByNameWithForeignKey(string name, string foreignKeyColumn, int foreignKeyId)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();

            string nameProperty = _tableName switch
            {
                "Station" => "assemblystation",
                "Tool" => "toolShortname",
                "Operation" => "operationShortname",
                "Line" => "lineName",
                _ => throw new InvalidOperationException($"Unbekannte Tabelle: {_tableName}")
            };

            var sql = $"SELECT COUNT(1) FROM [{_tableName}] WHERE [{nameProperty}] = @name AND [{foreignKeyColumn}] = @foreignKeyId";
            var result = await conn.ExecuteScalarAsync<int>(sql, new { name, foreignKeyId });

            return result > 0;
        }

    }
}
