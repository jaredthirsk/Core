﻿using System;
using System.Collections.Immutable;

namespace LionFire.Referencing
{
    public abstract class OverlayableReferenceBase<TReference> : IOverlayableReference<TReference>
        where TReference : IReference
    {
        //ImmutableArray<IReference> references;

        public abstract ReferenceOverlayMode OverlayMode { get; }

        public abstract TReference Reference { get; }
        IReference IReferencable.Reference => Reference;

        public abstract int Count { get; }

        public virtual IReferencable<TReference> AddLeft(IReference reference) => throw new NotSupportedException();
        public virtual IReferencable<TReference> AddRight(IReference reference) => throw new NotSupportedException();
        public virtual IReferencable<TReference> PopLeft(IReference reference) => throw new NotSupportedException();
        public virtual IReferencable<TReference> PopRight(IReference reference) => throw new NotSupportedException();

    }

    public abstract class BaseReference<TReference> : OverlayableReferenceBase<TReference>
        where TReference : IReference
    {
        public override ReferenceOverlayMode OverlayMode => ReferenceOverlayMode.CanAddRight;
    }

}
