// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.SimpleRootBot.Bots
{
    public class RootBot : ActivityHandler
    {
        public static readonly string ActiveSkillPropertyName = $"{typeof(RootBot).FullName}.ActiveSkillProperty";
        private readonly IStatePropertyAccessor<BotFrameworkSkill> _activeSkillProperty;
        private readonly string _botId;
        private readonly ConversationState _conversationState;
        private readonly SkillsConfiguration _skillsConfig;
        private readonly BotFrameworkAuthentication _auth;
        private readonly SkillConversationIdFactoryBase _cidFactory;
        private readonly BotFrameworkSkill _targetSkill;

        public RootBot(ConversationState conversationState, SkillsConfiguration skillsConfig, IConfiguration configuration, BotFrameworkAuthentication auth, SkillConversationIdFactoryBase cidFactory)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _skillsConfig = skillsConfig ?? throw new ArgumentNullException(nameof(skillsConfig));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _cidFactory = cidFactory ?? throw new ArgumentNullException(nameof(cidFactory));

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;

            // We use a single skill in this example.
            var targetSkillId = "EchoSkillBot";
            _skillsConfig.Skills.TryGetValue(targetSkillId, out _targetSkill);

            // Create state property to track the active skill
            _activeSkillProperty = conversationState.CreateProperty<BotFrameworkSkill>(ActiveSkillPropertyName);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            // Forward all activities except EndOfConversation to the skill.
            if (turnContext.Activity.Type != ActivityTypes.EndOfConversation)
            {
                // Try to get the active skill
                var activeSkill = await _activeSkillProperty.GetAsync(turnContext, () => null, cancellationToken);
                if (activeSkill != null)
                {
                    // Send the activity to the skill
                    await SendToSkill(turnContext, activeSkill, cancellationToken);
                    return;
                }
            }

            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("skill"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Got it, connecting you to the skill..."), cancellationToken);

                // Save active skill in state
                await _activeSkillProperty.SetAsync(turnContext, _targetSkill, cancellationToken);

                // Send the activity to the skill
                await SendToSkill(turnContext, _targetSkill, cancellationToken);
                return;
            }

            // just respond
            await turnContext.SendActivityAsync(MessageFactory.Text("Me no nothin'. Say \"skill\" and I'll patch you through"), cancellationToken);

            // Save conversation state
            await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);
        }

        protected override async Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            // forget skill invocation
            await _activeSkillProperty.DeleteAsync(turnContext, cancellationToken);

            // Show status message, text and value returned by the skill
            var eocActivityMessage = $"Received {ActivityTypes.EndOfConversation}.\n\nCode: {turnContext.Activity.Code}";
            if (!string.IsNullOrWhiteSpace(turnContext.Activity.Text))
            {
                eocActivityMessage += $"\n\nText: {turnContext.Activity.Text}";
            }

            if ((turnContext.Activity as Activity)?.Value != null)
            {
                eocActivityMessage += $"\n\nValue: {JsonConvert.SerializeObject((turnContext.Activity as Activity)?.Value)}";
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(eocActivityMessage), cancellationToken);

            // We are back at the root
            await turnContext.SendActivityAsync(MessageFactory.Text("Back in the root bot. Say \"skill\" and I'll patch you through"), cancellationToken);

            // Save conversation state
            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Hello and welcome!"), cancellationToken);
                }
            }
        }

        private async Task SendToSkill(ITurnContext turnContext, BotFrameworkSkill targetSkill, CancellationToken cancellationToken)
        {
            // NOTE: Always SaveChanges() before calling a skill so that any activity generated by the skill
            // will have access to current accurate state.
            await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);

            // Create a new Conversation Id for skill request
            string skillConversationId = await _cidFactory.CreateSkillConversationIdAsync(
                new SkillConversationIdFactoryOptions
                {
                    FromBotOAuthScope = _auth.GetOriginatingAudience(),
                    FromBotId = _botId,
                    Activity = turnContext.Activity,
                    BotFrameworkSkill = targetSkill
                },
                cancellationToken);

            // route the activity to the skill
            using (BotFrameworkClient skillClient = _auth.CreateBotFrameworkClient())
            {
                var response = await skillClient.PostActivityAsync(_botId, targetSkill.AppId, targetSkill.SkillEndpoint, _skillsConfig.SkillHostEndpoint, skillConversationId, turnContext.Activity, cancellationToken);

                // Check response status
                if (!(response.Status >= 200 && response.Status <= 299))
                {
                    throw new HttpRequestException($"Error invoking the skill id: \"{targetSkill.Id}\" at \"{targetSkill.SkillEndpoint}\" (status is {response.Status}). \r\n {response.Body}");
                }
            }
        }
    }
}
