// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EnterpriseBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Add Azure Logging
                    logging.AddAzureWebAppDiagnostics();

                    // Other Loggers.
                    // There are other logging options available:
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1
                    // logging.AddDebug();
                    // logging.AddConsole();
                })
                .UseStartup<Startup>() // Note: Application Insights is added in Startup.  Disabling is also handled there.
                .Build();
    }
}
