// <copyright file="ChannelDeletedEvent.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Channel deleted event arguments.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class ChannelDeletedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.ChannelDeleted;
            }
        }

        /// <summary>
        /// Gets the deleted channel.
        /// </summary>
        public ChannelInfo Channel { get; internal set; }
    }
}
