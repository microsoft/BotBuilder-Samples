// <copyright file="TeamEventBase.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Type of team event.
    /// </summary>
    public enum TeamEventType
    {
        /// <summary>
        /// Members added.
        /// </summary>
        MembersAdded,

        /// <summary>
        /// Members removed.
        /// </summary>
        MembersRemoved,

        /// <summary>
        /// New channel created in a team.
        /// </summary>
        ChannelCreated,

        /// <summary>
        /// Channel deleted from a team.
        /// </summary>
        ChannelDeleted,

        /// <summary>
        /// Channel was renamed.
        /// </summary>
        ChannelRenamed,

        /// <summary>
        /// Team was renamed.
        /// </summary>
        TeamRenamed,
    }

    /// <summary>
    /// Base class for events generated for teams.
    /// </summary>
    public abstract class TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public abstract TeamEventType EventType { get; }

        /// <summary>
        /// Gets the team for the event.
        /// </summary>
        public TeamInfo Team { get; set; }

        /// <summary>
        /// Gets the tenant for the team.
        /// </summary>
        public TenantInfo Tenant { get; set; }

        /// <summary>
        /// Gets the original activity.
        /// </summary>
        public ITurnContext TurnContext { get; set; }
    }
}
