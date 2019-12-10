// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Dialogs
{
    /// <summary>
    /// A sample MainDialog that uses SkillDialog to call skills.
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        // Note: in this example we using only one skill.
        private const string _targetSkillId = "DialogSkillBot";
        private readonly string _botId;
        private readonly ConversationState _conversationState;
        private readonly SkillHttpClient _skillClient;
        private readonly SkillsConfiguration _skillsConfig;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(ConversationState conversationState, SkillHttpClient skillClient, SkillsConfiguration skillsConfig, SkillDialog skillDialog, IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            _skillClient = skillClient;
            _skillsConfig = skillsConfig;
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(skillDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?";
            var promptMessage = CreateTaskPromptMessageWithActions(messageText);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        // Starts SkillDialog based on the user's selection
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Send a message activity to the skill.
            if (stepContext.Context.Activity.Text.StartsWith("m:", StringComparison.CurrentCultureIgnoreCase))
            {
                var dialogArgs = new SkillDialogArgs
                {
                    SkillId = _targetSkillId,
                    ActivityType = ActivityTypes.Message,
                    Text = stepContext.Context.Activity.Text.Substring(2).Trim()
                };
                return await stepContext.BeginDialogAsync(nameof(SkillDialog), dialogArgs, cancellationToken);
            }

            // Send a message activity to the skill with some artificial parameters in value
            if (stepContext.Context.Activity.Text.StartsWith("mv:", StringComparison.CurrentCultureIgnoreCase))
            {
                var dialogArgs = new SkillDialogArgs
                {
                    SkillId = _targetSkillId,
                    ActivityType = ActivityTypes.Message,
                    Text = stepContext.Context.Activity.Text.Substring(3).Trim(),
                    Value = new BookingDetails { Destination = "New York" }
                };
                return await stepContext.BeginDialogAsync(nameof(SkillDialog), dialogArgs, cancellationToken);
            }

            // Send an event activity to the skill with "OAuthTest" in the name.
            if (stepContext.Context.Activity.Text.Equals("OAuthTest", StringComparison.CurrentCultureIgnoreCase))
            {
                var dialogArgs = new SkillDialogArgs
                {
                    SkillId = _targetSkillId,
                    ActivityType = ActivityTypes.Event,
                    Name = "OAuthTest"
                };
                return await stepContext.BeginDialogAsync(nameof(SkillDialog), dialogArgs, cancellationToken);
            }

            // Send an event activity to the skill with "BookFlight" in the name.
            if (stepContext.Context.Activity.Text.Equals("BookFlight", StringComparison.CurrentCultureIgnoreCase))
            {
                var dialogArgs = new SkillDialogArgs
                {
                    SkillId = _targetSkillId,
                    ActivityType = ActivityTypes.Event,
                    Name = "BookFlight"
                };
                return await stepContext.BeginDialogAsync(nameof(SkillDialog), dialogArgs, cancellationToken);
            }

            // Send an event activity to the skill "BookFlight" in the name and some testing values.
            if (stepContext.Context.Activity.Text.Equals("BookFlightWithValues", StringComparison.CurrentCultureIgnoreCase))
            {
                var dialogArgs = new SkillDialogArgs
                {
                    SkillId = _targetSkillId,
                    ActivityType = ActivityTypes.Event,
                    Name = "BookFlight",
                    Value = new BookingDetails
                    {
                        Destination = "New York",
                        Origin = "Seattle"
                    }
                };
                return await stepContext.BeginDialogAsync(nameof(SkillDialog), dialogArgs, cancellationToken);
            }

            // Send an invoke activity to the skill with "GetWeather" in the name and some testing values.
            // Note that this operation doesn't use SkillDialog, InvokeActivities are single turn Request/Response.
            if (stepContext.Context.Activity.Text.Equals("GetWeather", StringComparison.CurrentCultureIgnoreCase))
            {
                var invokeActivity = Activity.CreateInvokeActivity();
                invokeActivity.Name = "GetWeather";
                invokeActivity.Value = new Location
                {
                    PostalCode = "11218"
                };
                invokeActivity.ApplyConversationReference(stepContext.Context.Activity.GetConversationReference(), true);

                await _conversationState.SaveChangesAsync(stepContext.Context, true, cancellationToken);
                var skillInfo = _skillsConfig.Skills[_targetSkillId];
                var response = await _skillClient.PostActivityAsync(_botId, skillInfo, _skillsConfig.SkillHostEndpoint, (Activity)invokeActivity, cancellationToken);
                if (response.Status != 200)
                {
                    throw new HttpRequestException($"Error invoking the skill id: \"{skillInfo.Id}\" at \"{skillInfo.SkillEndpoint}\" (status is {response.Status}). \r\n {response.Body}");
                }

                var invokeResult = $"Invoke result: {JsonConvert.SerializeObject(response.Body)}";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(invokeResult, inputHint: InputHints.IgnoringInput), cancellationToken: cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Catch all for unhandled intents
            var didntUnderstandMessageText = "Sorry, I didn't get that. Please try asking in a different way.";
            var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result != null)
            {
                var message = "Skill invocation complete.";
                message += $" Result: {JsonConvert.SerializeObject(stepContext.Result)}";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(message, inputHint: InputHints.IgnoringInput), cancellationToken: cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            return await stepContext.ReplaceDialogAsync(InitialDialogId, "What else can I do for you?", cancellationToken);
        }

        private Activity CreateTaskPromptMessageWithActions(string messageText)
        {
            var activity = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            activity.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>
                {
                    new CardAction
                    {
                        Title = "Hi",
                        Type = ActionTypes.ImBack,
                        Value = "Hi"
                    },
                    new CardAction
                    {
                        Title = "m:some message",
                        Type = ActionTypes.ImBack,
                        Value = "m:some message for tomorrow"
                    },
                    new CardAction
                    {
                        Title = "Book a flight",
                        Type = ActionTypes.ImBack,
                        Value = "BookFlight"
                    },
                    new CardAction
                    {
                        Title = "Get Weather",
                        Type = ActionTypes.ImBack,
                        Value = "GetWeather"
                    },
                    new CardAction
                    {
                        Title = "OAuthTest",
                        Type = ActionTypes.ImBack,
                        Value = "OAuthTest"
                    },
                    new CardAction
                    {
                        Title = "mv:some message with value",
                        Type = ActionTypes.ImBack,
                        Value = "mv:some message with value"
                    },
                    new CardAction
                    {
                        Title = "Book a flight with values",
                        Type = ActionTypes.ImBack,
                        Value = "BookFlightWithValues"
                    }
                }
            };
            return activity;
        }
    }
}
