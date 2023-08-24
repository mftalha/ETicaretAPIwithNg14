﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Stroge;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Infrastructure.Enums;
using ETicaretAPI.Infrastructure.Services;
using ETicaretAPI.Infrastructure.Services.Storage;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStorageService, StorageService>(); //IStorageService karşılık hangi servisin gideceğini belirtiyoruz.
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
        }
        // where T : class, IStorage  == IStorage'den türecek ama class olacak.
        public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : Storage, IStorage  
        {
            serviceCollection.AddScoped<IStorage, T>();
            // burda t verdiğim için herhangi bir bağımlılık yok : istersem local olacak , istersem azure olacak gibi. : burayı değiştirmem gerekmiyecek.
        }
        // aşşağısı kirli kod aslında her yeni eklemede gelip enumda değişim yapmam gerekecek.
        public static void AddStorage(this IServiceCollection serviceCollection, StorageType storageType)
        {
            switch(storageType)
            {
                case StorageType.Local:
                    serviceCollection.AddScoped<IStorage, LocalStorage>();
                    break;
                case StorageType.Azure:
                    serviceCollection.AddScoped<IStorage, AzureStorage>();
                    break;
                case StorageType.AWS:
                    break;
                default:
                    serviceCollection.AddScoped<IStorage, LocalStorage>();
                    break;
            }
        }
    }
}
