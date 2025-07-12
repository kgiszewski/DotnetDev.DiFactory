using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp;

public static class CoolThingRegistration
{
    //this doesn't let us use another resolved service b/c the service provider isn't available in the signature
    public static IServiceCollection AddCoolThing(this IServiceCollection services, CoolThingOptions options)
    {
        services.AddSingleton(options);
        services.AddScoped(serviceProvider => new CoolThing(serviceProvider.GetService<CoolThingOptions>()));

        return services;
    }
    
    //this method signature let's us delegate how to make the options
    public static IServiceCollection AddCoolThingOptions(this IServiceCollection services, Func<IServiceProvider, CoolThingOptions> func)
    {
        services.AddScoped(serviceProvider =>
        {
            return func(serviceProvider);
        });

        return services;
    }
}