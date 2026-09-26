using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Room.Persistence.Repositories;

public class Repository<T>(RoomDbContext context) : IRepository<T> where T : BaseEntity
{
    public async Task<T?> Add(T entity, CancellationToken cancellationToken)
    {
        await context.Set<T>().AddAsync(entity, cancellationToken);
        var row = await context.SaveChangesAsync(cancellationToken);
        return row > 0 ? entity : null;
    }

    public async Task<IEnumerable<T>> Get(CancellationToken cancellationToken)
    {
        var entities = await context.Set<T>().ToListAsync(cancellationToken);
        return entities.Any() ? entities : Enumerable.Empty<T>();
    }

    public async Task<T?> Get(Guid id, bool includeRelations, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = context.Set<T>();
        if (includeRelations)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => await context.Set<T>().AnyAsync(predicate, cancellationToken);

    public async Task<bool> Update(T entity, CancellationToken cancellationToken)
    {
        context.Set<T>().Update(entity);
        var row = await context.SaveChangesAsync(cancellationToken);
        return row > 0;
    }

    public async Task<bool> Disable(T entity, CancellationToken cancellationToken)
    {
        context.Set<T>().Update(entity);
        var row = await context.SaveChangesAsync(cancellationToken);
        return row > 0;
    }
}