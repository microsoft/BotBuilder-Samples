// Copyright (c) Microsoft Corporation. All rights reserved.    
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RunBotServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Help();
            CreateHostBuilder(args).Build().Run();
        }

        public static void Help()
        {
            Console.WriteLine("--root <PATH>: Absolute path to the root directory for declarative resources all *.main.dialog be options.  Default current directory");
            Console.WriteLine("--region <REGION>: LUIS endpoint region.  Default westus");
            Console.WriteLine("--environment <ENVIRONMENT>: LUIS environment settings to use.  Default is user alias.");
            Console.WriteLine("--dialog <DIALOG>: Name of root dialog to run.  By default all root *.dialog will be choices.");
            Console.WriteLine("To use LUIS you should do 'dotnet user-secrets set --id RunBot luis:endpointKey <yourKey>'");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, builder) =>
            {
                var config = builder.Build();
                // NOTE: This should not be necessary
                if (config.GetValue<string>("environment") == "Development")
                {
                    var settings = new Dictionary<string, string>();
                    settings["environment"] = Environment.UserName;
                    builder.AddInMemoryCollection(settings);
                }
                builder.UseLuisSettings();
                builder.AddUserSecrets("RunBot");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
