﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LionFire.Hosting
{
    public static class LionFireLoggingExtensions
    {
        public static IHostBuilder AddLionFireLogging(this IHostBuilder builder, IConfiguration configuration)
            => builder.ConfigureServices(services =>
                     services.AddLogging(loggingBuilder =>
                     {
                         loggingBuilder
                            .ClearProviders()
                            .SetMinimumLevel(LogLevel.Trace)
                            .AddLionFireNLog(configuration);
                     })
                );
    }
}
