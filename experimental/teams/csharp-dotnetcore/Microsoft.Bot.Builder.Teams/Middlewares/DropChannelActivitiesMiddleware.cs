// <copyright file="DropChannelActivitiesMiddleware.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Middlewares
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Automatically drops all messages from channels.
    /// </summary>
    /// <seealso cref="IMiddleware" />
    public class DropChannelActivitiesMiddleware : IMiddleware
    {
        /// <summary>
        /// Called in the activity processing pipeline to process incoming activity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="next">The next delegate to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task tracking operation.</returns>
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            TeamsChannelData teamsChannelData = context.Activity.GetChannelData<TeamsChannelData>();

            if (teamsChannelData.Team == null)
            {
                await next(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
