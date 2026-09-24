namespace Room.Persistence.Repositories;

public class Repository<T>(RoomDbContext context) : IRepository<T> where T : class
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

    public async Task<T?> Get(Guid id, CancellationToken cancellationToken) 
        => await context.Set<T>().FindAsync(id, cancellationToken);

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