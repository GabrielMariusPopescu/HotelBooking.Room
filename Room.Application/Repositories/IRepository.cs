using System.Linq.Expressions;

namespace Room.Application.Repositories;

public interface IRepository<T> where T: class
{
    Task<T?> Add(T entity, CancellationToken cancellationToken);
    
    Task<IEnumerable<T>> Get(CancellationToken cancellationToken);
    
    Task<T?> Get(Guid id, CancellationToken cancellationToken);

    Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    
    Task<bool> Update(T entity, CancellationToken cancellationToken);

    Task<bool> Disable(T entity, CancellationToken cancellationToken);
}