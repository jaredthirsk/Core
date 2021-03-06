﻿using LionFire.ExtensionMethods.Resolves;
using LionFire.Resolves;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    //public abstract class Mapper<TItem, TUnderlying, TUnderlyingCollection> : IEnumerable<TItem>
    //{
    //    public TUnderlyingCollection UnderlyingHandle => underlying;
    //    TUnderlyingCollection underlying;

    //    public Mapper(TUnderlyingCollection UnderlyingHandle)
    //    {
    //        underlying = UnderlyingHandle;
    //    }

    //    public abstract TItem Map(TUnderlying underlying);
    //    public IEnumerator<TItem> GetEnumerator() => throw new NotImplementedException();
    //    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    //}

    //#error NEXT 
    // Figure out where to do async, maybe multiple classes to do it in multiple locations.
    // Typical: resolve listings, as well as children all at once.  sync enumerate over children
    // Typical2: resolve listings, start retrieving all children all at once, async enumerate over children
    // Full async: resolve listings, start retrieving all children all at once, async enumerate over children


    public abstract class AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> : IAsyncEnumerable<TItem>, IResolves<TResolvedUnderlyingCollection>, ILazilyResolves<TResolvedUnderlyingCollection>
        //where TUnderlyingCollectionResolvable : IResolves<TUnderlyingCollection>
        where TUnderlyingCollection : IEnumerable<TUnderlying>
    {
        #region Static

        #region Unwrap

        protected static readonly Func<TResolvedUnderlyingCollection, TUnderlyingCollection> UnwrapUnderlyingCollection;

        protected static Func<TResolvedUnderlyingCollection, TUnderlyingCollection> UnwrapUnderlyingCollectionUnwrap = (TResolvedUnderlyingCollection resolved) =>
        {
            if (resolved is IReadWrapper<TUnderlyingCollection> rw) { return rw.Value; }

            throw new NotImplementedException($"Don't know how to unwrap {typeof(TResolvedUnderlyingCollection).FullName} to {typeof(TUnderlyingCollection).FullName }. Override UnwrapUnderlyingCollection.");
        };

        protected static Func<TResolvedUnderlyingCollection, TUnderlyingCollection> UnwrapUnderlyingCollectionNoop = (TResolvedUnderlyingCollection resolved) => (TUnderlyingCollection)(object)resolved; // HARDCAST

        #endregion

        static AsyncMapper()
        {
            UnwrapUnderlyingCollection = typeof(TResolvedUnderlyingCollection) == typeof(TUnderlyingCollection) ? UnwrapUnderlyingCollectionNoop : UnwrapUnderlyingCollectionUnwrap;
        }

        #endregion
        public IResolves<TResolvedUnderlyingCollection> UnderlyingResolves => underlyingResolves;
        IResolves<TResolvedUnderlyingCollection> underlyingResolves;
        public ILazilyResolves<TResolvedUnderlyingCollection> UnderlyingLazilyResolves => underlyingResolves as ILazilyResolves<TResolvedUnderlyingCollection>;

        private TResolvedUnderlyingCollection ResolvedUnderlyingCollection { get; set; }
        private TUnderlyingCollection UnderlyingCollection { get; set; }

        public TResolvedUnderlyingCollection Value { get; private set; }

        public bool HasValue { get; private set; }

        #region Construction

        public AsyncMapper(IResolves<TResolvedUnderlyingCollection> underlyingResolves)
        {
            this.underlyingResolves = underlyingResolves;
        }

        #endregion

        

        #region Abstract

        public abstract Task<TItem> Map(TUnderlying underlying);

        #endregion

        #region Enumerator

        private class AsyncEnumerator : IAsyncEnumerator<TItem>
        {
            AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> parent;

            ValueTask<TUnderlyingCollection>? getcol;

            public AsyncEnumerator(AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> parent)
            {
                this.parent = parent;

                var query = parent.QueryValue();

                getcol = query.HasValue ? new ValueTask<TUnderlyingCollection>(AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection>.UnwrapUnderlyingCollection(query.Value)) : new ValueTask<TUnderlyingCollection>(Task.Run(async () =>
                   {
                       var p = await parent.TryGetValue();
                       return AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection>.UnwrapUnderlyingCollection(p.Value);
                   }));
            }

            public TItem Current { get; private set; }

            public ValueTask DisposeAsync()
            {
                getcol = null;
                return new ValueTask();
            }

            IEnumerator<TUnderlying> enumerator;

            public async ValueTask<bool> MoveNextAsync()
            {
                if (!getcol.HasValue && enumerator == null) throw new ObjectDisposedException("");

                if (enumerator == null)
                {
                    TUnderlyingCollection e = (await getcol.Value.ConfigureAwait(false));
                    if(e == null) { return false; }
                    enumerator = e.GetEnumerator();
                    getcol = null;
                }

                if (!enumerator.MoveNext()) return false;

                Current = await parent.Map(enumerator.Current).ConfigureAwait(false);

                return true;
            }
        }

        public IAsyncEnumerator<TItem> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator(this);

        #endregion

        #region Resolve

        public async ITask<IResolveResult<TResolvedUnderlyingCollection>> Resolve()
        {
            var result = await underlyingResolves.Resolve();

            UnderlyingCollection = UnwrapUnderlyingCollection(result.Value);

            return result;
        }

        #region ILazilyResolves

        public async ITask<ILazyResolveResult<TResolvedUnderlyingCollection>> TryGetValue()
        {
            var ulr = UnderlyingLazilyResolves;
            if (ulr != null)
            {
                return await ulr.TryGetValue().ConfigureAwait(false);
            }
            else
            {
                return HasValue ? new LazyResolveResult<TResolvedUnderlyingCollection>(true, Value) : (await underlyingResolves.Resolve().ConfigureAwait(false)).ToLazyResolveResult();
            }
        }

        public ILazyResolveResult<TResolvedUnderlyingCollection> QueryValue()
        {
            var ulr = UnderlyingLazilyResolves;
            if (ulr != null)
            {
                return ulr.QueryValue();
            }
            else
            {
                return HasValue ? new LazyResolveResult<TResolvedUnderlyingCollection>(true, Value) : NoopFailResolveResult<TResolvedUnderlyingCollection>.Instance;
            }
        }

        public void DiscardValue()
        {
            var ulr = UnderlyingLazilyResolves;
            if (ulr != null)
            {
                ulr.DiscardValue();
            }
            Value = default;
            HasValue = false;
        }

        #endregion

        #endregion
    }

    //public class ReadHandles<TItem> : IAsyncEnumerable<TItem>
    //    //RCollectionBase<TReference, IReadHandle<Metadata<IEnumerable<Listing<T>>>>, TItem>, HC<TReference,string>
    //    , IHasUnderlyingReadHandle<Metadata<IEnumerable<Listing<TItem>>>>
    //{
    //    public IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> UnderlyingHandle => listings;
    //    IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> listings;

    //    public IAsyncEnumerator<TItem> GetAsyncEnumerator(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    //    public ReadHandles(IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> listings)
    //    {
    //        this.listings = listings;
    //    }

    //}

}
