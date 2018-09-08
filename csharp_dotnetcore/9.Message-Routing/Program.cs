// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MessageRoutingBot
{
    /// <summary>
    /// Entry point for Bot application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for Bot application.
        /// </summary>
        /// <param name="args">Arguments to bot application</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Builds a web host to expose the bot application.
        /// </summary>
        /// <param name="args">Arguments to the web host.</param>
        /// <returns>Built <see cref="IWebHost" /> to expose the bot application.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}
