// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Configuration;
using System.Linq;
using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.Bot.Builder.BotFramework
{
    /// <summary>
    /// Channel provider which uses <see cref="ConfigurationManager.AppSettings"/> to lookup the channel service property.
    /// </summary>
    /// <remarks>
    /// This will populate the <see cref="SimpleChannelProvider.ChannelService"/> from a configuration entry with the key of <see cref="ChannelServiceKey"/>.
    ///
    /// NOTE: if the keys are not present, a <c>null</c> value will be used.
    /// </remarks>
    public sealed class ConfigurationChannelProvider : SimpleChannelProvider
    {
        /// <summary>
        /// The key for ChannelService.
        /// </summary>
        public const string ChannelServiceKey = "ChannelService";

        public ConfigurationChannelProvider()
        {
            this.ChannelService = ConfigurationManager.AppSettings[ChannelServiceKey];
        }
    }
}
