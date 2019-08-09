// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class TeamsActivityHandler : ActivityHandler
    {
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity == null)
            {
                throw new ArgumentException($"{nameof(turnContext)} must have non-null Activity.");
            }

            if (turnContext.Activity.Type == null)
            {
                throw new ArgumentException($"{nameof(turnContext)}.Activity must have non-null Type.");
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Invoke:
                    var invokeResponse = await OnInvokeActivityAsync(new DelegatingTurnContext<IInvokeActivity>(turnContext), cancellationToken);
                    if (invokeResponse != null)
                    {
                        await turnContext.SendActivityAsync(new Activity { Value = invokeResponse, Type = ActivityTypesEx.InvokeResponse });
                    }
                    break;

                default:
                    await base.OnTurnAsync(turnContext, cancellationToken);
                    break;
            }
        }

        protected virtual Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Name)
            {
                case "signin/verifyState":
                    return OnSigninVerifyStateAsync(turnContext, cancellationToken);

                case "fileConsent/invoke":
                    return OnFileConsent(turnContext, cancellationToken);

                case "composeExtension/query":
                    return OnMessagingExtensionQueryAsync(turnContext, JObject.FromObject(turnContext.Activity.Value).ToObject<MessagingExtensionQuery>(), cancellationToken);

                case "actionableMessage/executeAction":
                    return OnO365ConnectorCardActionAsync(turnContext, cancellationToken);

                case "composeExtension/queryLink":
                    return OnAppBasedLinkQueryAsync(turnContext, cancellationToken);

                case "composeExtension/fetchTask":
                    return OnMessagingExtensionFetchTaskAsync(turnContext, cancellationToken);

                case "composeExtension/submitAction":
                    return OnMessagingExtensionSubmitActionAsync(turnContext, cancellationToken);

                case "task/fetch":
                    return OnTaskModuleFetchAsync(turnContext, cancellationToken);

                case "task/submit":
                    return OnTaskModuleSubmitAsync(turnContext, cancellationToken);

                default:
                    return Task.FromResult<InvokeResponse>(null);
            }

        }

        protected virtual Task<InvokeResponse> OnSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnFileConsent(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnO365ConnectorCardActionAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnAppBasedLinkQueryAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.ChannelData != null)
            {
                var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

                if (!string.IsNullOrEmpty(channelData?.EventType))
                {
                    switch (channelData.EventType)
                    {
                        case "teamMemberAdded":
                            {
                                return OnTeamMembersAddedEventAsync(new TeamMembersAddedEvent
                                {
                                    MembersAdded = turnContext.Activity.MembersAdded,
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                });
                            }

                        case "teamMemberRemoved":
                            {
                                return OnTeamMembersRemovedEventAsync(new TeamMembersRemovedEvent
                                {
                                    MembersRemoved = turnContext.Activity.MembersRemoved,
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                });
                            }

                        case "channelCreated":
                            {
                                return OnChannelCreatedEventAsync(new ChannelCreatedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                    Channel = channelData.Channel,
                                });
                            }

                        case "channelDeleted":
                            {
                                return OnChannelDeletedEventAsync(new ChannelDeletedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                    Channel = channelData.Channel,
                                });
                            }

                        case "channelRenamed":
                            {
                                return OnChannelRenamedEventAsync(new ChannelRenamedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                    Channel = channelData.Channel,
                                });
                            }

                        case "teamRenamed":
                            {
                                return OnTeamRenamedEventAsync(new TeamRenamedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                });
                            }
                    }
                }
            }

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        protected virtual Task OnTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelCreatedEventAsync(ChannelCreatedEvent channelCreatedEvent)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent)
        {
            return Task.CompletedTask;
        }
    }
}
