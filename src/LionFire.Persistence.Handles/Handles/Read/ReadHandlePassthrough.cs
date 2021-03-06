﻿#nullable enable
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Convenience class that combines Reference and Handle.  
    /// Implementors can implement a static implicit operator from string to provide easy (implicit) conversion from a string to a particular IReference type.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    public class ReadHandlePassthrough<TValue, TReference> : IReadHandle<TValue>, IReferencable<TReference>, IHasReadHandle<TValue>
        where TReference : IReference<TValue>
    {
        public ReadHandlePassthrough() { }
        public ReadHandlePassthrough(IReadHandle<TValue> handle) { this.handle = handle; Reference = (TReference?) handle?.Reference; }


        #region Reference

        [SetOnce]
        public TReference? Reference
        {
            get => reference;
            set
            {
                
                if (EqualityComparer<TReference>.Default.Equals(reference, value)) return;
                if (!EqualityComparer<TReference>.Default.Equals(reference, default)) throw new AlreadySetException();
                reference = value;
            }
        }
        private TReference? reference;

        IReference<TValue> IReferencableAsValueType<TValue>.Reference => Reference;

        #endregion

        [Ignore(LionSerializeContext.AllSerialization)]
        public IReadHandle<TValue>? ReadHandle => handle ??= Reference?.GetReadHandle<TValue>();
        protected IReadHandle<TValue>? handle;
        IReadHandleBase<object> IHasReadHandle.ReadHandle => (IReadHandle<object>)ReadHandle;


        public Type? Type => ReadHandle?.Type;

        IReference IReferencable.Reference => ReadHandle.Reference;

        public string? Key => ReadHandle?.Key;

        public PersistenceFlags Flags => ReadHandle.Flags;

        public TValue Value
        {
            get => ReadHandle == null ? default : ReadHandle.Value;
            protected set
            {
                if (handle != null) throw new AlreadySetException($"{nameof(ReadHandle)} is already set");
                if(Reference != null)
                {
                handle = Reference.GetReadHandle<TValue>(value);
                } else
                {
                    handle = value.ToObjectHandle();
                }
            }
        }

        public bool HasValue => ReadHandle?.HasValue == true;

        IReadHandle<TValue> IHasReadHandle<TValue>.ReadHandle => ReadHandle;

        public ITask<IResolveResult<TValue>> Resolve() => ReadHandle.Resolve();
        public ITask<ILazyResolveResult<TValue>> TryGetValue() => ReadHandle.TryGetValue();
        public ILazyResolveResult<TValue> QueryValue() => ReadHandle.QueryValue();
        public void DiscardValue() => ReadHandle.DiscardValue();

    }
}
