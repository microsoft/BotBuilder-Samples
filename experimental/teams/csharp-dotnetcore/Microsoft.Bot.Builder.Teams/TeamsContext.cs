// <copyright file="TeamsContext.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Internal
{
    using Microsoft.Bot.Connector.Teams;

    /// <summary>
    /// Teams extension class.
    /// </summary>
    public partial class TeamsContext : ITeamsContext
    {
        /// <summary>
        /// Turn context created by adapter and sent over through middlewares.
        /// </summary>
        private readonly ITurnContext turnContext;

        /// <summary>
        /// Teams connector client instance. This is used to make calls to BotFramework APIs which are only supported by MsTeams.
        /// </summary>
        private readonly ITeamsConnectorClient teamsConnectorClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsContext"/> class.
        /// </summary>
        /// <param name="turnContext">Turn context created by adapter and sent over through middlewares.</param>
        /// <param name="teamsConnectorClient">Teams connector client instance.</param>
        internal TeamsContext(ITurnContext turnContext, ITeamsConnectorClient teamsConnectorClient)
        {
            this.turnContext = turnContext;
            this.teamsConnectorClient = teamsConnectorClient;
        }

        /// <summary>
        /// Gets the teams operations. These are extended set of operations available only for 'MsTeams' channel.
        /// </summary>
        public ITeamsOperations Operations
        {
            get
            {
                return this.teamsConnectorClient.Teams;
            }
        }
    }
}
