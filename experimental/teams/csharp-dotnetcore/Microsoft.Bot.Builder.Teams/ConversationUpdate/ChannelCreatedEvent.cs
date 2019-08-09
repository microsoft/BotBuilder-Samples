// <copyright file="ChannelCreatedEvent.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Channel created event arguments.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class ChannelCreatedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.ChannelCreated;
            }
        }

        /// <summary>
        /// Gets the created channel.
        /// </summary>
        public ChannelInfo Channel { get; internal set; }
    }
}
