// <copyright file="TeamSpecificConversationState.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.StateStorage
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Teams specific conversation state management.
    /// </summary>
    /// <seealso cref="BotState" />
    public class TeamSpecificConversationState : BotState
    {
        /// <summary>
        /// The key to use to read and write this conversation state object to storage.
        /// </summary>
        private static string propertyName = $"TeamSpecificConversationState:{typeof(TeamSpecificConversationState).Namespace}.{typeof(TeamSpecificConversationState).Name}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSpecificConversationState"/> class.
        /// Creates a new <see cref="TeamSpecificConversationState"/> object.
        /// </summary>
        /// <param name="storage">The storage provider to use.</param>
        public TeamSpecificConversationState(IStorage storage)
            : base(storage, propertyName)
        {
        }

        /// <summary>
        /// Gets the key to use when reading and writing state to and from storage.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <returns>The storage key.</returns>
        protected override string GetStorageKey(ITurnContext turnContext)
        {
            TeamsChannelData teamsChannelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            if (string.IsNullOrEmpty(teamsChannelData.Team?.Id))
            {
                return $"chat/{turnContext.Activity.ChannelId}/{turnContext.Activity.Conversation.Id}";
            }
            else
            {
                return $"team/{turnContext.Activity.ChannelId}/{teamsChannelData.Team.Id}";
            }
        }
    }
}
