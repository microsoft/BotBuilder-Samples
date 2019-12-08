// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.DialogRootBot.Extensions;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Dialogs
{
    /// <summary>
    /// A sample dialog that can wrap remote calls to a skill.
    /// </summary>
    /// <remarks>
    /// The options parameter in <see cref="BeginDialogAsync"/> must be a <see cref="SkillDialogArgs"/> instance
    /// with the initial parameters for the dialog.
    /// </remarks>
    public class SkillDialog : Dialog
    {
        private readonly IStatePropertyAccessor<string> _activeSkillProperty;
        private readonly string _botId;
        private readonly ConversationState _conversationState;
        private readonly SkillHttpClient _skillClient;
        private readonly SkillsConfiguration _skillsConfig;

        public SkillDialog(ConversationState conversationState, SkillHttpClient skillClient, SkillsConfiguration skillsConfig, IConfiguration configuration)
            : base(nameof(SkillDialog))
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            if (string.IsNullOrWhiteSpace(_botId))
            {
                throw new ArgumentException($"{MicrosoftAppCredentials.MicrosoftAppIdKey} is not in configuration");
            }

            _skillClient = skillClient ?? throw new ArgumentNullException(nameof(skillClient));
            _skillsConfig = skillsConfig ?? throw new ArgumentNullException(nameof(skillsConfig));
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _activeSkillProperty = conversationState.CreateProperty<string>($"{typeof(SkillDialog).FullName}.ActiveSkillProperty");
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
        {
            if (options is CancellationToken)
            {
                throw new ArgumentException($"{nameof(options)} cannot be a cancellation token");
            }

            if (!(options is SkillDialogArgs dialogArgs))
            {
                throw new ArgumentNullException(nameof(options), $"Unable to cast {nameof(options)} to {nameof(SkillDialogArgs)}");
            }

            var skillId = dialogArgs.SkillId;
            if (!_skillsConfig.Skills.TryGetValue(skillId, out var skillInfo))
            {
                throw new KeyNotFoundException($"Unable to find \"{skillId}\" in the skill configuration.");
            }

            // Store Skill ID for this dialog instance
            await _activeSkillProperty.SetAsync(dc.Context, skillId, cancellationToken);

            var fwdActivity = dc.Context.Activity;
            if (!string.IsNullOrEmpty(dialogArgs.EventName))
            {
                await dc.Context.SendTraceActivityAsync($"{GetType().Name}.BeginDialogAsync(). Using an event: {dialogArgs.EventName}", cancellationToken: cancellationToken);
                var eventActivity = Activity.CreateEventActivity();
                eventActivity.Name = dialogArgs.EventName;
                eventActivity.Value = dialogArgs.Value;
                eventActivity.ApplyConversationReference(dc.Context.Activity.GetConversationReference(), true);
                fwdActivity = (Activity)eventActivity;
            }
            else
            {
                await dc.Context.SendTraceActivityAsync($"{GetType().Name}.BeginDialogAsync(). Using pass through (activity is: {dc.Context.Activity.Type}).", cancellationToken: cancellationToken);
                fwdActivity.Value = dialogArgs.Value;
            }

            // forward activity to the remote skill.
            return await SendToSkill(dc, fwdActivity, skillInfo, cancellationToken);
        }

        public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default)
        {
            await dc.Context.SendTraceActivityAsync($"{GetType().Name}.ContinueDialogAsync(). ActivityType: {dc.Context.Activity.Type}", cancellationToken: cancellationToken);

            var skillId = await _activeSkillProperty.GetAsync(dc.Context, () => null, cancellationToken);

            if (dc.Context.Activity.Type == ActivityTypes.Message && dc.Context.Activity.Text.Equals("abort", StringComparison.CurrentCultureIgnoreCase))
            {
                // Send a message to the skill to let it do some cleanup
                var eocActivity = Activity.CreateEndOfConversationActivity();
                eocActivity.ApplyConversationReference(dc.Context.Activity.GetConversationReference(), true);
                await SendToSkill(dc, (Activity)eocActivity, _skillsConfig.Skills[skillId], cancellationToken);

                // End this dialog and return (we don't care if the skill responds or not)
                await dc.Context.SendTraceActivityAsync($"{GetType().Name}.ContinueDialogAsync(). Cancelled", cancellationToken: cancellationToken);
                return await dc.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (dc.Context.Activity.Type == ActivityTypes.EndOfConversation)
            {
                await dc.Context.SendTraceActivityAsync($"{GetType().Name}.ContinueDialogAsync(). Got EndOfConversation", cancellationToken: cancellationToken);
                return await dc.EndDialogAsync(dc.Context.Activity.Value, cancellationToken);
            }

            // Just forward to the remote skill
            return await SendToSkill(dc, dc.Context.Activity, _skillsConfig.Skills[skillId], cancellationToken);
        }

        private async Task<DialogTurnResult> SendToSkill(DialogContext dc, Activity activity, BotFrameworkSkill skillInfo, CancellationToken cancellationToken)
        {
            // Always save state before forwarding
            // (the dialog stack won't get updated with the skillDialog and things won't work if you don't)
            await _conversationState.SaveChangesAsync(dc.Context, true, cancellationToken);
            var response = await _skillClient.PostActivityAsync(_botId, skillInfo, _skillsConfig.SkillHostEndpoint, activity, cancellationToken);
            if (response.Status != 200)
            {
                throw new HttpRequestException($"Error invoking the skill id: \"{skillInfo.Id}\" at \"{skillInfo.SkillEndpoint}\" (status is {response.Status}). \r\n {response.Body}");
            }

            return EndOfTurn;
        }
    }
}
