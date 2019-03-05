// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    public class BookingDialog : CancelAndHelpDialog
    {
        public BookingDialog()
            : base(nameof(BookingDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new DateTimePrompt(nameof(DateTimePrompt), DateTimePromptValidator));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DestinationStepAsync,
                OriginStepAsync,
                TravelDateStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            if (bookingDetails.Destination == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Where would you like to travel to?") }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(bookingDetails.Destination);
            }
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.Destination = (string)stepContext.Result;

            if (bookingDetails.Origin == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Where are you traveling from?") }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(bookingDetails.Origin);
            }
        }
        private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.Origin = (string)stepContext.Result;

            var promptMsg = "When would you like to travel?";
            var repromptMsg = "I'm sorry, to make your booking I need an exact travel date including Day Month and Year.";

            if (bookingDetails.TravelDate == null)
            {
                // We were not given any date at all so prompt the user.
                return await stepContext.PromptAsync(nameof(DateTimePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text(promptMsg),
                        RetryPrompt = MessageFactory.Text(repromptMsg)
                    }, cancellationToken);
            }
            else
            {
                // We have a Date we just need to check it is unambiguous.
                var timexProperty = new TimexProperty(bookingDetails.TravelDate);
                if (!timexProperty.Types.Contains(Constants.TimexTypes.Definite))
                {
                    // This is essentially a "reprompt" of the data we were given up front.
                    return await stepContext.PromptAsync(nameof(DateTimePrompt),
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text(repromptMsg)
                        }, cancellationToken);
                }
                else
                {
                    return await stepContext.NextAsync(new DateTimeResolution { Timex = bookingDetails.TravelDate });
                }
            }
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            // At this point we are going to assume this is a definite Date because of the custom validator we added to the DateTimePrompt.
            bookingDetails.TravelDate = ((List<DateTimeResolution>)stepContext.Result)[0].Timex;

            var msg = $"Please confirm, I have you traveling to: {bookingDetails.Destination} from: {bookingDetails.Origin} on: {bookingDetails.TravelDate}";

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result == true)
            {
                var bookingDetails = (BookingDetails)stepContext.Options;
                return await stepContext.EndDialogAsync(bookingDetails);
            }
            else
            {
                return await stepContext.EndDialogAsync(null);
            }
        }

        private static Task<bool> DateTimePromptValidator(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
        {
            // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
            // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
            var timex = promptContext.Recognized.Value[0].Timex.Split('T')[0];

            // If this is a definite Date including year, month and day we are good otherwise reprompt.
            // A better solution might be to let the user know what part is actually missing.
            var isDefinite = new TimexProperty(timex).Types.Contains(Constants.TimexTypes.Definite);

            return Task.FromResult(isDefinite);
        }
    }
}
