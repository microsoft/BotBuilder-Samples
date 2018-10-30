// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ScaleoutBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //var accountName = "<ACCOUNT-NAME>";
            //var accountKey = "<ACCOUNT-KEY>";
            //var container = "dialogs";
            //services.AddScoped<IStore>(_ => new BlobStore(accountName, accountKey, container));

            services.AddSingleton<IStore>(new MemoryStore());
           
            services.AddSingleton<Dialog>(new RootDialog());

            services.AddBot<MyBot>(options =>
            {
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync($"Exception caught : {exception}");
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
