﻿using LionFire.Assets;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Assets;
using LionFire.Vos.Assets.Handles;
using LionFire.Vos.Assets.Persisters;
using LionFire.Vos.Collections.ByType;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Vos.Collections;
using LionFire.Types;

namespace LionFire.Services
{


    public static class VosAssetServicesExtensions
    {
        public static IServiceCollection AddAssets(this IServiceCollection services, VosAssetOptions options = null, VosReference contextVob = null)
        {
            var assetsRoot = contextVob?.Path ?? "assets";
            services
                .AddSingleton<IReadHandleProvider<IAssetReference>, VosAssetHandleProvider>()
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>, VosAssetHandleProvider>()
                .VobEnvironment("assets", assetsRoot)
                .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue)
                .InitializeVob<IServiceProvider>("$assets", (vob, serviceProvider) =>
                {
                    vob.AddOwn<ICollectionTypeProvider>(v => new CollectionsByTypeManager(v, serviceProvider.GetRequiredService<TypeNameRegistry>()));
                }, key: "$assets<ICollectionTypeProvider>")

                .Configure<VosAssetOptions>(o => { })
                .AddSingleton(s => s.GetService<IOptionsMonitor<VosAssetOptions>>()?.CurrentValue)

                .TryAddEnumerableSingleton<ICollectionTypeProvider, CollectionsByTypeManager>()

                .AddSingleton<VosAssetHandleProvider>()
                .AddSingleton<IReadHandleProvider<IAssetReference>, VosAssetHandleProvider>(s => s.GetRequiredService<VosAssetHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>, VosAssetHandleProvider>(s => s.GetRequiredService<VosAssetHandleProvider>())

                .AddSingleton<VosAssetPersisterProvider>()
                .AddSingleton<IPersisterProvider<IAssetReference>, VosAssetPersisterProvider>(s => s.GetRequiredService<VosAssetPersisterProvider>())

                .AddAssetPersister(options, contextVob)
            ;
            return services;
        }

        public static IServiceCollection AddAssetPersister(this IServiceCollection services, VosAssetOptions options = null, VosReference contextVob = null)
        {
            //.InitializeVob("/", v => v.AddOwn<VosAssetPersister>(), p => p.Key = $"/<VosAssetPersister>")

            var vob = contextVob ?? "/".ToVosReference();
            services.InitializeVob<IServiceProvider>(vob, (vob, serviceProvider) =>
            {
                vob.AddOwn<VosAssetPersister>(v =>
                {
                    return (VosAssetPersister)ActivatorUtilities.CreateInstance(serviceProvider, typeof(VosAssetPersister), options ?? new VosAssetOptions());
                });
                return;
            }, c=>c.Key = $"{vob}<VosAssetPersister>");
            return services;
        }
    }
}
