# DotnetDev.DiFactory

A .NET library demonstrating advanced dependency injection patterns, specifically showing how to expose the ServiceProvider during DI registration to enable more flexible service configuration.

## Overview

This library provides a pattern for creating extension methods that can access the `IServiceProvider` during service registration. This is particularly useful when you need to resolve other services to configure your dependencies, such as when integrating with secrets managers, configuration providers, or other complex scenarios.

## Tutorial: Exposing ServiceProvider During DI Registration

### The Problem

Traditional DI registration methods don't provide access to the `IServiceProvider`, limiting your ability to resolve other services during configuration:

```csharp
// Limited - can't resolve other services
services.AddCoolThing(new CoolThingOptions
{
    SomeSetting = configurationManager.GetSection("CoolThing")["SomeValue"]
});
```

### The Solution

This library demonstrates a pattern where you can expose the `IServiceProvider` during registration:

```csharp
// Flexible - can resolve other services for configuration
services.AddCoolThingOptions(serviceProvider => new CoolThingOptions
{
    SomeSetting = serviceProvider.GetService<SecretsManager>().GetSecret("foo")
});
```

### Implementation Pattern

Here's how to implement this pattern in your own extension methods:

#### 1. Traditional Extension Method (Limited)

```csharp
public static IServiceCollection AddCoolThing(this IServiceCollection services, CoolThingOptions options)
{
    services.AddSingleton(options);
    services.AddScoped(serviceProvider => new CoolThing(serviceProvider.GetService<CoolThingOptions>()));
    
    return services;
}
```

#### 2. ServiceProvider-Aware Extension Method (Flexible)

```csharp
public static IServiceCollection AddCoolThingOptions(this IServiceCollection services, Func<IServiceProvider, CoolThingOptions> func)
{
    services.AddScoped(serviceProvider =>
    {
        return func(serviceProvider);
    });

    return services;
}
```

### Complete Usage Example

```csharp
using ConsoleApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configurationManager = new ConfigurationManager();

// Traditional approach - limited to configuration and static values
services.AddCoolThing(new CoolThingOptions
{
    SomeSetting = configurationManager.GetSection("CoolThing")["SomeValue"]
});

// ServiceProvider-aware approach - can resolve other services
services.AddCoolThingOptions(serviceProvider => new CoolThingOptions
{
    SomeSetting = serviceProvider.GetService<SecretsManager>().GetSecret("foo")
});

// Register the main service
services.AddScoped(serviceProvider => new CoolThing(serviceProvider.GetService<CoolThingOptions>()));
```

### Use Cases

This pattern is particularly valuable for:

1. **Secrets Management**: Resolving secrets from a secrets manager service
2. **Complex Configuration**: Building configuration that depends on other services
3. **Service Composition**: Creating services that require other resolved dependencies
4. **Runtime Decision Making**: Configuring services based on runtime conditions
5. **External API Integration**: Fetching configuration from external APIs during startup

### Benefits

- **Flexibility**: Access to the full service container during registration
- **Separation of Concerns**: Keep configuration logic separate from service logic
- **Testability**: Easier to mock dependencies during configuration
- **Maintainability**: Cleaner, more modular registration patterns

### Best Practices

1. **Use Factory Functions**: Always use `Func<IServiceProvider, T>` for ServiceProvider-aware registrations
2. **Avoid Circular Dependencies**: Be careful not to create circular dependencies when resolving services
3. **Consider Lifetime**: Ensure the lifetime of resolved services matches your needs
4. **Error Handling**: Include proper error handling for service resolution failures

## Getting Started

1. Clone this repository
2. Open the solution in your IDE
3. Examine the `ConsoleApp/CoolThingRegistration.cs` file for implementation details
4. Run the console application to see the pattern in action

## License

This project is provided as an educational example for demonstrating advanced dependency injection patterns in .NET.
