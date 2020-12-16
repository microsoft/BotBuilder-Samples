// <copyright file="JustInTimeInstallationBot.cs" company="Microsoft">
// Copyright (c) Microsoft. All Rights Reserved.
// </copyright>

namespace JustInTimeInstallation.Bots
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a bot that processes incoming activities. 
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called. 
    /// This is a Transient lifetime service. Transient lifetime services are created each time they're requested. 
    /// For each Activity received, a new instance of this class is created. 
    /// Objects that are expensive to construct, or have a lifetime beyond the single turn, should be carefully managed.
    /// </summary>
    public class JustInTimeInstallationBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Echo: {turnContext.Activity.Text}";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            // we are handling two cases within try/catch block 
            //if the bot is installed it will create adaptive card attachment and show card with input fields
            try
            {
                await TeamsInfo.GetPagedMembersAsync(turnContext, 100, null, cancellationToken);
                return new MessagingExtensionActionResponse
                {
                    Task = new TaskModuleContinueResponse
                    {
                        Value = new TaskModuleTaskInfo
                        {
                            Card = CreateAdaptiveCardAttachment(),
                            Height = 200,
                            Width = 400,
                            Title = "Adaptive Card: Inputs",
                        },
                    },
                };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("403"))
                {
                    // else it will show installation card in Task module for the Bot so user can install the app
                    return new MessagingExtensionActionResponse
                    {
                        Task = new TaskModuleContinueResponse
                        {
                            Value = new TaskModuleTaskInfo
                            {
                                Card = GetJustInTimeInstallationCard(),
                                Height = 200,
                                Width = 400,
                                Title = "Adaptive Card: Inputs",
                            },
                        },
                    };

                }
                return null;

            }
        }

        protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            // This method is to handle the 'Close' button on the confirmation Task Module after the user signs out.
            return Task.FromResult(new MessagingExtensionActionResponse());
        }

        private static Attachment CreateAdaptiveCardAttachment()
        {
            // combine path for cross platform support
            string[] paths = { ".", "Resources", "adaptiveCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }

        private static Attachment GetJustInTimeInstallationCard()
        {

            // combine path for cross platform support
            //Reading the card json and sending it as an attachment to Task module response
            string[] paths = { ".", "Resources", "justintimeinstallation.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }
    }
}
