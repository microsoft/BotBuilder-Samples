// <copyright file="EchoState.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.TeamEchoBot
{
    /// <summary>
    /// Class for storing conversation state.
    /// </summary>
    public class EchoState
    {
        /// <summary>
        /// Gets or sets the turn count.
        /// </summary>
        public int TurnCount { get; set; } = 0;
    }
}
