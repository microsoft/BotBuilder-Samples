// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
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

        protected virtual async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name == null)
            {
                return await OnTeamsCardActionInvokeAsync(turnContext, cancellationToken);
            }
            else
            {
                switch (turnContext.Activity.Name)
                {
                    case "signin/verifyState":
                        return await OnTeamsSigninVerifyStateAsync(turnContext, cancellationToken);

                    case "fileConsent/invoke":
                        return await OnTeamsFileConsentAsync(turnContext, SafeCast<FileConsentCardResponse>(turnContext.Activity.Value), cancellationToken);

                    case "actionableMessage/executeAction":
                        // TODO: need to understand what result this callback should take.
                        return await OnTeamsO365ConnectorCardActionAsync(turnContext, SafeCast<O365ConnectorCardActionQuery>(turnContext.Activity.Value), cancellationToken);
                        
                    case "composeExtension/query":
                        return CreateInvokeResponse(await OnTeamsMessagingExtensionQueryAsync(turnContext, SafeCast<MessagingExtensionQuery>(turnContext.Activity.Value), cancellationToken));

                    case "composeExtension/queryLink":
                        return CreateInvokeResponse(await OnTeamsAppBasedLinkQueryAsync(turnContext, cancellationToken));

                    case "composeExtension/selectItem":
                        return CreateInvokeResponse(await OnTeamsMessagingExtensionSelectItemAsync(turnContext, SafeCast<MessagingExtensionQuery>(turnContext.Activity.Value), cancellationToken));

                    case "composeExtension/submitAction":
                        return CreateInvokeResponse(await OnTeamsMessagingExtensionSubmitActionDispatchAsync(turnContext, SafeCast<MessagingExtensionAction>(turnContext.Activity.Value), cancellationToken));

                    case "composeExtension/fetchTask":
                        return CreateInvokeResponse(await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, cancellationToken));

                    case "composeExtension/onquerySettingsUrl":
                        return CreateInvokeResponse(await OnTeamsMessagingExtensionConfigurationSettingsUrlAsync(turnContext, cancellationToken));

                    case "composeExtension/setting":
                        return CreateInvokeResponse(await OnTeamsMessagingExtensionConfigurationSettings(turnContext, cancellationToken));

                    case "task/fetch":
                        return CreateInvokeResponse(await OnTeamsTaskModuleFetchAsync(turnContext, cancellationToken));

                    case "task/submit":
                        return CreateInvokeResponse(await OnTeamsTaskModuleSubmitAsync(turnContext, cancellationToken));

                    default:
                        return null;
                }
            }
        }

        protected virtual Task<InvokeResponse> OnTeamsCardActionInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTeamsSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual async Task<InvokeResponse> OnTeamsFileConsentAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            switch (fileConsentCardResponse.Action)
            {
                case "accept":
                    await OnTeamsFileConsentAcceptAsync(turnContext, fileConsentCardResponse, cancellationToken);
                    return CreateInvokeResponse();

                case "decline":
                    await OnTeamsFileConsentDeclineAsync(turnContext, fileConsentCardResponse, cancellationToken);
                    return CreateInvokeResponse();

                default:
                    return null;
            }
        }

        protected virtual Task<InvokeResponse> OnTeamsFileConsentAcceptAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTeamsFileConsentDeclineAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTeamsO365ConnectorCardActionAsync(ITurnContext<IInvokeActivity> turnContext, O365ConnectorCardActionQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<MessagingExtensionResponse> OnTeamsAppBasedLinkQueryAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionResponse>(null);
        }
        protected virtual Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionResponse>(null);
        }

        protected virtual Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionActionResponse>(null);
        }

        protected virtual async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionDispatchAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction query, CancellationToken cancellationToken)
        {
            var value = turnContext.Activity.Value as MessagingExtensionAction;
            if (value?.BotMessagePreviewAction != null)
            {
                switch (value.BotMessagePreviewAction)
                {
                    case "edit":
                        return await OnTeamsMessagingExtensionBotMessagePreviewEdit(turnContext, query, cancellationToken);

                    case "submit":
                        return await OnTeamsMessagingExtensionBotMessagePreviewSend(turnContext, query, cancellationToken);

                    default:
                        return null;
                }
            }
            else
            {
                return await OnTeamsMessagingExtensionSubmitActionAsync(turnContext, query, cancellationToken);
            }
        }

        protected virtual Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction query, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionActionResponse>(null);
        }

        protected virtual Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewEdit(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction query, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionActionResponse>(null);
        }

        protected virtual Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewSend(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction query, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionActionResponse>(null);
        }

        protected virtual Task<MessagingExtensionResponse> OnTeamsMessagingExtensionConfigurationSettingsUrlAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionResponse>(null);
        }
        protected virtual Task<MessagingExtensionResponse> OnTeamsMessagingExtensionConfigurationSettings(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<MessagingExtensionResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected virtual Task<InvokeResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            if (string.IsNullOrEmpty(channelData?.EventType))
            {
                return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
            }

            switch (channelData.EventType)
            {
                case "teamMemberAdded":
                    return OnTeamMembersAddedEventAsync(turnContext.Activity.MembersAdded, channelData.Team, turnContext, cancellationToken);

                case "teamMemberRemoved":
                    return OnTeamMembersRemovedEventAsync(turnContext.Activity.MembersRemoved, channelData.Team, turnContext, cancellationToken);

                case "channelCreated":
                    return OnChannelCreatedEventAsync(channelData.Channel, channelData.Team, turnContext, cancellationToken);

                case "channelDeleted":
                    return OnChannelDeletedEventAsync(channelData.Channel, channelData.Team, turnContext, cancellationToken);

                case "channelRenamed":
                    return OnChannelRenamedEventAsync(channelData.Channel, channelData.Team, turnContext, cancellationToken);

                case "teamRenamed":
                    return OnTeamRenamedEventAsync(channelData.Team, turnContext, cancellationToken);

                default:
                    return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
            }
        }

        protected virtual Task OnTeamMembersAddedEventAsync(IList<ChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return OnMembersAddedAsync(membersAdded, turnContext, cancellationToken);
        }

        protected virtual Task OnTeamMembersRemovedEventAsync(IList<ChannelAccount> membersRemoved, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return OnMembersRemovedAsync(membersRemoved, turnContext, cancellationToken);
        }

        protected virtual Task OnChannelCreatedEventAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelDeletedEventAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelRenamedEventAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnTeamRenamedEventAsync(TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private static InvokeResponse CreateInvokeResponse(object body = null)
        {
            return body == null ? null : new InvokeResponse { Status = (int)HttpStatusCode.OK, Body = body };
        }

        private static T SafeCast<T>(object value)
        {
            var obj = value as JObject;
            if (obj == null)
            {
                throw new Exception($"expected type '{value.GetType().Name}'");
            }

            return obj.ToObject<T>();
        }
    }
}
