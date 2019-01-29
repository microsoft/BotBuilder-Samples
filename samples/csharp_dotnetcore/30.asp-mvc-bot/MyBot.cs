// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;

namespace Asp_Mvc_Bot
{
    public class MyBot : BotBase
    {
        public MyBot(IAdapterIntegration adapter)
            : base(adapter)
        {
        }

        protected override async Task OnMessageAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(turnContext.Activity.Text));
        }

        protected override async Task OnConversationUpdateAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("conversation update"));
        }
    }
}
