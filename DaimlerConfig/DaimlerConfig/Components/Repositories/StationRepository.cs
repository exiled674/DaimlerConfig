using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;
using Dapper;

namespace DaimlerConfig.Components.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        public StationRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        public async Task<IEnumerable<Station>> GetStationsFromLine(int lineID)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var query = @"
                SELECT * 
                FROM Station
                WHERE lineID = @lineID";
           

            var stations = await connection.QueryAsync<Station>(query, new { lineID });

            return stations;
        }
    }
}
