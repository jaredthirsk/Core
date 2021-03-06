﻿//using LionFire.Dependencies;
//using LionFire.ObjectBus;
//using LionFire.Referencing;
//using System;

//namespace LionFire.Referencing
//{
//    //    /// <summary>
//    // OLD - Migrated to a shim to HandleFactory<object>
//    //    /// Caches Reference URIs to IHandles

//    //    /// </summary>
//    //    public static class HandleFactory
//    //    {
//    //        private static Dictionary<string, IH<object>> handlesByUri = new Dictionary<string, IH<object>>();
//    //        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();
//    //        public static bool ShareHandles = true;

//    //        public static IH<object> ToHandle(IReference reference, object obj = null)
//    //        {
//    //            if (ShareHandles)
//    //            {
//    //                try
//    //                {
//    //                    handlesLock.EnterUpgradeableReadLock();
//    //                    IH<object> handle = 
//    //#if AOT
//    //						(IHandle)
//    //#endif
//    //						handlesByUri.TryGetValue(reference.Uri);

//    //                    if (handle != null)
//    //                    {
//    //                        return handle;
//    //                    }
//    //                    else
//    //                    {
//    //                        //return CreateHandle(reference); TODO
//    //                        handle = new H<object>(reference, obj);
//    //                        handlesLock.EnterWriteLock();
//    //                        try
//    //                        {
//    //                            handlesByUri.Add(reference.Uri, handle);
//    //                        }
//    //                        finally
//    //                        {
//    //                            handlesLock.ExitWriteLock();
//    //                        }
//    //                        return handle;
//    //                    }
//    //                }
//    //                finally
//    //                {
//    //                    if (handlesLock.IsUpgradeableReadLockHeld)
//    //                    {
//    //                        handlesLock.ExitUpgradeableReadLock();
//    //                    }
//    //                }
//    //            }
//    //            else
//    //            {
//    //                return CreateHandle(reference);
//    //            }
//    //        }

//    //        public static IH<object> CreateHandle(IReference reference)
//    //        {
//    //            var handle = new H<object>(reference);

//    //            //if (ShareHandles && handlesByUri.ContainsKey(reference.Uri)) // TODO: With locking
//    //            //{
//    //            //    handlesByUri.Add(reference.Uri, handle);
//    //            //}
//    //            return handle;
//    //        }
//    //    }

//    /// <summary>
//    /// REVIEW - keep this?  What about ToReadHandle?
//    /// </summary>
//    public static class HandleProviderOBusExtensions
//    {
//        //public static H<T> ToHandle<T>(this string uriString) where T : class => DependencyContext.Current.GetService<IReadWriteHandleProvider>().ToHandle<T>(new UriStringReference(uriString));

//        public static H<object> ToHandle(this IReference reference, object obj = null) => reference.GetOBase().ToHandle<object>(reference);
//        public static H<T> ToHandle<T>(this IReference reference, object obj = null) => reference.GetOBase().ToHandle<object>(reference);

//        //      public static H<T> ToHandle<T>(this IReferencable referencable, T obj = null)
//        //          where T : class//, new()
//        //      {
//        //          return HandleProvider<T>.ToHandle(referencable.Reference, obj);
//        //      }

//        //      public static H ToHandle(this IReference reference, object obj , Type type)
//        //{
//        //	// TODO: Get HandleFactory<T> via reflection, perhaps only if not AOT
//        //	return HandleProvider.ToHandle(reference, obj);
//        //}

//        //public static H CreateHandle(this IReference reference)
//        //{
//        //    return HandleFactory.CreateHandle(reference);
//        //}
//        //#if !AOT
//        //        public static H <T> CreateHandle<T>(this IReference reference)
//        //            where T : class, new()
//        //        {
//        //            return HandleFactory<T>.CreateHandle(reference);
//        //        }
//        //#endif
//        //        public static H ToHandle(this IReferencable referencable)
//        //        {
//        //            return HandleProvider.ToHandle(referencable.Reference);
//        //        }
//    }
//}
