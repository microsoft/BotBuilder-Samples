// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CustomComponent v$templateversion$

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Runtime.Plugins;

namespace $safeprojectname$
{
    /// <summary>
    // Represents a plugin to be utilized within the bot runtime that can provide access
    // to additional components that implement well-defined interfaces in the Bot Framework
    // SDK.
    /// </summary>
    public class CustomComponentPlugin : IBotPlugin
    {
        /// <summary>
        /// Load the contents of the plugin into the bot runtime.
        /// </summary>
        /// <param name="context">Load context that provides access to application configuration and service collection.</param>
        public void Load(IBotPluginLoadContext context)
        {
            ComponentRegistration.Add(new CustomComponentRegistration());
        }
    }
}
