using ConsoleApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configurationManager = new ConfigurationManager();

//if a Nuget package provides something in this format to add the service, it presumes you'll likely not be using another dependency to construct the options
services.AddCoolThing(new CoolThingOptions
{
    SomeSetting = configurationManager.GetSection("CoolThing")["SomeValue"] //not a whole lot of choices here other than the config
});

//this one will delegate how to create the options, but this time by injection the service provider so you can resolve another service properly
services.AddCoolThingOptions(serviceProvider => new CoolThingOptions
{
    SomeSetting = "" //now you can resolve your service like a secrets manager properly.
    //SomeSetting = serviceProvider.GetService<SecretsManager>().GetSecret("foo")
});

//and then all we have to do is register the cool thing either here or in another setup method with another factory delegate
services.AddScoped(serviceProvider => new CoolThing(serviceProvider.GetService<CoolThingOptions>()));
