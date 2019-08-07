// <copyright file="DropNonTeamsActivitiesMiddleware.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Middlewares
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Automatically drop all messages received from any channel except Microsoft Teams.
    /// </summary>
    public class DropNonTeamsActivitiesMiddleware : IMiddleware
    {
        /// <summary>
        /// When implemented in middleware, processess an incoming activity.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task that represents the work queued to execute.
        /// </returns>
        /// <remarks>
        /// Middleware calls the <paramref name="next" /> delegate to pass control to
        /// the next middleware in the pipeline. If middleware doesn’t call the next delegate,
        /// the adapter does not call any of the subsequent middleware’s request handlers or the
        /// bot’s receive handler, and the pipeline short circuits.
        /// <para>The <paramref name="turnContext" /> provides information about the
        /// incoming activity, and other data needed to process the activity.</para>
        /// </remarks>
        /// <seealso cref="ITurnContext" />
        /// <seealso cref="Schema.IActivity" />
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            BotAssert.ContextNotNull(turnContext);

            if (turnContext.Activity.ChannelId.Equals(Channels.Msteams, StringComparison.OrdinalIgnoreCase))
            {
                await next(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
