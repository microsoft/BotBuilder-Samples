// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace AspNetCore_QnA_Bot
{
    /// <summary>
    /// Represents a bot that can process incoming activities.
    /// For each interaction from the user, an instance of this class is called.
    /// </summary>
    public class QnABot : IBot
    {
        /// <summary>
        /// Key in the Bot config (.bot file) for the QnaMaker instance.
        /// In the .bot file, multiple instances of QnaMaker can be configured.
        /// </summary>
        public static readonly string QnaMakerKey = "QnaBot";

        /// <summary>
        /// Services configured from the ".bot" file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnABot"/> class.
        /// </summary>
        /// <param name="services">Services configured from the ".bot" file.</param>
        public QnABot(BotServices services)
        {
            _services = services;
        }

        /// <summary>
        /// Every Conversation turn for our QnA Bot will call this method. In this example,
        /// the bot checks the Activty type to verify it's a message, and asks the QnA Maker
        /// service if it recognizes an answer for the question given.
        /// There are no dialogs used, since it's "single turn" processing, meaning there
        /// are no set of decision points or set of properties that need to be gathered from
        /// the user to successfully process the activity.
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message && !context.Responded)
            {
                // Check QnAMaker model
                var response = await _services.QnAServices[QnaMakerKey].GetAnswersAsync(context);

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
