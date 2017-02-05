using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Zummer.Utilities.CardGenerators;

namespace Zummer.Handlers
{
    internal sealed class GreetingIntentHandler : IIntentHandler
    {
        private readonly IBotToUser botToUser;

        public GreetingIntentHandler(IBotToUser botToUser)
        {
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task Respond(IMessageActivity activity, LuisResult result)
        {
            var reply = this.botToUser.MakeMessage();

            reply.Attachments.Add(CardGenerator.GetHeroCard(text: string.Format(Strings.GreetOnDemand)));
            reply.Attachments.Add(CardGenerator.GetThumbNailCard(
                cardActions: new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, Strings.BingForMore, value: "https://www.bing.com")
                }));

            await this.botToUser.PostAsync(reply);
        }
    }
}