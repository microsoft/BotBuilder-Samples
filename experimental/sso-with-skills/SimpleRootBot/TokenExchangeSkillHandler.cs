using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.SimpleRootBot
{
    public class TokenExchangeSkillHandler : SkillHandler
    {
        private readonly BotAdapter _adapter;
        private readonly SkillsConfiguration _skillsConfig;
        private readonly SkillHttpClient _skillClient;
        private readonly string _botId;
        private readonly string _connectionName;
        private readonly SkillConversationIdFactoryBase _conversationIdFactory;
        private IExtendedUserTokenProvider _tokenExchangeProvider;

        public TokenExchangeSkillHandler(
            BotAdapter adapter,
            IBot bot,
            IConfiguration configuration,
            SkillConversationIdFactoryBase conversationIdFactory,
            SkillsConfiguration skillsConfig,
            SkillHttpClient skillClient,
            ICredentialProvider credentialProvider,
            AuthenticationConfiguration authConfig,
            IChannelProvider channelProvider = null,
            ILogger logger = null)
            : base(adapter, bot, conversationIdFactory, credentialProvider, authConfig, channelProvider, logger)
        {
            _adapter = adapter;
            _tokenExchangeProvider = adapter as IExtendedUserTokenProvider;
            if (_tokenExchangeProvider == null)
            {
                throw new ArgumentException($"{nameof(adapter)} does not support token exchange");
            }

            _skillsConfig = skillsConfig;
            _skillClient = skillClient;
            _conversationIdFactory = conversationIdFactory;

            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            _connectionName = configuration.GetSection("ConnectionName")?.Value;
        }

        protected override async Task<ResourceResponse> OnSendToConversationAsync(ClaimsIdentity claimsIdentity, string conversationId, Activity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await InterceptOAuthCards(claimsIdentity, activity).ConfigureAwait(false))
            {
                return new ResourceResponse(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
            }

            return await base.OnSendToConversationAsync(claimsIdentity, conversationId, activity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task<ResourceResponse> OnReplyToActivityAsync(ClaimsIdentity claimsIdentity, string conversationId, string activityId, Activity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await InterceptOAuthCards(claimsIdentity, activity).ConfigureAwait(false))
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

        private async Task<bool> InterceptOAuthCards(ClaimsIdentity claimsIdentity, Activity activity)
        {
            if (activity.Attachments != null)
            {
                foreach (var attachment in activity.Attachments.Where(a => a?.ContentType == OAuthCard.ContentType))
                {
                    var targetSkill = GetCallingSkill(claimsIdentity);

                    if (targetSkill != null)
                    {
                        var oauthCard = ((JObject)attachment.Content).ToObject<OAuthCard>();

                        if (oauthCard.TokenExchangeResource != null /*&& _tokenExchangeConfig.ProviderId == oauthCard.TokenExchangeResource.ProviderId*/)
                        {
                            using (var context = new TurnContext(_adapter, activity))
                            {
                                context.TurnState.Add<IIdentity>("BotIdentity", claimsIdentity);

                                // AAD token exchange
                                var result = await _tokenExchangeProvider.ExchangeTokenAsync(
                                    context,
                                    _connectionName,
                                    activity.Recipient.Id,
                                    new TokenExchangeRequest() { Uri = oauthCard.TokenExchangeResource.Uri }).ConfigureAwait(false);

                                if (!string.IsNullOrEmpty(result.Token))
                                {
                                    // Send an Invoke back to the Skill
                                    return await SendTokenExchangeInvokeToSkill(activity, oauthCard.TokenExchangeResource.Id, result.Token, oauthCard.ConnectionName, targetSkill, default(CancellationToken)).ConfigureAwait(false);
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
            var activity = incomingActivity.CreateReply() as Activity;
            activity.Type = ActivityTypes.Invoke;
            activity.Name = SignInConstants.TokenExchangeOperationName;
            activity.Value = new TokenExchangeInvokeRequest()
            {
                Id = id,
                Token = token,
                ConnectionName = connectionName,
            };

            var skillConversationReference = await _conversationIdFactory.GetSkillConversationReferenceAsync(incomingActivity.Conversation.Id, cancellationToken).ConfigureAwait(false);
            activity.Conversation = skillConversationReference.ConversationReference.Conversation;
            activity.ServiceUrl = skillConversationReference.ConversationReference.ServiceUrl;

            // route the activity to the skill
            var response = await _skillClient.PostActivityAsync(_botId, targetSkill, _skillsConfig.SkillHostEndpoint, activity, cancellationToken);

            // Check response status: true if success, false if failure
            return (response.Status >= 200 && response.Status <= 299);
        }
    }
}
