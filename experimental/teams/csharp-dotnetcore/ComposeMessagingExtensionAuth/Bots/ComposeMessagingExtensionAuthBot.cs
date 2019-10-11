// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * This Bot requires an Azure Bot Service OAuth connection name in appsettings.json
     * see: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication
     *
    * Clicking this bot's Task Menu will retrieve the login dialog, if the user is not already signed in.
    */
    public class ComposeMessagingExtensionAuthBot : TeamsActivityHandler
    {
        readonly string _connectionName;

        public ComposeMessagingExtensionAuthBot(IConfiguration configuration)
        {
            _connectionName = configuration["ConnectionName"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                // Hack around weird behavior of RemoveRecipientMention (it alters the activity.Text)
                string originalText = turnContext.Activity.Text;
                turnContext.Activity.RemoveRecipientMention();
                string text = turnContext.Activity.Text.Replace(" ", string.Empty).ToUpper();
                turnContext.Activity.Text = originalText;

                if (text.Equals("LOGOUT") || text.Equals("SIGNOUT"))
                {
                    await (turnContext.Adapter as IUserTokenProvider).SignOutUserAsync(turnContext, _connectionName, turnContext.Activity.From.Id, cancellationToken);

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Signed Out: {turnContext.Activity.From.Name}"), cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"echo: {turnContext.Activity.Text}"), cancellationToken);
                }
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            var tokenStatus = await (turnContext.Adapter as IUserTokenProvider).GetTokenStatusAsync(turnContext, turnContext.Activity.From.Id, _connectionName, cancellationToken: cancellationToken);
            var token = tokenStatus.FirstOrDefault(t => t.HasToken == true);
            if (token == null)
            {
                // There is no token, so the user has not signed in yet.

                // Retrieve the OAuth Sign in Link to use in the MessagingExtensionResult Suggested Actions
                var signInLink = await (turnContext.Adapter as IUserTokenProvider).GetOauthSignInLinkAsync(turnContext, _connectionName, cancellationToken);

                return new MessagingExtensionActionResponse
                {
                    ComposeExtension = new MessagingExtensionResult
                    {
                        Type = "auth",
                        SuggestedActions = new MessagingExtensionSuggestedAction
                        {
                            Actions = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Value = signInLink,
                                    Title = "Bot Service OAuth",
                                },
                            },
                        },
                    },
                };
            }

            // User is already signed in.
            return new MessagingExtensionActionResponse
            {
                Task = new TaskModuleContinueResponse(CreateSignedInTaskModuleTaskInfo()),
            };
        }

        protected override async Task<TaskModuleTaskInfo> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            // When a user has successfully signed in, the bot receives the Magic Code from Azure Bot Service in
            // the Activity.Value. Use the magic code to obtain the user token.
            var data = turnContext.Activity.Value as JObject;
            if (data != null && data["state"] != null)
            {
                var tokenResponse = await (turnContext.Adapter as IUserTokenProvider).GetUserTokenAsync(turnContext, _connectionName, data["state"].ToString(), cancellationToken: cancellationToken);
                return CreateSignedInTaskModuleTaskInfo(tokenResponse.Token);
            }
            else
            {
                var reply2 = MessageFactory.Text("OnTeamsTaskModuleFetchAsync called without 'state' in Activity.Value");
                await turnContext.SendActivityAsync(reply2, cancellationToken);
                return null;
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var asJObject = action.Data as JObject;
            if (asJObject != null && asJObject.ContainsKey("key") && asJObject["key"].ToString() == "signout")
            {
                // User clicked the Sign Out button from a Task Module
                await (turnContext.Adapter as IUserTokenProvider).SignOutUserAsync(turnContext, _connectionName, turnContext.Activity.From.Id, cancellationToken);
                await turnContext.SendActivityAsync(MessageFactory.Text($"Signed Out: {turnContext.Activity.From.Name}"), cancellationToken);
            }

            return null;
        }

        private TaskModuleTaskInfo CreateSignedInTaskModuleTaskInfo(string token = "")
        {
            var taskModuleTaskInfo = new TaskModuleTaskInfo
            {
                Card = new Attachment
                {
                    Content = new AdaptiveCard
                    {
                        Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock("You are signed in!"),
                                new AdaptiveTextBlock("Send 'Log out' or 'Sign out' to start over."),
                                new AdaptiveTextBlock("(Or click the Sign Out button below.)"),
                            },
                        Actions = new List<AdaptiveAction>()
                            {
                                new AdaptiveSubmitAction() { Title = "Close", Data = new JObject { { "key", "close" } } },
                                new AdaptiveSubmitAction() { Title = "Sign Out", Data = new JObject { { "key", "signout" } } },
                            },
                    },
                    ContentType = AdaptiveCard.ContentType,
                },
                Height = 160,
                Width = 350,
                Title = "Compose Extension Auth Example",
            };

            if (!string.IsNullOrEmpty(token))
            {
                var card = taskModuleTaskInfo.Card.Content as AdaptiveCard;

                // Embed a child Adaptive Card behind a AdaptiveShowCardAction to display the User's token
                card.Actions.Add(new AdaptiveShowCardAction()
                {
                    Title = "Show Token",
                    Card = new AdaptiveCard
                    {
                        Body = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock($"Your token is { token }") { Wrap = true },
                        },
                    },
                });

                taskModuleTaskInfo.Height = 300;
                taskModuleTaskInfo.Width = 500;
            }

            return taskModuleTaskInfo;
        }
    }
}
