// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Dialogs
{
    /// <summary>
    /// The main dialog for this bot. It uses a <see cref="SkillDialog"/> to call skills.
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        public static readonly string ActiveSkillPropertyName = $"{typeof(MainDialog).FullName}.ActiveSkillProperty";
        private readonly IStatePropertyAccessor<BotFrameworkSkill> _activeSkillProperty;
        private readonly string _selectedSkillKey = $"{typeof(MainDialog).FullName}.SelectedSkillKey";
        private readonly SkillsConfiguration _skillsConfig;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(ConversationState conversationState, SkillConversationIdFactoryBase conversationIdFactory, SkillHttpClient skillClient, SkillsConfiguration skillsConfig, IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            var botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            if (string.IsNullOrWhiteSpace(botId))
            {
                throw new ArgumentException($"{MicrosoftAppCredentials.MicrosoftAppIdKey} is not in configuration");
            }

            _skillsConfig = skillsConfig ?? throw new ArgumentNullException(nameof(skillsConfig));

            if (skillClient == null)
            {
                throw new ArgumentNullException(nameof(skillClient));
            }

            if (conversationState == null)
            {
                throw new ArgumentNullException(nameof(conversationState));
            }

            // ChoicePrompt to render available skills and skill actions
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            // Create SkillDialog instances for the configured skills
            foreach (var skillInfo in _skillsConfig.Skills.Values)
            {
                // SkillDialog used to wrap interaction with the selected skill
                var skillDialogOptions = new SkillDialogOptions
                {
                    BotId = botId,
                    ConversationIdFactory = conversationIdFactory,
                    SkillClient = skillClient,
                    SkillHostEndpoint = skillsConfig.SkillHostEndpoint,
                    ConversationState = conversationState,
                    Skill = skillInfo
                };
                AddDialog(new SkillDialog(skillDialogOptions, skillInfo.Id));
            }

            // Main waterfall dialog for this bot
            var waterfallSteps = new WaterfallStep[]
            {
                SelectSkillStepAsync,
                SelectSkillActionStepAsync,
                CallSkillActionStepAsync,
                FinalStepAsync
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // Create state property to track the active skill
            _activeSkillProperty = conversationState.CreateProperty<BotFrameworkSkill>(ActiveSkillPropertyName);

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            // This is an example on how to cancel a SkillDialog that is currently in progress from the parent bot
            var activeSkill = await _activeSkillProperty.GetAsync(innerDc.Context, () => null, cancellationToken);
            var activity = innerDc.Context.Activity;
            if (activeSkill != null && activity.Type == ActivityTypes.Message && activity.Text.Equals("abort", StringComparison.CurrentCultureIgnoreCase))
            {
                // Cancel all dialog when the user says abort.
                await innerDc.CancelAllDialogsAsync(cancellationToken);
                return await innerDc.ReplaceDialogAsync(InitialDialogId, "Canceled! \n\n What skill would you like to call?", cancellationToken);
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        // Render a prompt to select the skill to call.
        private async Task<DialogTurnResult> SelectSkillStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Create the PromptOptions from the skill configuration which contain the list of configured skills.
            var messageText = stepContext.Options?.ToString() ?? "What skill would you like to call?";
            var repromptMessageText = "That was not a valid choice, please select a valid skill.";
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
                Choices = _skillsConfig.Skills.Select(skill => new Choice(skill.Value.Id)).ToList()
            };

            // Prompt the user to select a skill.
            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        // Render a prompt to select the action for the skill.
        private async Task<DialogTurnResult> SelectSkillActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the skill info based on the selected skill.
            var selectedSkillId = ((FoundChoice)stepContext.Result).Value;
            var selectedSkill = _skillsConfig.Skills.FirstOrDefault(s => s.Value.Id == selectedSkillId).Value;

            // Remember the skill selected by the user.
            stepContext.Values[_selectedSkillKey] = selectedSkill;

            // Create the PromptOptions with the actions supported by the selected skill.
            var messageText = $"What action would you like to call in **{selectedSkill.Id}**?";
            var repromptMessageText = "That was not a valid choice, please select a valid action.";
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
                Choices = GetSkillActions(selectedSkill)
            };

            // Prompt the user to select a skill action.
            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        // Starts SkillDialog based on the user's selections
        private async Task<DialogTurnResult> CallSkillActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var selectedSkill = (BotFrameworkSkill)stepContext.Values[_selectedSkillKey];

            Activity skillActivity;
            switch (selectedSkill.Id)
            {
                case "EchoSkillBot":
                    // Echo skill only handles message activities, send a dummy utterance to get it started.
                    skillActivity = (Activity)Activity.CreateMessageActivity();
                    skillActivity.Text = "Start echo skill";
                    break;
                case "DialogSkillBot":
                    skillActivity = CreateDialogSkillBotActivity(((FoundChoice)stepContext.Result).Value);
                    break;
                default:
                    throw new Exception($"Unknown target skill id: {selectedSkill.Id}.");
            }

            // Create the BeginSkillDialogOptions
            var skillDialogArgs = new BeginSkillDialogOptions { Activity = skillActivity };

            // We are manually creating the activity to send to the skill, ensure we add the ChannelData and Properties 
            // from the original activity so the skill gets them.
            // Note: this is not necessary if we are just forwarding the current activity from context. 
            skillDialogArgs.Activity.ChannelData = stepContext.Context.Activity.ChannelData;
            skillDialogArgs.Activity.Properties = stepContext.Context.Activity.Properties;

            // Comment or uncomment this line if you need to enable or disable buffering
            // replies from the skill in the response.
            //skillDialogArgs.Activity.DeliveryMode = DeliveryModes.ExpectReplies;

            // Save active skill in state
            await _activeSkillProperty.SetAsync(stepContext.Context, selectedSkill, cancellationToken);

            // Start the skillDialog instance with the arguments. 
            return await stepContext.BeginDialogAsync(selectedSkill.Id, skillDialogArgs, cancellationToken);
        }

        // The SkillDialog has ended, render the results (if any) and restart MainDialog.
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activeSkill = await _activeSkillProperty.GetAsync(stepContext.Context, () => null, cancellationToken);

            if (stepContext.Result != null)
            {
                var message = $"Skill \"{activeSkill.Id}\" invocation complete.";
                message += $" Result: {JsonConvert.SerializeObject(stepContext.Result)}";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(message, message, inputHint: InputHints.IgnoringInput), cancellationToken: cancellationToken);
            }

            // Clear the skill selected by the user.
            stepContext.Values[_selectedSkillKey] = null;

            // Clear active skill in state
            await _activeSkillProperty.DeleteAsync(stepContext.Context, cancellationToken);

            // Restart the main dialog with a different message the second time around
            return await stepContext.ReplaceDialogAsync(InitialDialogId, $"Done with \"{activeSkill.Id}\". \n\n What skill would you like to call?", cancellationToken);
        }

        // Helper method to create Choice elements for the actions supported by the skill
        private IList<Choice> GetSkillActions(BotFrameworkSkill skill)
        {
            // Note: the bot would probably render this by reading the skill manifest
            // we are just using hardcoded skill actions here for simplicity.

            var choices = new List<Choice>();
            switch (skill.Id)
            {
                case "EchoSkillBot":
                    choices.Add(new Choice("Messages"));
                    break;

                case "DialogSkillBot":
                    choices.Add(new Choice("m:some message for tomorrow"));
                    choices.Add(new Choice("BookFlight"));
                    choices.Add(new Choice("OAuthTest"));
                    choices.Add(new Choice("mv:some message with value"));
                    choices.Add(new Choice("BookFlightWithValues"));
                    break;
            }

            return choices;
        }

        // Helper method to create the activity to be sent to the DialogSkillBot using selected type and values
        private Activity CreateDialogSkillBotActivity(string selectedOption)
        {
            // Note: in a real bot, the dialogArgs will be created dynamically based on the conversation
            // and what each action requires, here we hardcode the values to make things simpler.

            // Send a message activity to the skill.
            if (selectedOption.StartsWith("m:", StringComparison.CurrentCultureIgnoreCase))
            {
                var activity = (Activity)Activity.CreateMessageActivity();
                activity.Text = selectedOption.Substring(2).Trim();
                return activity;
            }

            // Send a message activity to the skill with some artificial parameters in value
            if (selectedOption.StartsWith("mv:", StringComparison.CurrentCultureIgnoreCase))
            {
                var activity = (Activity)Activity.CreateMessageActivity();
                activity.Text = selectedOption.Substring(3).Trim();
                activity.Value = new BookingDetails { Destination = "New York" };
                return activity;
            }

            // Send an event activity to the skill with "OAuthTest" in the name.
            if (selectedOption.Equals("OAuthTest", StringComparison.CurrentCultureIgnoreCase))
            {
                var activity = (Activity)Activity.CreateEventActivity();
                activity.Name = "OAuthTest";
                return activity;
            }

            // Send an event activity to the skill with "BookFlight" in the name.
            if (selectedOption.Equals("BookFlight", StringComparison.CurrentCultureIgnoreCase))
            {
                var activity = (Activity)Activity.CreateEventActivity();
                activity.Name = "BookFlight";
                return activity;
            }

            // Send an event activity to the skill "BookFlight" in the name and some testing values.
            if (selectedOption.Equals("BookFlightWithValues", StringComparison.CurrentCultureIgnoreCase))
            {
                var activity = (Activity)Activity.CreateEventActivity();
                activity.Name = "BookFlight";
                activity.Value = new BookingDetails
                {
                    Destination = "New York",
                    Origin = "Seattle"
                };
                return activity;
            }

            throw new Exception($"Unable to create dialogArgs for \"{selectedOption}\".");
        }
    }
}
