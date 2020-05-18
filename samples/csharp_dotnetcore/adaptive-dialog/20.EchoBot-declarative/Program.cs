// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration((hostingConetxt, config) =>
                {
                    var di = new DirectoryInfo(".");
                    foreach (var file in di.GetFiles($"appsettings.json", SearchOption.AllDirectories))
                    {
                        var relative = file.FullName.Substring(di.FullName.Length);
                        if (!relative.Contains("bin\\") && !relative.Contains("obj\\"))
                        {
                            config.AddJsonFile(file.FullName, optional: false, reloadOnChange: true);
                        }
                    }
                })

                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    builder.UseLuisSettings();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((logging) =>
                    {
                        logging.AddDebug();
                        logging.AddConsole();
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
