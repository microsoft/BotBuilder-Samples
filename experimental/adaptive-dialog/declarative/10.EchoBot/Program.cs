// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // add luis LU model environment settings
                    var env = hostingContext.HostingEnvironment;
                    var luisAuthoringRegion = Environment.GetEnvironmentVariable("LUIS_AUTHORING_REGION") ?? "westus";
                    config.AddJsonFile($"luis.settings.{env.EnvironmentName}.{luisAuthoringRegion}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"luis.settings.{Environment.UserName}.{luisAuthoringRegion}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
                })
            .UseStartup<Startup>()
            .Build();
    }
}
