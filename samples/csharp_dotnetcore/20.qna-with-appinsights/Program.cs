// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is an ASP.NET Core app that creates a webserver.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// This is the entry point for your application.
        /// It is run first once your application is started.
        /// This method creates a web server.
        /// </summary>
        /// <param name="args">Arguments for this method.</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Hosting.WebHostBuilder" />
        /// class with pre-configured defaults.
        /// The UseStartup method on WebHost specifies the Startup class for your app.
        /// </summary>
        /// <param name="args">Arguments for this method.</param>
        /// <returns>A web server.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
