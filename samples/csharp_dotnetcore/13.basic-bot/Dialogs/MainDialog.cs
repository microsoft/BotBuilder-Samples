// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        UserState _userState;

        public MainDialog(UserState userState, IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            _userState = userState;

            // TODO: create the LUIS client using the configuration we've been given

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new BookingDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        protected IStatePropertyAccessor<BookingDetails> BookingDetailsAccessor { get; set; }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("what I help you with today?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = (string)stepContext.Result;

            // TODO: run LUIS on the text

            // imagine we gathered the origin city from the LUIS entities
            var bookingDetails = new BookingDetails
            {
                Destination = null,
                Origin = "Berlin",
                TravelDate = null,
            };

            return await stepContext.BeginDialogAsync(nameof(BookingDialog), bookingDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // a more realistic scenario would be to call the remote booking system but here we'll just store the result in UserState
            var accessor = _userState.CreateProperty<BookingDetails>(nameof(BookingDetails));
            await accessor.SetAsync(stepContext.Context, (BookingDetails)stepContext.Result, cancellationToken);

            return await stepContext.EndDialogAsync();
        }
    }
}
