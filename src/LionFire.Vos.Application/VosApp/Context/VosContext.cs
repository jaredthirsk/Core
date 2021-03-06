﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LionFire.Collections;
using LionFire.Copying;
using LionFire.Dependencies;
using LionFire.ExtensionMethods.Copying;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    /// <summary>
    /// A context for VOS subpaths.  Typical assets have a path such as Map/Mapname.
    /// Optionally specify package and location to get a non-layered VobHandle, or else
    /// a layered VobHandle will be returned by Root.
    /// </summary>
    public class VosContext : IDisposable
    {

        #region Static

        /// <summary>
        /// The current context, using an AsyncLocal with a global static fallback of RootContext
        /// </summary>
        public static VosContext Current
        {
            get
            {
                var s = ContextStack;
#if SanityChecks
                if (s == null)
                {
                    throw new UnreachableCodeException("contextStack == null");
                }
                //if (rootContext == null) throw new UnreachableCodeException("rootContext == null");
#endif
                return ContextStack.Count > 0 ? ContextStack.Peek() : rootContext;
            }
        }

        private static AsyncLocal<Stack<VosContext>> contextStack = new AsyncLocal<Stack<VosContext>>();
        private static Stack<VosContext> ContextStack
        {
            get
            {
                if (contextStack.Value == null)
                {
                    contextStack.Value = new Stack<VosContext>();
                }
                return contextStack.Value;
            }
        }
                
        /// <summary>
        /// The default shared context for the process
        /// </summary>
        public static VosContext RootContext => rootContext;
        private static VosContext rootContext;
                          
        static VosContext()
        {
            contextStack = new AsyncLocal<Stack<VosContext>>();

#if AOT
//			System.Collections.Generic.Dictionary<int, System.Collections.Generic.Stack<LionFire.Vos.VosContext>> d4 = null;

//						Dictionary<int, System.Collections.Generic.Stack<LionFire.Vos.VosContext> > d1  = null;
//						NotifyCollectionChangedEventArgs<LionFire.Vos.Mount> d3 = new NotifyCollectionChangedEventArgs<LionFire.Vos.Mount> ();
//						LionFire.Collections.NotifyCollectionChangedHandler<LionFire.Vos.Mount> d2 = null;
//						System.Threading.Interlocked.CompareExchange
//							<LionFire.Collections.NotifyCollectionChangedHandler > 
//								(ref d2, d2,d2);
						
			//			CompareExchange 
			//				(LionFire.Collections.NotifyCollectionChangedHandler`1<LionFire.Vos.Mount>&,
			//				 LionFire.Collections.NotifyCollectionChangedHandler`1<LionFire.Vos.Mount>,
			//				 LionFire.Collections.NotifyCollectionChangedHandler`1<LionFire.Vos.Mount>)' 
#endif
            rootContext = new VosContext();

        }

        public static string DefaultPackage
        {
            get => rootContext.Package;
            set => rootContext.Package = value;
        }

        public static string DefaultLocation
        {
            get => rootContext.Store;
            set => rootContext.Store = value;
        }

        public static IVob DefaultRoot
        {
            get => defaultRoot;//return rootContext.Root; -
            set
            {
                //rootContext.Root = value;
                defaultRoot = value;
            }
        }
        private static IVob defaultRoot;

        #endregion

        #region Construction and Destruction

        public VosContext(string package = null, string location = null)
        {
            if (VosContext.Current != null)
            {
                // Alternative to using Overlay
                //this.AssignFrom(VosContext.Current, useICloneableIfAvailable: false);
                this.AssignFrom(VosContext.Current, AssignmentMode.Assign);
            }

            Package = package;
            Store = location;

            ContextStack.Push(this);
        }

#if AOT
		private void AssignFrom(VosContext o, bool useICloneableIfAvailable = false)
		{
			this.DisableWritingToDisk = o.DisableWritingToDisk;
			this.IgnoreReadonly = o.IgnoreReadonly;
			this.Package = o.Package;

			this.resolver = o.Resolver;
			this.Root = o.Root;
			this.Store = o.Store;
		}
#endif

        public void Dispose()
        {
            if (object.ReferenceEquals(this, rootContext))
            {
                throw new InvalidOperationException("Disposing root context is not allowed.");
            }

            var vosContext = ContextStack.Peek();
            if (Object.ReferenceEquals(vosContext, this))
            {
                ContextStack.Pop();
            }
            else
            {
                throw new InvalidOperationException("Cannot dispose context when it is not on top of the context stack.");
            }
        }

        #endregion

        private void UpdateRoot() => Root = Resolver?.GetVobRoot(null, this);

        #region Resolver

        public IVosContextResolver Resolver
        {
            get
            {
                if (resolver == null)
                {
                    return defaultResolver;
                }
                return resolver;
            }
            set
            {
                if (resolver == value)
                {
                    return;
                }

                if (resolver != default(IVosContextResolver))
                {
                    throw new NotSupportedException("Resolver can only be set once.");
                }

                resolver = value;
            }
        }
        private IVosContextResolver resolver;

        public static IVosContextResolver DefaultResolver
        {
            get
            {
                if (defaultResolver == null)
                {
                    defaultResolver = DependencyContext.Default.GetService<IVosContextResolver>();
                }
                return defaultResolver;
            }
        }
        private static IVosContextResolver defaultResolver;

        #endregion

        #region Context Properties

        #region Root

        public Vob Root
        {
            get
            {
                if (root == null)
                {
                    UpdateRoot();
                }
                return root;
            }
            private set => root = value;
        }
        private Vob root;

        #endregion

        #region Package

        public string Package
        {
            get => package;
            set
            {
                if (package == value)
                {
                    return;
                }

                package = value;
                UpdateRoot();
            }
        }
        private string package;

        #endregion

        #region Store

        public string Store
        {
            get => store;
            set
            {
                if (store == value)
                {
                    return;
                }

                store = value;
                UpdateRoot();
            }
        }
        private string store;

        #endregion



        //#region PopulatingArchive

        ///// <summary>
        ///// Location name
        ///// </summary>
        //public string PopulatingArchive
        //{
        //    get { return populatingArchive; }
        //    set 
        //    {
        //        if (populatingArchive == locked && value != null) throw new AlreadyException("Property is frozen");
        //        if (populatingArchive != value && populatingArchive != null) throw new AlreadyException("Already set");
        //        if (value == null)
        //        {
        //            populatingArchive = locked;
        //        }
        //        else
        //        {
        //            populatingArchive = value;
        //        }
        //    }
        //} private string populatingArchive;
        //private const string locked = "((frozen))";

        //#endregion

        public bool DisableWritingToDisk { get; set; }

        #region IgnoreReadonly

        public bool IgnoreReadonly
        {
            get => ignoreReadonly;
            set
            {
                //#if DEV
                ignoreReadonly = value;
                //#else
                //                if (value) l.Trace("IgnoreReadonly ignoring setting to true, as it is not supported in this build.");
                //#endif
            }
        }
        //#if DEV
        private bool ignoreReadonly;
        //#endif

        #endregion

        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}

