﻿using LionFire.Execution.Executables;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace LionFire.Execution
{


    public class RemoteExecutionControllerProxy : ExecutableExBase, IExecutionController
    {

        [SetOnce]
        public ExecutionContext ExecutionContext { get; set; }


        public Task<bool> Initialize()
        {
            throw new Exception("No remote transports available");
            //return false;
        }


        public Task StartAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren) { return Task.CompletedTask; }

    

    }
}
