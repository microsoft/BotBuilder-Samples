// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using TaskModuleFactorySample.Extensions.Teams;
using TaskModuleFactorySample.Models;
using TaskModuleFactorySample.Services;
using TaskModuleFactorySample.TeamsChannels;
using TaskModuleFactorySample.TeamsChannels.Invoke;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using TaskModuleFactorySample.Dialogs.Helper;
using ITSMSkill.Models;
using TaskModuleFactorySample.Utils;

namespace TaskModuleFactorySample.Dialogs.Teams.TicketTaskModule
{
    /// <summary>
    /// CreateTicketTeamsImplementation Handles OnFetch and OnSumbit Activity for TaskModules
    /// </summary>
    [TeamsInvoke(FlowType = nameof(TeamsFlowType.CreateSample_Form))]
    public class CreateFormTeamsTaskModuleHandler : TeamsInvokeHandler<TaskModuleContinueResponse>
        //ITeamsTaskModuleHandler<TaskModuleContinueResponse>
    {
        private readonly IStatePropertyAccessor<SkillState> _stateAccessor;
        private readonly IStatePropertyAccessor<ActivityReferenceMap> _activityReferenceMapAccessor;
        private readonly ITeamsActivity<AdaptiveCard> _teamsTicketUpdateActivity;
        private readonly ConversationState _conversationState;
        private readonly BotSettings _settings;
        private readonly BotServices _services;

        public CreateFormTeamsTaskModuleHandler(
             IServiceProvider serviceProvider)
        {
            _conversationState = serviceProvider.GetService<ConversationState>();
            _settings = serviceProvider.GetService<BotSettings>();
            _services = serviceProvider.GetService<BotServices>();
            _activityReferenceMapAccessor = _conversationState.CreateProperty<ActivityReferenceMap>(nameof(ActivityReferenceMap));
            _teamsTicketUpdateActivity = serviceProvider.GetService<ITeamsActivity<AdaptiveCard>>();
            _stateAccessor = _conversationState.CreateProperty<SkillState>(nameof(SkillState));
        }

        // Handle Fetch
        public override async Task<TaskModuleContinueResponse> OnTeamsTaskModuleFetchAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            return new TaskModuleContinueResponse()
            {
                Value = new TaskModuleTaskInfo()
                {
                    Title = "Create Form",
                    Height = "medium",
                    Width = 500,
                    Card = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = TicketDialogHelper.CreateFormAdaptiveCard(_settings.MicrosoftAppId)
                    }
                }
            };
        }

        // Handle Submit True
        public override async Task<TaskModuleContinueResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            var activityReferenceMap = await _activityReferenceMapAccessor.GetAsync(
                context,
                () => new ActivityReferenceMap(),
                cancellationToken)
            .ConfigureAwait(false);

            var state = await _stateAccessor.GetAsync(context, () => new SkillState());

            var activityValueObject = JObject.FromObject(context.Activity.Value);

            var isDataObject = activityValueObject.TryGetValue("data", StringComparison.InvariantCultureIgnoreCase, out JToken dataValue);
            JObject dataObject = null;
            if (isDataObject)
            {
                dataObject = dataValue as JObject;

                // Get Title
                var title = dataObject.GetValue("FormTitle");

                // Get Description
                var description = dataObject.GetValue("FormDescription");

                // Get Urgency
                var urgency = dataObject.GetValue("FormUrgency");
                var sampleForm = new SampleForm
                {
                    Title = title.Value<string>(),
                    Description = description.Value<string>(),
                    Urgency = urgency.Value<string>()
                };
                // Get saved Activity Reference mapping to conversation Id
                activityReferenceMap.TryGetValue(context.Activity.Conversation.Id, out var activityReference);

                await _teamsTicketUpdateActivity.UpdateTaskModuleActivityAsync(
                    context,
                    activityReference,
                    TicketDialogHelper.UpdateFormCard(sampleForm, _settings.MicrosoftAppId),
                    cancellationToken);

                return new TaskModuleContinueResponse()
                {
                    Type = "continue",
                    Value = new TaskModuleTaskInfo()
                    {
                        Title = "Form Created",
                        Height = "medium",
                        Width = 500,
                        Card = new Attachment
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = TicketDialogHelper.FormResponseCard("Form has been created")
                        }
                    }
                };
            }

            // Failed to create Form
            return new TaskModuleContinueResponse()
            {
                Type = "continue",
                Value = new TaskModuleTaskInfo()
                {
                    Title = "Form Create Failed",
                    Height = "medium",
                    Width = 500,
                    Card = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = TicketDialogHelper.FormResponseCard("Form Create Failed")
                    }
                }
            };
        }
    }
}
