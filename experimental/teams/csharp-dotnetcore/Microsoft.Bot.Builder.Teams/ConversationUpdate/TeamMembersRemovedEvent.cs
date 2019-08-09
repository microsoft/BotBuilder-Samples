// <copyright file="TeamMembersRemovedEvent.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Event arguments for members removed event.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class TeamMembersRemovedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.MembersRemoved;
            }
        }

        /// <summary>
        /// Gets the list of removed members.
        /// </summary>
        public IList<ChannelAccount> MembersRemoved { get; set; }
    }
}
