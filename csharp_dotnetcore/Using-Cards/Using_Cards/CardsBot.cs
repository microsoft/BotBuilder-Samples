using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Using_Cards.Dialogs;

namespace Using_Cards
{
    public class CardsBot : IBot
    {
        private readonly CardsBotAccessors _accessors;
        public DialogSet Dialogs;

        public CardsBot(CardsBotAccessors accessors)
        {
            _accessors = accessors;

            Dialogs = new DialogSet(accessors.ConversationDialogState);
            Dialogs.Add(new MainDialog(MainDialog.DialogId));
            Dialogs.Add(new ChoicePrompt("cardPrompt"));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    var dc = await Dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dc.ContinueAsync(cancellationToken);

                    if (!dc.Context.Responded)
                    {
                        await dc.BeginAsync(MainDialog.DialogId, cancellationToken: cancellationToken);
                    }

                    break;
                case ActivityTypes.ConversationUpdate:

                    // Send a welcome message to the user and tell them what actions they need to perform to use this bot
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        {
                            foreach (var member in turnContext.Activity.MembersAdded)
                            {
                                var newUserName = member.Name;
                                if (member.Id != turnContext.Activity.Recipient.Id)
                                {
                                    await turnContext.SendActivityAsync($"Welcome to CardBot {newUserName}. This bot will show you different types of Rich Cards.  Please type anything to get started.", cancellationToken: cancellationToken);
                                }
                            }
                        }
                    }
                    break;
                default:
                    // There is no code in this bot to deal with ActivityTypes other than conversationUpdate or message
                    await turnContext.SendActivityAsync("This type of activity is not handled in this bot", cancellationToken: cancellationToken);
                    break;
            }
        }
    }
}
