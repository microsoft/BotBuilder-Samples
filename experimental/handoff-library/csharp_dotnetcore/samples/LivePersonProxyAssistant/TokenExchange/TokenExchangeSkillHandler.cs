// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LivePersonProxyAssistant.TokenExchange
{
    public class TokenExchangeSkillHandler : SkillHandler
    {
        private readonly BotAdapter _adapter;
        private readonly IExtendedUserTokenProvider _tokenExchangeProvider;
        private readonly ITokenExchangeConfig _tokenExchangeConfig;
        private readonly SkillsConfiguration _skillsConfig;
        private readonly SkillHttpClient _skillClient;
        private readonly string _botId;
        private readonly SkillConversationIdFactoryBase _conversationIdFactory;

        public TokenExchangeSkillHandler(
            BotAdapter adapter,
            IBot bot,
            IConfiguration configuration,
            SkillConversationIdFactoryBase conversationIdFactory,
            SkillsConfiguration skillsConfig,
            SkillHttpClient skillClient,
            ICredentialProvider credentialProvider,
            AuthenticationConfiguration authConfig,
            ITokenExchangeConfig tokenExchangeConfig,
            IChannelProvider channelProvider = null,
            ILogger logger = null)
            : base(adapter, bot, conversationIdFactory, credentialProvider, authConfig, channelProvider, logger)
        {
            _adapter = adapter;
            _tokenExchangeProvider = adapter as IExtendedUserTokenProvider;
            _tokenExchangeConfig = tokenExchangeConfig;
            _skillsConfig = skillsConfig;
            _skillClient = skillClient;
            _conversationIdFactory = conversationIdFactory;

            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
        }

        protected override async Task<ResourceResponse> OnSendToConversationAsync(ClaimsIdentity claimsIdentity, string conversationId, Activity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_tokenExchangeConfig != null && await InterceptOAuthCardsAsync(claimsIdentity, activity).ConfigureAwait(false))
            {
                return new ResourceResponse(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
            }

            return await base.OnSendToConversationAsync(claimsIdentity, conversationId, activity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task<ResourceResponse> OnReplyToActivityAsync(ClaimsIdentity claimsIdentity, string conversationId, string activityId, Activity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_tokenExchangeConfig != null && await InterceptOAuthCardsAsync(claimsIdentity, activity).ConfigureAwait(false))
            {
                return new ResourceResponse(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
            }

            return await base.OnReplyToActivityAsync(claimsIdentity, conversationId, activityId, activity, cancellationToken).ConfigureAwait(false);
        }

        private BotFrameworkSkill GetCallingSkill(ClaimsIdentity claimsIdentity)
        {
            var appId = JwtTokenValidation.GetAppIdFromClaims(claimsIdentity.Claims);

            if (string.IsNullOrWhiteSpace(appId))
            {
                return null;
            }

            return _skillsConfig.Skills.Values.FirstOrDefault(s => string.Equals(s.AppId, appId, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<bool> InterceptOAuthCardsAsync(ClaimsIdentity claimsIdentity, Activity activity)
        {
            if (activity.Attachments != null)
            {
                BotFrameworkSkill targetSkill = null;
                foreach (var attachment in activity.Attachments.Where(a => a?.ContentType == OAuthCard.ContentType))
                {
                    if (targetSkill == null)
                    {
                        targetSkill = GetCallingSkill(claimsIdentity);
                    }

                    if (targetSkill != null)
                    {
                        var oauthCard = ((JObject)attachment.Content).ToObject<OAuthCard>();

                        if (oauthCard != null && oauthCard.TokenExchangeResource != null &&
                            _tokenExchangeConfig != null && !string.IsNullOrWhiteSpace(_tokenExchangeConfig.Provider) &&
                            _tokenExchangeConfig.Provider == oauthCard.TokenExchangeResource.ProviderId)
                        {
                            using (var context = new TurnContext(_adapter, activity))
                            {
                                context.TurnState.Add<IIdentity>(BotAdapter.BotIdentityKey, claimsIdentity);

                                // AAD token exchange
                                var result = await _tokenExchangeProvider.ExchangeTokenAsync(
                                    context,
                                    activity.Recipient?.Id,
                                    _tokenExchangeConfig.ConnectionName,
                                    new TokenExchangeRequest() { Uri = oauthCard.TokenExchangeResource.Uri }).ConfigureAwait(false);

                                if (!string.IsNullOrEmpty(result.Token))
                                {
                                    // Send an Invoke back to the Skill
                                    return await SendTokenExchangeInvokeToSkill(activity, oauthCard.TokenExchangeResource.Id, result.Token, oauthCard.ConnectionName, targetSkill, default).ConfigureAwait(false);
                                }

                                return false;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private async Task<bool> SendTokenExchangeInvokeToSkill(Activity incomingActivity, string id, string token, string connectionName, BotFrameworkSkill targetSkill, CancellationToken cancellationToken)
        {
            var activity = incomingActivity.CreateReply();
            activity.Type = ActivityTypes.Invoke;
            activity.Name = SignInConstants.TokenExchangeOperationName;
            activity.Value = new TokenExchangeInvokeRequest
            {
                Id = id,
                Token = token,
                ConnectionName = connectionName,
            };

            var conversationReference = await _conversationIdFactory.GetSkillConversationReferenceAsync(incomingActivity.Conversation.Id, cancellationToken).ConfigureAwait(false);
            activity.Conversation = conversationReference.ConversationReference.Conversation;

            // route the activity to the skill
            var response = await _skillClient.PostActivityAsync(_botId, targetSkill.AppId, targetSkill.SkillEndpoint, _skillsConfig.SkillHostEndpoint, activity.Conversation.Id, activity, cancellationToken);

            // Check response status: true if success, false if failure
            return response.Status >= 200 && response.Status <= 299;
        }
    }
}
