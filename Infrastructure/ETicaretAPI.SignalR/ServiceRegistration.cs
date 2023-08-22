using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.SignalR.HubServices;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.SignalR;

public static class ServiceRegistration
{
    public static void AddSignalRServices(this IServiceCollection collection)
    {
        collection.AddTransient<IProductHubService, ProductHubService>();
        // Bu işlem sayesinde SignalR kütüphanesi kendisi ile alakalı herşeyi IOSContainer'a atıyor bizde direk çağırabiliyoruz ihtiyacımız olan yapılarını.
        collection.AddTransient<IOrderHubService, OrderHubService>();
        collection.AddSignalR(); 
    }
}
