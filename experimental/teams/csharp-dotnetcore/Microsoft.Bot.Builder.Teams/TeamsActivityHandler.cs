// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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

        protected virtual Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Name)
            {
                case "signin/verifyState":
                    return OnSigninVerifyStateAsync(turnContext, cancellationToken);

                case "fileConsent/invoke":
                    return OnFileConsent(turnContext, JObject.FromObject(turnContext.Activity.Value).ToObject<FileConsentCardResponse>(), cancellationToken);

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

        protected virtual Task<InvokeResponse> OnFileConsent(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
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
            var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            if (!string.IsNullOrEmpty(channelData?.EventType))
            {
                return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
            }

            switch (channelData.EventType)
            {
                case "teamMemberAdded":
                    return OnTeamMembersAddedEventAsync(turnContext.Activity.MembersAdded, channelData, turnContext, cancellationToken);

                case "teamMemberRemoved":
                    return OnTeamMembersRemovedEventAsync(turnContext.Activity.MembersRemoved, channelData, turnContext, cancellationToken);

                case "channelCreated":
                    return OnChannelCreatedEventAsync(channelData, turnContext, cancellationToken);

                case "channelDeleted":
                    return OnChannelDeletedEventAsync(channelData, turnContext, cancellationToken);

                case "channelRenamed":
                    return OnChannelRenamedEventAsync(channelData, turnContext, cancellationToken);

                case "teamRenamed":
                    return OnTeamRenamedEventAsync(channelData, turnContext, cancellationToken);

                default:
                    return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
            }
        }

        protected virtual Task OnTeamMembersAddedEventAsync(IList<ChannelAccount> membersAdded, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnTeamMembersRemovedEventAsync(IList<ChannelAccount> membersRemoved, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelCreatedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelDeletedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnChannelRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnTeamRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
