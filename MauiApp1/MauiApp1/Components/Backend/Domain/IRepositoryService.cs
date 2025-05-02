using System.Linq.Expressions;

namespace MauiApp1.Components.Backend.Domain;

public interface IRepositoryService<T>
{
    void Add(T item);
    void Delete(T item);
    T Find(Expression<Func<T, bool>> predicate);
    T GetById(Guid id);
    IEnumerable<T> GetAll();
}