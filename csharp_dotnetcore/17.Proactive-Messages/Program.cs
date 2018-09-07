// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
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
