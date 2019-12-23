﻿using LionFire.Data;
using LionFire.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Redis
{
    // FUTURE - state machine?
    //public enum ConnectionStates
    //{
    //    Unspecified,
    //    Connected,
    //    Connecting,
    //    WaitingToReconnect,
    //    Reconnecting,
    //    FailedToConnect,
    //    Disconnected,
    //}

    /// <summary>
    /// Connection string:
    ///   Comma separated host:port
    ///   E.g. "server1:6379,server2:6379"
    ///   Order not important; master is automatically identified
    /// </summary>
    public class RedisConnection : OptionsConnectionBase<RedisConnectionOptions, RedisConnection>
    {

        public IDatabase Db => redis.GetDatabase();
        public ConnectionMultiplexer Redis => redis;
        private ConnectionMultiplexer redis;

        public RedisConnection(string name, IOptionsMonitor<NamedConnectionOptions<RedisConnectionOptions>> options, ILogger<RedisConnection> logger) : base(name, options, logger)
        {
        }

        public bool IsConnectionDesired
        {
            get => isConnectionDesired;
            set
            {
                if (value)
                {
                    ConnectImpl().FireAndForget();
                }
                else
                {
                    DisconnectImpl().FireAndForget();
                }
            }
        }
        private bool isConnectionDesired;
        private Task<ConnectionMultiplexer> connectingTask;

        public override async Task ConnectImpl(CancellationToken cancellationToken = default(CancellationToken))
        {
        start:
            #region Detect already done or in progress REVIEW

            if (redis != null)
            {
                return;
            }

            if (connectingTask != null)
            {
                var copy = connectingTask;
                if (copy != null)
                {
                    await copy;
                    return;
                }
                else
                {
                    goto start;
                }
            }

            #endregion

            isConnectionDesired = true;
            logger.LogDebug($"[CONNECTING] Connecting to redis at {ConnectionString}...");
            connectingTask = ConnectionMultiplexer.ConnectAsync(ConnectionString);
            redis = connectingTask.Result;
            connectingTask = null;
            logger.LogInformation($"[connected] ...connected to redis at {ConnectionString}");
        }

        public override async Task DisconnectImpl(CancellationToken cancellationToken = default(CancellationToken))
        {
            isConnectionDesired = false;
            if (redis != null)
            {
                var redisCopy = redis;
                redis = null;
                try
                {
                    logger.LogDebug($"[DISCONNECTING] Disconnecting from redis at {ConnectionString}...");
                    await redisCopy.CloseAsync(true);
                    logger.LogInformation($"[disconnected] ...disconnected from redis at {ConnectionString}");
                }
                finally
                {
                    redisCopy.Dispose();
                }
            }
        }


    }
}
