// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace QnABot
{
    /// <summary>
    /// For each interaction from the user, an instance of this class is created and
    /// the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single Turn, should be carefully managed.
    /// </summary>
    public class QnABot : IBot
    {
        /// <summary>
        /// Key in the bot config (.bot file) for the QnA Maker instance.
        /// In the ".bot" file, multiple instances of QnA Maker can be configured.
        /// </summary>
        public static readonly string QnAMakerKey = "QnABot";

        /// <summary>
        /// Services configured from the ".bot" file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnABot"/> class.
        /// </summary>
        /// <param name="services">A <see cref="BotServices"/> configured from the ".bot" file.</param>
        public QnABot(BotServices services)
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            if (!_services.QnAServices.ContainsKey(QnAMakerKey))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a QnA service named '{QnAMakerKey}'.");
            }
        }

        /// <summary>
        /// Every conversation turn for our QnA bot will call this method.
        /// There are no dialogs used, the sample only uses "single turn" processing,
        /// meaning a single request and response, with no stateful conversation.
        /// </summary>
        /// <param name="context">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Check QnA Maker model
                var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(context);
                if (response != null && response.Length > 0)
                {
                    await context.SendActivityAsync(response[0].Answer);
                }
                else
                {
                    var msg = @"No QnA Maker answers were found. This example uses a QnA Maker Knowledge Base that focuses on smart light bulbs. 
                        To see QnA Maker in action, ask the bot questions like 'Why won't it turn on?' or 'I need help'.";

                    await context.SendActivityAsync(msg);
                }
            }
        }
    }
}
