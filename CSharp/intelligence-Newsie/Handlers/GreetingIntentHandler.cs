using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newsie.Extensions;
using Newsie.Utilities;
using Newsie.Utilities.CardGenerators;

namespace Newsie.Handlers
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
            var cards = new List<CardAction>();

            foreach (var @enum in Enum.GetValues(typeof(NewsCategory)).Cast<NewsCategory>())
            {
                if (@enum == NewsCategory.None)
                {
                    continue;
                }

                var cardAction = new CardAction(ActionTypes.ImBack, @enum.GetDislaplyName(), value: @enum.GetDislaplyName());
                cards.Add(cardAction);
            }

            reply.Attachments.Add(CardGenerator.GetHeroCard(text: string.Format(Strings.GreetOnDemand)));
            reply.Attachments.Add(CardGenerator.GetHeroCard(cardActions: cards));
            reply.Attachments.Add(CardGenerator.GetThumbNailCard(
                    cardActions: new List<CardAction>
                    {
                        new CardAction(ActionTypes.OpenUrl, Strings.BingForMore, value: "https://www.bing.com/news/search?q=bing+news")
                    }));

            await this.botToUser.PostAsync(reply);
        }
    }
}