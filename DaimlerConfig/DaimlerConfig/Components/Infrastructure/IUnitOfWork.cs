using System;
using System.Threading.Tasks;
using DaimlerConfig.Components.Repositories;

namespace DaimlerConfig.Components.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IToolRepository ToolRepository { get; }
        IStationRepository StationRepository { get; }
        IOperationRepository OperationRepository { get; }

        Task<int> SaveChangesAsync();
        void BeginTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
    }
}
