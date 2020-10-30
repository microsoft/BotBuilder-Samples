// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Catering.Cards;
using Catering.Models;
using Microsoft.Bot.AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Catering
{
    // This bot will respond to the user's input with an Adaptive Card.
    // Adaptive Cards are a way for developers to exchange card content
    // in a common and consistent way. A simple open card format enables
    // an ecosystem of shared tooling, seamless integration between apps,
    // and native cross-platform performance on any device.
    // For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    // This is a Transient lifetime service. Transient lifetime services are created
    // each time they're requested. For each Activity received, a new instance of this
    // class is created. Objects that are expensive to construct, or have a lifetime
    // beyond the single turn, should be carefully managed.

    public class CateringBot<TDialog> : ActivityHandler where TDialog : Dialog
    {
        private const string WelcomeText = "Welcome to the Adaptive Cards 2.0 Bot. This bot will introduce you to Action.Execute in Adaptive Cards.";
        private BotState _userState;
        private CateringDb _cateringDb;
        private readonly CateringRecognizer _cateringRecognizer;
        private readonly Dialog _dialog;

        public CateringBot(UserState userState, CateringDb cateringDb, CateringRecognizer cateringRecognizer, TDialog dialog)
        {
            _userState = userState;
            _cateringDb = cateringDb;
            _cateringRecognizer = cateringRecognizer;
            _dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.ChannelId == Channels.Directline || turnContext.Activity.ChannelId == Channels.Webchat)
            {
                await SendWelcomeMessageAsync(turnContext, cancellationToken);
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _dialog.RunAsync(turnContext, _userState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }
        protected override async Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            await _dialog.RunAsync(turnContext, _userState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await _dialog.RunAsync(turnContext, _userState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            if (AdaptiveCardInvokeValidator.IsAdaptiveCardAction(turnContext))
            {
                var userSA = _userState.CreateProperty<User>(nameof(User));
                var user = await userSA.GetAsync(turnContext, () => new User() { Id = turnContext.Activity.From.Id });

                try
                {
                    AdaptiveCardInvoke request = AdaptiveCardInvokeValidator.ValidateRequest(turnContext);

                    if (request.Action.Verb == "order")
                    {
                        var cardOptions = AdaptiveCardInvokeValidator.ValidateAction<CardOptions>(request);

                        // process action
                        var responseBody = await ProcessOrderAction(user, cardOptions);
                        
                        return CreateInvokeResponse(HttpStatusCode.OK, responseBody);
                    }
                    else
                    {
                        AdaptiveCardActionException.VerbNotSupported(request.Action.Type);
                    }
                }
                catch (AdaptiveCardActionException e)
                {
                    return CreateInvokeResponse(HttpStatusCode.OK, e.Response);
                }
            }

            return null;
        }

        private async Task<AdaptiveCardInvokeResponse> ProcessOrderAction(User user, CardOptions cardOptions)
        {
            if ((Card)cardOptions.currentCard == Card.Entre)
            {
                if (!string.IsNullOrEmpty(cardOptions.custom))
                {
                    if (!await _cateringRecognizer.ValidateEntre(cardOptions.custom))
                    {
                        return CardResponse("RedoEntreOptions.json", new Lunch() { Entre = cardOptions.custom });
                    }
                    cardOptions.option = cardOptions.custom;
                }

                user.Lunch.Entre = cardOptions.option;
            }
            else if ((Card)cardOptions.currentCard == Card.Drink)
            {
                if (!string.IsNullOrEmpty(cardOptions.custom))
                {
                    if (!await _cateringRecognizer.ValidateDrink(cardOptions.custom))
                    {
                        return CardResponse("RedoDrinkOptions.json", new Lunch() { Drink = cardOptions.custom });
                    }

                    cardOptions.option = cardOptions.custom;
                }

                user.Lunch.Drink = cardOptions.option;
            }

            AdaptiveCardInvokeResponse responseBody = null;
            switch ((Card)cardOptions.nextCardToSend)
            {
                case Card.Drink:
                    responseBody = CardResponse("DrinkOptions.json");
                    break;
                case Card.Entre:
                    responseBody = CardResponse("EntreOptions.json");
                    break;
                case Card.Review:
                    responseBody = CardResponse("ReviewOrder.json", user.Lunch);
                    break;
                case Card.ReviewAll:
                    var latestOrders = await _cateringDb.GetRecentOrdersAsync();
                    responseBody = CardResponse("RecentOrders.json",
                        new
                        {
                            users = latestOrders.Items.Select(u => new
                            {
                                lunch = new
                                {
                                    entre = u.Lunch.Entre,
                                    drink = u.Lunch.Drink,
                                    orderTimestamp = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(u.Lunch.OrderTimestamp, "Pacific Standard Time").ToString("g")
                                }
                            }).ToList()
                        });
                    break;
                case Card.Confirmation:
                    await _cateringDb.UpsertOrderAsync(user);
                    responseBody = CardResponse("Confirmation.json");
                    break;
                default:
                    throw new NotImplementedException("No card matches that nextCardToSend.");
            }

            return responseBody;
        }

        private static InvokeResponse CreateInvokeResponse(HttpStatusCode statusCode, object body = null)
        {
            return new InvokeResponse()
            {
                Status = (int)statusCode,
                Body = body
            };
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var message = MessageFactory.Text(WelcomeText);
                    await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync($"Type anything to see a card here, or type recents to see recent orders.");
                }
            }
        }

        #region Cards As InvokeResponses

        private AdaptiveCardInvokeResponse CardResponse(string cardFileName)
        {
            return new AdaptiveCardInvokeResponse()
            {
                StatusCode = 200,
                Type = AdaptiveCard.ContentType,
                Value = new CardResource(cardFileName).AsJObject()
            };
        }

        private AdaptiveCardInvokeResponse CardResponse<T>(string cardFileName, T data)
        {
            return new AdaptiveCardInvokeResponse()
            {
                StatusCode = 200,
                Type = AdaptiveCard.ContentType,
                Value = new CardResource(cardFileName).AsJObject(data)
            };
        }

        #endregion
    }
}
