// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Testing;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Obsolete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Form.Extensions;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("Microsoft.Bot.Builder.Integration.ApplicationInsights.Core.Tests.Startup", "Tests")]

namespace Microsoft.Bot.Builder.Integration.ApplicationInsights.Core.Tests
{
    public class Startup : XunitTestFramework
    {
        public Startup(IMessageSink messageSink) 
            : base(messageSink)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            Configuration = builder.Build();

            /* TODO: This does not work until TestAdapter is updated
            Services = new ServiceCollection();
            Services.AddSingleton(Configuration);
            new DialogsBotComponent().ConfigureServices(Services, Configuration);
            new AdaptiveBotComponent().ConfigureServices(Services, Configuration);
            new LanguageGenerationBotComponent().ConfigureServices(Services, Configuration);
            new LuisBotComponent().ConfigureServices(Services, Configuration);
            new QnAMakerBotComponent().ConfigureServices(Services, Configuration);
            new FormExtensionsBotComponent().ConfigureServices(Services, Configuration);
            */
            ComponentRegistration.Add(new DialogsComponentRegistration());
            ComponentRegistration.Add(new DeclarativeComponentRegistration());
            ComponentRegistration.Add(new AdaptiveComponentRegistration());
            ComponentRegistration.Add(new LanguageGenerationComponentRegistration());
            ComponentRegistration.Add(new AdaptiveTestingComponentRegistration());
            ComponentRegistration.Add(new LuisComponentRegistration());
            ComponentRegistration.Add(new QnAMakerComponentRegistration());
            ComponentRegistration.Add(new DeclarativeComponentRegistrationBridge<FormExtensionsBotComponent>());
        }

        public IConfiguration Configuration { get; }
    }
}
