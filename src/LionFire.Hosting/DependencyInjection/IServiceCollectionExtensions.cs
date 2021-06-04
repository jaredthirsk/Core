﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services)
        {
            services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));

            // TODO: Use a function that captures the instance after it is created and set ManualSingleton<T>.Instance to it 
            //if (AppHostBuilderSettings.SetManualSingletons)
            //{
            //    ManualSingleton<T>.Instance = implementationInstance;
            //}
            return services;
        }

        public static IServiceCollection TryAddEnumerableSingleton<TService>(this IServiceCollection services, TService instance)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TService), ServiceLifetime.Singleton));
            //services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), instance));

            // TODO: Use a function that captures the instance after it is created and set ManualSingleton<T>.Instance to it 
            //if (AppHostBuilderSettings.SetManualSingletons)
            //{
            //    ManualSingleton<T>.Instance = implementationInstance;
            //}
            return services;
        }

        public static IServiceCollection TryAddEnumerableSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), sp => factory(sp), ServiceLifetime.Singleton));
            //services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TService), ServiceLifetime.Singleton));
            //services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), instance));

            // TODO: Use a function that captures the instance after it is created and set ManualSingleton<T>.Instance to it 
            //if (AppHostBuilderSettings.SetManualSingletons)
            //{
            //    ManualSingleton<T>.Instance = implementationInstance;
            //}
            return services;
        }

        public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services, TImplementation instance)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
            return services;
        }

        public static IServiceCollection If(this IServiceCollection services, bool condition, Action<IServiceCollection> action)
        {
            if (condition)
            {
                action(services);
            }
            return services;
        }

        #region IHostedService

        public static IServiceCollection AddSingletonHostedService<T>(this IServiceCollection services)
            where T : class, IHostedService
            => services
                .AddSingleton<T>()
                .AddHostedService<T>(sp => sp.GetRequiredService<T>());

        #endregion
    }
}
namespace LionFire.Hosting
{
    // RENAME
}