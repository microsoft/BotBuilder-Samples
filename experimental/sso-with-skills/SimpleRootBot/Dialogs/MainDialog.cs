using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.SimpleRootBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;

        public MainDialog(ConversationState conversationState, SkillsConfiguration skillsConfig, SkillHttpClient skillClient, IConfiguration configuration, SkillConversationIdFactoryBase conversationIdFactory, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Logger = logger;

            AddDialog(new SignInDialog(configuration));
            AddDialog(new SignOutDialog(configuration));
            AddDialog(new DisplayTokenDialog(configuration));

            var botId = configuration.GetSection("MicrosoftAppId")?.Value;

            skillsConfig.Skills.TryGetValue("SkillBot", out var skill);
            AddDialog(new SkillDialog(
                new SkillDialogOptions()
                {
                    BotId = botId,
                    ConversationIdFactory = conversationIdFactory,
                    ConversationState = conversationState,
                    Skill = skill,
                    SkillClient = skillClient,
                    SkillHostEndpoint = skillsConfig.SkillHostEndpoint
                },
                nameof(SkillDialog)));
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                // Top level commands
                if (text == "signin" || text == "login" || text == "sign in" || text == "log in")
                {
                    return await innerDc.BeginDialogAsync(nameof(SignInDialog), null, cancellationToken);
                }
                else if (text == "signout" || text == "logout" || text == "sign out" || text == "log out")
                {
                    return await innerDc.BeginDialogAsync(nameof(SignOutDialog), null, cancellationToken);
                }
                else if (text == "token" || text == "get token" || text == "gettoken")
                {
                    return await innerDc.BeginDialogAsync(nameof(DisplayTokenDialog), null, cancellationToken);
                }
                else
                {
                    var skillResult = await innerDc.BeginDialogAsync(nameof(SkillDialog), new BeginSkillDialogOptions() { Activity = innerDc.Context.Activity }, cancellationToken);
                    return skillResult;
                }
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }
    }
}
