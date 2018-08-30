// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using AspNetCore_QnA_Bot.AppInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace AspNetCore_QnA_Bot
{
    public class QnABot : IBot
    {
        private readonly MyAppInsightsQnaMaker _qnaMaker;

        public QnABot(MyAppInsightsQnaMaker qnaMaker)
        {
            _qnaMaker = qnaMaker;
        }

        /// <summary>
        /// Every Conversation turn for our QnA Bot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, and asks the QnA Maker
        /// service if it recognizes an answer for the question given.
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message && !context.Responded)
            {
                // Check QnAMaker model
                var response = await _qnaMaker.GetAnswersAsync(context);

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
