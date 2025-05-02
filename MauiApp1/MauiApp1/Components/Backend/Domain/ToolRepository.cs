using System.Linq.Expressions;

namespace MauiApp1.Components.Backend.Domain;

public class ToolRepository : IRepositoryService<Tool>
{
    private readonly List<Tool> _tools = new List<Tool>();
    
//Write -------------------------------------------------------------------
    public void Add(Tool entity)
    {
        _tools.Add(entity);
    }

    public void Delete(Tool entity)
    {
        _tools.Remove(entity);
    }
    
//Read -------------------------------------------------------------------
    public Tool Find(Expression<Func<Tool, bool>> predicate)
    {
        return _tools.AsQueryable().FirstOrDefault(predicate);
    }

    public Tool GetById(Guid id)
    {
        return _tools.FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<Tool> GetAll()
    {
        return _tools;
    }
}
