// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class ActionBasedMessagingExtensionFetchTaskBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();
            if (turnContext.Activity.Text?.ToUpperInvariant() == "LOGOUT")
            {
                var tokenProvider = turnContext.Adapter as IUserTokenProvider;
                await tokenProvider.SignOutUserAsync(turnContext, "aadv2", turnContext.Activity.From.Id, cancellationToken);

                await turnContext.SendActivityAsync(MessageFactory.Text("You have been Signed Out."), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"echo: {turnContext.Activity.Text}"), cancellationToken);
            }
        }

        protected override async Task<TaskModuleTaskInfo> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsTaskModuleFetchAsync TaskModuleRequest: " + JsonConvert.SerializeObject(taskModuleRequest));
            await turnContext.SendActivityAsync(reply, cancellationToken);


            var tokenProvider = turnContext.Adapter as IUserTokenProvider;

            var data = turnContext.Activity.Value as JObject;

            if (data != null && data["state"] != null)
            {
                var tokenResponse = await tokenProvider.GetUserTokenAsync(turnContext, "aadv2", data["state"].ToString(), cancellationToken: cancellationToken);

                return new TaskModuleTaskInfo()
                {
                    Card = new Attachment
                    {
                        Content = new AdaptiveCard
                        {
                            Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock("Thank you for signing in!"),
                                new AdaptiveTextBlock($"Your token is { tokenResponse.Token }") { Wrap = true },
                            },
                        },
                        ContentType = AdaptiveCard.ContentType,
                    },
                    Height = 200,
                    Width = 400,
                    Title = "Task Module Example",
                };
            }
            else
            {

                return new TaskModuleTaskInfo()
                {
                    Card = new Attachment
                    {
                        Content = new AdaptiveCard
                        {
                            Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock("Something went wrong."),
                            },
                        },
                        ContentType = AdaptiveCard.ContentType,
                    },
                    Height = 200,
                    Width = 400,
                    Title = "Task Module Example",
                };
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionFetchTaskAsync MessagingExtensionQuery: " + JsonConvert.SerializeObject(query));
            await turnContext.SendActivityAsync(reply, cancellationToken);

            var tokenProvider = turnContext.Adapter as IUserTokenProvider;
            var tokenStatus = await tokenProvider.GetTokenStatusAsync(turnContext, turnContext.Activity.From.Id, cancellationToken: cancellationToken);

            if (!tokenStatus.Any(t => t.HasToken == true))
            {
                var signInLink = await (turnContext.Adapter as IUserTokenProvider).GetOauthSignInLinkAsync(turnContext, "aadv2", cancellationToken).ConfigureAwait(false);

                return new MessagingExtensionActionResponse
                {
                    ComposeExtension = new MessagingExtensionResult
                    {
                        Type = "auth",
                        SuggestedActions = new MessagingExtensionSuggestedAction()
                        {
                            Actions = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Value = signInLink,
                                },
                            },
                        },
                    },
                };
            }

            return AdaptiveCardHelper.CreateTaskModuleAdaptiveCardResponse();
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionSubmitActionAsync MessagingExtensionAction: " + JsonConvert.SerializeObject(action));
            await turnContext.SendActivityAsync(reply, cancellationToken);

            var submittedData = JsonConvert.DeserializeObject<SubmitExampleData>(action.Data.ToString());
            var adaptiveCard = submittedData.ToAdaptiveCard();
            return adaptiveCard.ToMessagingExtensionBotMessagePreviewResponse();
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewEditAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionBotMessagePreviewEditAsync MessagingExtensionAction: " + JsonConvert.SerializeObject(action));
            await turnContext.SendActivityAsync(reply, cancellationToken);

            var submitData = action.ToSubmitExampleData();
            return AdaptiveCardHelper.CreateTaskModuleAdaptiveCardResponse(
                                                        submitData.Question,
                                                        bool.Parse(submitData.MultiSelect),
                                                        submitData.Option1,
                                                        submitData.Option2,
                                                        submitData.Option3);
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewSendAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionBotMessagePreviewSendAsync MessagingExtensionAction: " + JsonConvert.SerializeObject(action));
            await turnContext.SendActivityAsync(reply, cancellationToken);

            var submitData = action.ToSubmitExampleData();
            var adaptiveCard = submitData.ToAdaptiveCard();
            return adaptiveCard.ToComposeExtensionResultResponse();
        }

        protected override async Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject obj, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
