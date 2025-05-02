using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
    }
}
