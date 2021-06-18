using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Runtime.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ControllingConversationFlowSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    var applicationRoot = AppDomain.CurrentDomain.BaseDirectory;
                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    var settingsDirectory = "settings";

                    builder.AddBotRuntimeConfiguration(applicationRoot, settingsDirectory, environmentName);

                    builder.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}