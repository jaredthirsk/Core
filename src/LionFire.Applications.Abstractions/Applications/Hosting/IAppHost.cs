﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Applications.Hosting;

namespace LionFire.Applications.Hosting
{
    public interface IAppHost
    {
        #region Dependency Injection

        IServiceCollection ServiceCollection { get; }

        IServiceProvider ServiceProvider { get; }

        #endregion

        #region Configuration

        IDictionary<string, object> Properties { get; }

        T Add<T>(T component)
            where T : IAppComponent;

        #endregion

            #region Execution

        /// <summary>
        /// Optionally call this to prepare the application to run without running it.  If it is not invoked by the user, it will be invoked from the Run() method.  Invokations of this after initialization has completed will be ignored.  
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Start application and return a task that waits for all ApplicationTasks with WaitForComplete = true to complete.
        /// (Also see Run extension method which will block until the application completes.)
        /// </summary>
        /// <returns></returns>
        Task Start();

        #endregion

        #region Shutdown

        //IObservable<bool> IsShuttingDown { get; }

        /// <summary>
        /// Sets IsShuttingDown to true.  All components that run perpetually until shutdown should monitor this flag and shut down in a timely manner.
        /// </summary>
        Task Shutdown(long millisecondsTimeout = 0);

        #endregion
    }
}

namespace LionFire.Applications
{
    public static class IAppHostExtensions
    {
        /// <summary>
        /// Adds a task to the application
        /// </summary>
        public static IAppHost Add(this IAppHost host, Action action, Func<bool> tryInitialize = null)
        {
            host.Add(new AppTask(action, tryInitialize));
            return host;
        }

        public static IAppHost AddConfig(this IAppHost host, Action<IAppHost> tryInitialize)
        {
            host.Add(new AppConfigurer(tryInitialize));
            return host;
        }

        public static IAppHost AddInit(this IAppHost host, Func<IAppHost, bool> tryInitialize)
        {
            host.Add(new AppInitializer(tryInitialize));
            return host;
        }
        public static IAppHost AddInit(this IAppHost host, Action<IAppHost> initialize)
        {
            host.Add(new AppInitializer(initialize));
            return host;
        }

        /// <summary>
        /// Start application and wait until all ApplicationTasks with WaitForComplete = true to complete.
        /// </summary>
        public static void Run(this IAppHost host)
        {
            host.Start().Wait();
        }


    }
}