// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.RootBot.Bots
{
    public class RootBot<T> : ActivityHandler
        where T : Dialog
    {
        public const string ActiveSkillPropertyName = "activeSkillProperty";
        private readonly IStatePropertyAccessor<BotFrameworkSkill> _activeSkillProperty;
        private readonly string _botId;
        private readonly ConversationState _conversationState;
        private readonly Dialog _mainDialog;
        private readonly SkillHttpClient _skillClient;
        private readonly SkillsConfiguration _skillsConfig;
        private readonly BotFrameworkSkill _targetSkill;

        public RootBot(ConversationState conversationState, Dialog dialog, SkillsConfiguration skillsConfig, SkillHttpClient skillClient, IConfiguration configuration)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _mainDialog = dialog ?? throw new ArgumentNullException(nameof(dialog));

            _skillsConfig = skillsConfig ?? throw new ArgumentNullException(nameof(skillsConfig));
            _skillClient = skillClient ?? throw new ArgumentNullException(nameof(skillsConfig));
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            if (string.IsNullOrWhiteSpace(_botId))
            {
                throw new ArgumentException($"{MicrosoftAppCredentials.MicrosoftAppIdKey} is not set in configuration");
            }

            // We use a single skill in this example.
            var targetSkillId = "SkillBot";
            if (!_skillsConfig.Skills.TryGetValue(targetSkillId, out _targetSkill))
            {
                throw new ArgumentException($"Skill with ID \"{targetSkillId}\" not found in configuration");
            }

            // Create state property to track the active skill
            _activeSkillProperty = conversationState.CreateProperty<BotFrameworkSkill>(ActiveSkillPropertyName);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await _conversationState.LoadAsync(turnContext, true, cancellationToken);
            await _mainDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _conversationState.LoadAsync(turnContext, true, cancellationToken);
            await _mainDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            // forget skill invocation
            await _activeSkillProperty.DeleteAsync(turnContext, cancellationToken);

            await _conversationState.LoadAsync(turnContext, true, cancellationToken);
            await _mainDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {

                    await turnContext.SendActivityAsync(MessageFactory.Text("Hello and welcome!"), cancellationToken);
                    await _mainDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }
    }
}
