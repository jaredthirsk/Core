﻿using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Assets.Persisters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Assets.Handles
{
    public class VosAssetHandleProvider : IReadHandleProvider<IAssetReference>, IReadWriteHandleProvider<IAssetReference>
    {
        IPersisterProvider<IAssetReference> PersisterProvider { get; }
        public VosAssetHandleProvider(IPersisterProvider<IAssetReference> persisterProvider)
        {
            PersisterProvider = persisterProvider;
        }

        public IReadHandle<T> GetReadHandle<T>(IAssetReference reference)
            => new PersisterReadHandle<IAssetReference, T, VosAssetPersister>((VosAssetPersister)PersisterProvider.GetPersister(reference.Persister), reference);
        public IReadHandle<T> GetReadHandle<T>(IReference reference)
            => reference is IAssetReference iar ? GetReadHandle<T>(iar) : null;

        public Persistence.IReadWriteHandle<T> GetReadWriteHandle<T>(IAssetReference reference)
            => new PersisterReadWriteHandle<IAssetReference, T, VosAssetPersister>((VosAssetPersister)PersisterProvider.GetPersister(reference.Persister), reference);
    }
}