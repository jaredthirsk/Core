﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public static class IExecutableExtensions
    {

        /// <summary>
        /// Returns true if a request to start has been received, and no request to stop has been received.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsStarted(this IExecutable e)
        {
            switch (e.State.Value)
            {
                //case ExecutionState.Unspecified:
                //    break;
                //case ExecutionState.Unconfigured:
                //    break;
                //case ExecutionState.InvalidConfiguration:
                //    break;
                //case ExecutionState.Uninitialized:
                //    break;
                //case ExecutionState.Initializing:
                //    break;
                //case ExecutionState.Ready:
                //    break;
                //case ExecutionState.Stopping:
                //    break;
                //case ExecutionState.Stopped:
                //    break;
                //case ExecutionState.Finished:
                //    break;
                //case ExecutionState.Disposed:
                //    break;
                case ExecutionState.Starting:
                case ExecutionState.Started:
                case ExecutionState.Pausing:
                case ExecutionState.Paused:
                case ExecutionState.Unpausing:
                case ExecutionState.WaitingToStart:
                    return true;
                default:
                    return false;
            }
        }
    }
}
