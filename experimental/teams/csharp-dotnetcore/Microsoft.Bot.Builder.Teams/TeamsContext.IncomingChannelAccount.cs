// <copyright file="TeamsContext.IncomingChannelAccount.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Internal
{
    using System;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Channel account operations.
    /// </summary>
    public partial class TeamsContext
    {
        /// <summary>
        /// Gets teams channel account data.
        /// </summary>
        /// <param name="channelAccount">Converts <see cref="ChannelAccount"/> into <see cref="TeamsChannelAccount"/>.</param>
        /// <returns>Teams channel account data.</returns>
        public TeamsChannelAccount AsTeamsChannelAccount(ChannelAccount channelAccount)
        {
            if (channelAccount == null)
            {
                throw new ArgumentNullException(nameof(channelAccount));
            }

            return channelAccount.AsJObject().ToObject<TeamsChannelAccount>();
        }
    }
}
