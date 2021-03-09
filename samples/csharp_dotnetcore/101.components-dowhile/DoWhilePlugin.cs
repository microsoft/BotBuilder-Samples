// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Runtime.Plugins;

namespace Microsoft.BotBuilderSamples.Components
{
    /// <summary>
    // Represents a plugin to be utilized within the bot runtime that can provide access
    // to additional components that implement well-defined interfaces in the Bot Framework
    // SDK.
    /// </summary>
    public class DoWhilePlugin : IBotPlugin
    {
        /// <summary>
        /// Load the contents of the plugin into the bot runtime.
        /// </summary>
        /// <param name="context">Load context that provides access to application configuration and service collection.</param>
        public void Load(IBotPluginLoadContext context)
        {
            ComponentRegistration.Add(new DoWhileComponentRegistration());
        }
    }
}
