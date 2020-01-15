﻿using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public class VosStoresBuilder : IVosStoresBuilder
    {
        IServiceCollection Services;
        VosStoresOptions Options;
        public VosStoresBuilder(IServiceCollection services, VosStoresOptions options)
        {
            Services = services;
            Options = options;
        }

        public static MountOptions DefaultAvailablePackageMountOptions => new MountOptions { IsManuallyEnabled = true };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeName"></param>
        /// <param name="target"></param>
        /// <param name="dataMountOptions">Set the ReadPriority and WritePriority that will take effect in the PackageManager's data location here.  It is recommended to specify a Read (and optionally Write) priority here.</param>
        /// <param name="availablePackageMountOptions">This is the MountOptions for the store being mounted under the PackageManager's available location.  It is recommended to leave this as null.  Default will have IsManuallyEnabled = true.</param>
        /// <param name="rootName">Specify a non-default RootVob here.</param>
        /// <returns></returns>
        public IVosStoresBuilder AddStore(string storeName, IReference target, MountOptions dataMountOptions = null, MountOptions availablePackageMountOptions = null, string rootName = VosConstants.DefaultRootName)
        {
            //Services.InitializeRootVob((serviceProvider, root) =>
            //{
            //    var vosStoreOptions = root.GetOwn<VosStoresOptions>();
            //    var mountPoint = vosStoreOptions.StoresLocation.ToVob(); // Note: might be in another rootName!

            //}, rootName: rootName);
            var availablePackageLocation = Options.AvailableLocation.GetChild(storeName);
            Services.VosMount(availablePackageLocation, target, availablePackageMountOptions ?? DefaultAvailablePackageMountOptions);
            if (dataMountOptions != null) Services.InitializeVob(availablePackageLocation, (_, v) => v.AddOwn(_ => dataMountOptions));
            return this;
        }
    }
}
