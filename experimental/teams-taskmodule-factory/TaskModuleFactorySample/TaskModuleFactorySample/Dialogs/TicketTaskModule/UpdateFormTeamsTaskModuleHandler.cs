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
    using AdaptiveCards.Rendering;

    /// <summary>
    /// UpdateTicket Handler for Updating Ticket OnFetch and OnSumbit Activity for TaskModules
    /// </summary>
    [TeamsInvoke(FlowType = nameof(TeamsFlowType.UpdateSample_Form))]
    public class UpdateFormTeamsTaskModuleHandler : TeamsInvokeHandler<TaskModuleContinueResponse>
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

        public override async Task<TaskModuleContinueResponse> OnTeamsTaskModuleFetchAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            var taskModuleMetadata = context.Activity.GetTaskModuleMetadata<TaskModuleMetadata>();

            var ticketDetails = taskModuleMetadata.FlowData != null ?
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(taskModuleMetadata.FlowData)) : null;

            // Convert JObject to Ticket
            ticketDetails.TryGetValue("Description", out var description);
            ticketDetails.TryGetValue("Title", out var title);
            ticketDetails.TryGetValue("Urgency", out var urgency);

            SampleForm formDetails = new SampleForm { Description = (string)description, Title = (string)title, Urgency = (string)urgency };

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
                        Content = TicketDialogHelper.UpdateFormCardSubmit(formDetails, _settings.MicrosoftAppId)
                    }
                }
            };
        }

        public override async Task<TaskModuleContinueResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("TaskSubmit For UpdateForm not implemented");
        }
    }
}
