using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ETicaretAPI.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection collection)
    {
        //ServiceRegistration namespace'i altındaki hangler'ları bul ve kullan hepsini.
        //collection.AddMediatR(typeof(ServiceRegistration));
        collection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    }
}
