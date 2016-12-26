using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
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
            await this.botToUser.PostAsync(string.Format(Strings.GreetOnDemand, Emojis.WideSmile, Emojis.News));

            var reply = this.botToUser.MakeMessage();
            var cards = new List<CardAction>();

            foreach (var @enum in Enum.GetValues(typeof(Categories)).Cast<Categories>())
            {
                if (@enum == Categories.None)
                {
                    continue;
                }

                var cardAction = new CardAction(ActionTypes.ImBack, @enum.GetDislaplyName(), value: @enum.GetDislaplyName());
                cards.Add(cardAction);
            }

            reply.Attachments.Add(CardGenerator.GetHeroCard(cards));

            await this.botToUser.PostAsync(reply);

            await this.botToUser.PostAsync(string.Format(Strings.GreetOnDemandCont, Emojis.Wink));
        }
    }
}