using System.Linq.Expressions;

namespace MauiApp1.Components.Backend.Domain;

public class OperationRepository : IRepositoryService<Operation>
{
    private readonly List<Operation> _operations = new List<Operation>(); // Dummy-Storage

    public void Add(Operation entity)
    {
        _operations.Add(entity);
    }

    public void Delete(Operation entity)
    {
        _operations.Remove(entity);
    }

    public Operation Find(Expression<Func<Operation, bool>> predicate)
    {
        return _operations.AsQueryable().FirstOrDefault(predicate);
    }

    public Operation GetById(Guid id)
    {
        return _operations.FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Operation> GetAll()
    {
        return _operations;
    }
}
