﻿using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    using LionFire.Data;

    public static class IHasConnectionStringServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureConnection<THasConnectionString>(this IServiceCollection services, string connectionString)
       where THasConnectionString : class, IHasConnectionString
        {
            services.Configure<THasConnectionString>(o => o.ConnectionString = connectionString);
            return services;
        }
    }
}
