// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace EnterpriseBot
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class EnterpriseBot : IBot
    {
        private readonly BotServices _services;
        private readonly EnterpriseBotAccessors _accessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnterpriseBot"/> class.
        /// </summary>
        /// <param name="botServices">Bot services.</param>
        /// <param name="accessors">Bot State Accessors.</param>
        public EnterpriseBot(BotServices botServices, EnterpriseBotAccessors accessors)
        {
            _accessors = accessors;
            _services = botServices;

            Dialogs = new DialogSet(accessors.ConversationDialogState);
            Dialogs.Add(new MainDialog(_services));
        }

        private DialogSet Dialogs { get; set; }

        /// <summary>
        /// Run every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dc = await Dialogs.CreateContextAsync(turnContext);
            var result = await dc.ContinueAsync();

            if (result.Status == DialogTurnStatus.Empty)
            {
                if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
                {
                    var activity = turnContext.Activity.AsConversationUpdateActivity();

                    // if conversation update is not from the bot.
                    if (!activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
                    {
                        await dc.BeginAsync(nameof(MainDialog));
                    }
                }
            }
        }
    }
}
