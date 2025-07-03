using DConfig.Components.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DConfig.Components.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbTransaction _transaction;
        private IDbConnection _connection;

        public IToolRepository ToolRepository { get; }
        public IStationRepository StationRepository { get; }
        public IOperationRepository OperationRepository { get; }

        public UnitOfWork(IDbConnectionFactory connectionFactory,
                          IToolRepository toolRepository,
                          IStationRepository stationRepository,
                          IOperationRepository operationRepository)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();
            ToolRepository = toolRepository;
            StationRepository = stationRepository;
            OperationRepository = operationRepository;
        }

        public void BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                _transaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Hier kannst du Änderungen an der Datenbank speichern
            // Beispiel: await _dbContext.SaveChangesAsync();
            return 0; // Dummy-Wert
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
