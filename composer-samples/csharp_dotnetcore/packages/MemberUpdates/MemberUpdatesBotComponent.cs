// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsofts.Bot.Component.Samples.MemberUpdates
{
    /// <summary>
    /// Definition of a <see cref="Microsoft.Bot.Builder.BotComponent"/> that allows registration of
    /// services, custom actions, memory scopes and adapters.
    /// </summary>
    /// To make your components available to the system you derive from BotComponent and register services to add functionality.
    /// These components then are consumed in appropriate places by the systems that need them. When using Composer, Startup gets called
    /// automatically on the components by the bot runtime, as long as the components are registered in the configuration.
    public class MemberUpdatesBotComponent : BotComponent
    {
        /// <summary>
        /// Entry point for bot components to register types in resource explorer, consume configuration and register services in the
        /// services collection.
        /// </summary>
        /// <param name="services">Services collection to register dependency injection.</param>
        /// <param name="configuration">Configuration for the bot component.</param>
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Anything that could be done in Startup.ConfigureServices can be done here.
            // In this case, the OnMembersAdded and OnMembersRemoved needs to be added as a new DeclarativeTypes.
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<OnMembersAdded>(OnMembersAdded.Kind));
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<OnMembersRemoved>(OnMembersRemoved.Kind));
        }
    }
}
