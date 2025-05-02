using System.Linq.Expressions;

namespace MauiApp1.Components.Backend.Domain;

public class StationRepository : IRepositoryService<Station>
{
    private readonly List<Station> _stations = new List<Station>(); // Beispiel-Dummy-Daten

    public void Add(Station entity)
    {
        _stations.Add(entity);
    }

    public void Delete(Station entity)
    {
        _stations.Remove(entity);
    }

    public Station Find(Expression<Func<Station, bool>> predicate)
    {
        return _stations.AsQueryable().FirstOrDefault(predicate);
    }

    public Station GetById(Guid id)
    {
        return _stations.FirstOrDefault(s => s.Id == id);
    }

    public IEnumerable<Station> GetAll()
    {
        return _stations;
    }
}
