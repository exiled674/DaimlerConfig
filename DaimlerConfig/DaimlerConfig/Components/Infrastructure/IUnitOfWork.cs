using System;
using System.Threading.Tasks;
using DConfig.Components.Repositories;

namespace DConfig.Components.Infrastructure
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
