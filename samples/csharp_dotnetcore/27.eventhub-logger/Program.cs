// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((logging) =>
                {
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.AddApplicationInsights();

                    // Optional: Apply filters to configure LogLevel Information or above is sent to
                    // ApplicationInsights for all categories.
                    logging.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
                });
    }
}
