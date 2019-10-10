// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;

namespace Cards.Bots
{
    /*
     * You can install this bot in any scope. From the UI just @mention the bot.
     */
    public class CardsBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var card = Cards.CreateSampleO365ConnectorCard();
            var cardAttachment = new Attachment
            {
                Content = card,
                ContentType = O365ConnectorCard.ContentType,
            };

            await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment));
        }

        protected override async Task OnTeamsO365ConnectorCardActionAsync(ITurnContext<IInvokeActivity> turnContext, O365ConnectorCardActionQuery query, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"O365ConnectorCardActionQuery event value: {JsonConvert.SerializeObject(query)}"));
        }
    }
}
