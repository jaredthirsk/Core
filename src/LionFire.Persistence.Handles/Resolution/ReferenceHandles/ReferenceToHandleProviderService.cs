﻿using LionFire.Dependencies;
using LionFire.ExtensionMethods;
using LionFire.Ontology;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// 
    /// </summary>
    /// <design>
    /// Thoughts:
    ///  - should be fast
    ///  - having multiple resolvers per IReference type and/or URI Scheme can provide flexibility:
    ///    - overloading URI Schemes with multiple implementations
    ///    - caching layer -- or should that be done at the OBase configuration level?
    ///  - On the flip side, I may always want one IReference to predictably resolve to one handle.
    ///  - CreateHandle (new) vs ToHandle (probably/possibly reused)
    /// 
    /// 
    /// </design>
    public class ReferenceToHandleService : IReferenceToHandleService
    {

        #region Lookup types

        /// <summary>
        /// For types of type Key, Use the Value type when looking for IReadHandleProvider&lt;T&gt;
        /// </summary>
        ConcurrentDictionary<Type, Type> LookupTypes { get; } = new ConcurrentDictionary<Type, Type>();

        private Type GetLookupType(Type type) 
            => LookupTypes.GetOrAdd(type, t => type.GetCustomAttribute<TreatAsAttribute>()?.Type ?? GetNonGenericType(type));

        private static Type GetNonGenericType(Type type)
        {
            while (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            return type;
        }

        #endregion

        //public static IReferenceToHandleService Current => DependencyLocator.TryGet<IReferenceToHandleService>();

        //IOptionsFactory<NamedHandleProviderConfig> optionsFactory;
        IServiceProvider ServiceProvider { get; }
        public ReferenceToHandleService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        //public ReferenceToHandleService(IOptionsFactory<NamedHandleProviderConfig> optionsFactory)
        //{
        //    this.optionsFactory = optionsFactory;
        //}
#warning TODO: If this method is okay, do the same generic parameter for the others
        public IReadHandleProvider<TReference> GetReadHandleProvider<TReference>(TReference input)
            where TReference : IReference
        {
            // REVIEW - is this IReference check needed? What is performance?
            //if (typeof(TReference) == typeof(IReference))
            //{
            //// Question: Handle named providers here, or let each provider type do it?
            //return ServiceProvider.GetRequiredService<IReadHandleProvider>(typeof(IReadHandleProvider<>).MakeGenericType(input.GetType()));
            //}
            //else
            {
                if (typeof(TReference).IsGenericType)
                {
                    throw new ArgumentException("Cannot be invoked when TReference is generic.  Use non-generic overload of this method instead.");
                }
                //else
                //{
                return ServiceProvider.GetRequiredService<IReadHandleProvider<TReference>>(typeof(IReadHandleProvider<TReference>));
                //}
            }
        }

        public IReadHandleProvider GetReadHandleProvider(IReference input)
        {
            // Question: Handle named providers here, or let each provider type do it?
            return ServiceProvider.GetRequiredService<IReadHandleProvider>(typeof(IReadHandleProvider<>).MakeGenericType(GetLookupType(input.GetType())));
        }

        public IReadWriteHandleProvider GetReadWriteHandleProvider(IReference input)
            => ServiceProvider.GetRequiredService<IReadWriteHandleProvider>(typeof(IReadWriteHandleProvider<>).MakeGenericType(GetLookupType(input.GetType())));

        //if (handleProviders.TryGetValue(input.GetType(), out IReadWriteHandleProvider result))
        //{
        //    return result;
        //}

        //public IReadHandleProvider GetReadHandleProvider<TReference>(TReference input)
        //    where TReference: IReference
        //{
        //    ServiceProvider.GetRequiredService<IReadHandleProvider<TReference>>(typeof(IReadHandleProvider<TReference>));
        //}

        public IWriteHandleProvider GetWriteHandleProvider(IReference input)
                    => ServiceProvider.GetRequiredService<IWriteHandleProvider>(typeof(IWriteHandleProvider<>).MakeGenericType(GetLookupType(input.GetType())));

        public ICollectionHandleProvider GetCollectionHandleProvider(IReference input)
            => ServiceProvider.GetRequiredService<ICollectionHandleProvider>(typeof(ICollectionHandleProvider<>).MakeGenericType(GetLookupType(input.GetType())));

        public IReadHandleCreator GetReadHandleCreator(IReference input) => throw new NotImplementedException();
        public IReadHandleCreator<TReference> GetReadHandleCreator<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IReadWriteHandleProvider<TReference> GetReadWriteHandleProvider<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IReadWriteHandleCreator GetReadWriteHandleCreator(IReference input) => throw new NotImplementedException();
        public IReadWriteHandleCreator<TReference> GetReadWriteHandleCreator<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IWriteHandleProvider<TReference> GetWriteHandleProvider<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IWriteHandleCreator GetWriteHandleCreator(IReference input) => throw new NotImplementedException();
        public IWriteHandleCreator<TReference> GetWriteHandleCreator<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();

        /// ///////////////////


        //ConcurrentDictionary<Type,  List<IReadWriteHandleProvider>> handleProviders = new ConcurrentDictionary<Type, List<IReadWriteHandleProvider>>();
        //ConcurrentDictionary<Type,  List<IReadHandleProvider>> readHandleProviders = new ConcurrentDictionary<Type, List<IReadHandleProvider>>();

        //ConcurrentDictionary<Type, SortedList<double, IReadWriteHandleProvider>> handleProvidersRanked = new ConcurrentDictionary<Type, SortedList<double, IReadWriteHandleProvider>>();
        //ConcurrentDictionary<Type, SortedList<double, IReadHandleProvider>> readHandleProvidersRanked = new ConcurrentDictionary<Type, SortedList<double, IReadHandleProvider>>();

        //public void Reset()
        //{
        //    handleProviders.Clear();
        //    readHandleProviders.Clear();
        //}

        //public IReferenceToHandleProviderProvider HPP(IReference input)
        //{
        //    ServiceProvider.GetRequiredService<><>()
        //}


        //public IReadHandleProvider GetReadHandleProvider(IReference input) => throw new System.NotImplementedException();

        //public IEnumerable<IReadWriteHandleProvider> GetHandleProviders(IReference input)=>handleProviders
        //public IEnumerable<IReadHandleProvider> GetReadHandleProviders(IReference input);

    }


}
