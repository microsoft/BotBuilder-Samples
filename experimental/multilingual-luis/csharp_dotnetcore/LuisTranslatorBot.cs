// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Samples.Ai.Luis.Translator
{
    public class LuisTranslatorBot : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    var luisResult = turnContext.TurnState.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);
                    var luisResponse = new LuisResponse();
                    luisResponse.Convert(luisResult);

                    if (luisResult != null)
                    {
                        await turnContext.SendActivityAsync($"Your input message after translation is " + luisResponse.Text);
                        var topItem = luisResponse.TopIntent();
                        await turnContext.SendActivityAsync($"After using LUIS recognition:\nthe top intent was: {topItem.intent}, with score {topItem.score}");
                    }

                    break;
                case ActivityTypes.ConversationUpdate:
                    foreach (var newMember in turnContext.Activity.MembersAdded)
                    {
                        if (newMember.Id != turnContext.Activity.Recipient.Id)
                        {
                            await turnContext.SendActivityAsync("Hello and welcome to the Luis Sample bot.");
                        }
                    }

                    break;
            }
        }
    }
}
