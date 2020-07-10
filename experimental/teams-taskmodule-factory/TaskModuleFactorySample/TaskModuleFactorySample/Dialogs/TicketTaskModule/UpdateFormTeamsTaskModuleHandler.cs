// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.Dialogs.Teams.TicketTaskModule
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using TaskModuleFactorySample.Extensions;
    using TaskModuleFactorySample.Extensions.Teams;
    using TaskModuleFactorySample.Extensions.Teams.TaskModule;
    using TaskModuleFactorySample.Models;
    using TaskModuleFactorySample.Services;
    using TaskModuleFactorySample.TeamsChannels;
    using TaskModuleFactorySample.TeamsChannels.Invoke;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using TaskModuleFactorySample.Dialogs.Helper;

    /// <summary>
    /// UpdateTicket Handler for Updating Ticket OnFetch and OnSumbit Activity for TaskModules
    /// </summary>
    [TeamsInvoke(FlowType = nameof(TeamsFlowType.UpdateSample_Form))]
    public class UpdateFormTeamsTaskModuleHandler : ITeamsTaskModuleHandler<TaskModuleContinueResponse>
    {
        private readonly IStatePropertyAccessor<SkillState> _stateAccessor;
        private readonly ConversationState _conversationState;
        private readonly BotSettings _settings;
        private readonly BotServices _services;

        public UpdateFormTeamsTaskModuleHandler(IServiceProvider serviceProvider)
        {
            _conversationState = serviceProvider.GetService<ConversationState>();
            _settings = serviceProvider.GetService<BotSettings>();
            _services = serviceProvider.GetService<BotServices>();
            _stateAccessor = _conversationState.CreateProperty<SkillState>(nameof(SkillState));
        }

        public async Task<TaskModuleContinueResponse> OnTeamsTaskModuleFetchAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            var taskModuleMetadata = context.Activity.GetTaskModuleMetadata<TaskModuleMetadata>();

            var ticketDetails = taskModuleMetadata.FlowData != null ?
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(taskModuleMetadata.FlowData))
                    .GetValueOrDefault("FormDetails") : null;

            // Convert JObject to Ticket
            SampleState formDetails = JsonConvert.DeserializeObject<SampleState>(ticketDetails.ToString());

            return new TaskModuleContinueResponse()
            {
                Value = new TaskModuleTaskInfo()
                {
                    Title = "Update Form",
                    Height = "medium",
                    Width = 500,
                    Card = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = TicketDialogHelper.UpdateFormCard(formDetails, _settings.MicrosoftAppId)
                    }
                }
            };
        }

        public async Task<TaskModuleContinueResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            var state = await _stateAccessor.GetAsync(context, () => new SkillState());

            var taskModuleMetadata = context.Activity.GetTaskModuleMetadata<TaskModuleMetadata>();

            var ticketDetails = taskModuleMetadata.FlowData != null ?
                JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(taskModuleMetadata.FlowData))
                .GetValueOrDefault("FormDetails") : null;

            // Convert JObject to Ticket
            SampleState formDetails = JsonConvert.DeserializeObject<SampleState>(ticketDetails.ToString());

            // If ticket is valid go ahead and update
            if (formDetails != null)
            {
                // Get User Input from AdatptiveCard
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

                    // Return Added Form Envelope
                    return new TaskModuleContinueResponse()
                    {
                        Type = "continue",
                        Value = new TaskModuleTaskInfo()
                        {
                            Title = "Form Updated",
                            Height = "small",
                            Width = 300,
                            Card = new Attachment
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = TicketDialogHelper.FormResponseCard("Form has been Updated")
                            }
                        }
                    };
                }
            }

            // Failed to update Form
            return new TaskModuleContinueResponse()
            {
                Type = "continue",
                Value = new TaskModuleTaskInfo()
                {
                    Title = "Form Updated",
                    Height = "small",
                    Width = 300,
                    Card = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = TicketDialogHelper.FormResponseCard("Form Update Failed")
                    }
                }
            };
        }
    }
}
