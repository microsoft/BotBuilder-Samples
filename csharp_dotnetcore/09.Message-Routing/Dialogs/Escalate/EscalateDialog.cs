// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    /// <summary>
    /// Dialog for escalating the conversation to humans.
    /// </summary>
    public class EscalateDialog : RoutingSampleDialog
    {
        // Constants
        public const string Name = "EscalateDialog";

        // Fields
        private EscalateResponses _responder = new EscalateResponses();

        /// <summary>
        /// Initializes a new instance of the <see cref="EscalateDialog"/> class.
        /// </summary>
        /// <param name="botServices">The <see cref="BotServices"/> for the bot.</param>
        public EscalateDialog(BotServices botServices)
            : base(botServices, Name)
        {
            var escalate = new WaterfallStep[]
            {
                SendPhone,
            };

            AddDialog(new WaterfallDialog(Name, escalate));
        }

        private async Task<DialogTurnResult> SendPhone(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            await _responder.ReplyWith(dc.Context, EscalateResponses.SendPhone);
            return await dc.EndAsync();
        }
    }
}
