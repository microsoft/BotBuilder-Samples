// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AspNetWebApi_QnA_Bot
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Represents a bot that can operate on incoming activities.
    /// </summary>
    public class QnABot : IBot
    {
        /// <summary>
        /// Key in the Bot config for our QnaMaker.
        /// </summary>
        public static readonly string QnaMakerKey = "QnaBot";

        private readonly BotServices _connectedServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnABot"/> class.
        /// </summary>
        /// <param name="services">The Bot Services used in the QnABot class.
        /// For example, the common instance of QnaMaker is passed in here.</param>
        public QnABot(BotServices services)
        {
            _connectedServices = services;
        }

        /// <summary>
        /// Handles the incoming Activity request from the user.
        /// </summary>
        /// <param name="context">The context object for this turn.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message && !context.Responded)
            {
                // Check QnAMaker model
                QnAMaker maker = _connectedServices.QnAServices[QnaMakerKey];
                var response = await maker.GetAnswersAsync(context);

                if (response != null && response.Length > 0)
                {
                    await context.SendActivityAsync(response[0].Answer);
                }
                else
                {
                    await context.SendActivityAsync("No QnA Maker answers were found. This example uses a QnA Maker Knowledge Base that focuses on smart light bulbs. To see QnA Maker in action, ask the bot questions like \"Why won't it turn on?\" or say something like \"I need help.\"");
                }
            }
        }
    }
}
