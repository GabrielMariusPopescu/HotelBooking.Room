namespace Room.Application;

public static class ApplicationServicesRegistration
{
    public static void RegisterMediator(this IServiceCollection services)
    {
        var assembly = typeof(CreateRoomCommandValidator).Assembly;
        services.AddValidatorsFromAssembly(assembly);
        
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
    }
}