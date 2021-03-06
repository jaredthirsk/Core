﻿#if false
using LionFire.CommandLine;
using LionFire.CommandLine.Arguments;

using LionFire.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace LionFire.Execution.Hosting
{
    public class CliApp
    {
        public void Run(string[] args)
        {
            CommandLine.Dispatching.CliDispatcher.DefaultOptions.AdditionalHandlerTypes.Add(typeof(LionFire.Environment.LionEnvironmentCliHandlers));

            //Console.WriteLine(typeof(LionFire.Environment.LionEnvironmentCliHandlers).AssemblyQualifiedName);
            CommandLine.Dispatching.CliDispatcher.Dispatch(args, this);
            LionEnvironment.StandardOutput.Flush();
        }
    }

    public class HostCli : CliApp
    {
        #region State

        Dictionary<string, ServiceState> hostedServices = new Dictionary<string, ServiceState>();
        Dictionary<string, Task> startTasks = new Dictionary<string, Task>();
        Dictionary<string, Task> runTasks = new Dictionary<string, Task>();
        Dictionary<string, Task> stopTasks = new Dictionary<string, Task>();

        //ServiceHost

        #endregion


        #region Command Line Options

        bool IsTestOptionEnabled = false;
        [CliOption(ShortForm = 't')]
        public void TestOption(string[] args)
        {
            Console.WriteLine("TESTOPTION");
            IsTestOptionEnabled = true;
        }

        [CliOption(ShortForm = '2')]
        public bool IsTestOption2Enabled { get; set; } = false;

        [CliOption(ShortForm = '3')]
        public string TestOption3 { get; set; } = "TestOption3Default";

        #endregion

        #region In Process

        private void StartInProcess(ServiceConfig sc)
        {
            string serviceKey = sc.Key;

            var serviceState = hostedServices.TryGetValue(sc.Key);
            if (serviceState == null)
            {
                serviceState = new ServiceState(sc);
                hostedServices.Add(sc.Key, serviceState);
            }
            if (serviceState.IsActive) return;

            startTasks.Add(sc.Key, Task.Factory.StartNew(() =>
            {
                if (typeof(IService).IsAssignableFrom(sc.Class))
                {
                    serviceState.Status = ServiceStatus.Initializing;
                    serviceState.ServiceObject = Activator.CreateInstance(sc.Class);
                    serviceState.Status = ServiceStatus.Initialized;

                    IService svc = obj as IService;
                    serviceState.Status = ServiceStatus.Starting;
                    svc.Start();
                    serviceState.Status = ServiceStatus.Started;
                }
                else
                {
                    serviceState.Status = ServiceStatus.Starting;
                    serviceState.ServiceObject = Activator.CreateInstance(sc.Class);
                     
                    serviceState.Status = ServiceStatus.Started;
                }
            }));
        }

        #endregion


        public bool IsServiceRunning(ServiceConfig sc)
        {
            switch (sc.ExecutionLocationType)
            {
                case ExecutionLocationType.InProcess:
                    // Check in-process
                    var serviceState = hostedServices.TryGetValue(sc.Key);
                    return serviceState != null && serviceState.IsActive;
                //case ExecutionLocationType.CurrentUser:
                //    break;
                //case ExecutionLocationType.LocalMachine:
                //    break;
                //case ExecutionLocationType.Hive:
                //    break;
                //case ExecutionLocationType.Global:
                //    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #region Command Line Verbs


        [CliVerb]
        public void Start(params string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("TODO: Start default configuration");
                return;
            }

            var conf = new ServiceConfig(args[1]);

            if (IsServiceRunning(conf))
            {
                Console.WriteLine("[start] Already running: " + conf);
                return;
            }

            switch (conf.ExecutionLocationType)
            {
                case ExecutionLocationType.InProcess:
                    if (!conf.TryResolveForInProcess())
                    {
                        Console.WriteLine("ERROR: Failed to resolve: " + args[1]);
                        return;
                    }
                    StartInProcess(conf);
                    break;
                //case ExecutionLocationType.CurrentUser:
                //    break;
                //case ExecutionLocationType.LocalMachine:
                //    break;
                //case ExecutionLocationType.Hive:
                //    break;
                //case ExecutionLocationType.Global:
                //    break;
                default:
                    throw new NotImplementedException();
            }

            //hostedServices.Add();

            //var psi = new ProcessStartInfo("dotnet");
            //psi.RedirectStandardOutput = true;
            //var p = Process.Start(psi);
            //while (!p.HasExited)
            //{
            //    for (string line = p.StandardOutput.ReadLine(); line != null; line = p.StandardOutput.ReadLine())
            //    {
            //        if (line != null)
            //        {
            //            LionEnvironment.StandardOutput.WriteLine("child: " + line);
            //        }
            //    }
            //    Thread.Sleep(1000);
            //}

            //var sh = new ServiceHost();
            //var sc = new ServiceConfig("LionFire.Services.Tests", "LionFire.Services.Tests.TestService", "TestParameter")
            //{
            //};
            //sh.Start(sc);

            WaitForTasksToComplete();
        }

     
        #endregion


    }
}

#endif