// <copyright file="TeamRenamedEvent.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    /// <summary>
    /// Team renamed event.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class TeamRenamedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.TeamRenamed;
            }
        }
    }
}
