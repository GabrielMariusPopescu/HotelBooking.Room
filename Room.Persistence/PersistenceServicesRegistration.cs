namespace Room.Persistence;

public static class PersistenceServicesRegistration
{
    public static void RegisterRepositories(this IServiceCollection services)
    => services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
    
    public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RoomDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var assemblyName = typeof(RoomDbContext).Assembly.FullName;
            options.UseNpgsql(connectionString, builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                builder.MigrationsAssembly(assemblyName);
            });
            options.ConfigureWarnings(warnings =>
            {
                warnings.Log(CoreEventId.ManyServiceProvidersCreatedWarning);
                warnings.Log(RelationalEventId.MultipleCollectionIncludeWarning);
            });
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
    }
}