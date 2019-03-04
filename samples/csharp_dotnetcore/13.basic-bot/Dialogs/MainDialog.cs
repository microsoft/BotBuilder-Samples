// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        private IConfiguration _configuration;

        public MainDialog(IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            _configuration = configuration;

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
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call LUIS and gather any potential booking details.
            var bookingDetails = await ExecuteLuisQuery(stepContext.Context, cancellationToken);

            // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
            return await stepContext.BeginDialogAsync(nameof(BookingDialog), bookingDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled or the user failed to confirm, the Result here will be null.
            if (stepContext.Result != null)
            {
                var result = (BookingDetails)stepContext.Result;

                // Now we have all the booking details call the booking service.

                // If the call to the booking service was successful tell the user.
                var msg = $"I have you booked to {result.Destination} from {result.Origin} on {result.TravelDate}";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            }
            return await stepContext.EndDialogAsync();
        }

        private async Task<BookingDetails> ExecuteLuisQuery(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // Create the LUIS client from configuration.
            var luisService = new LuisService
            {
                AppId = _configuration["BotServices:Luis-Booking-AppId"],
                AuthoringKey = _configuration["BotServices:Luis-Booking-AuthoringKey"],
                Region = _configuration["BotServices:Luis-Booking-Region"],
            };

            var recognizer = new LuisRecognizer(new LuisApplication(
                luisService.AppId,
                luisService.AuthoringKey,
                luisService.GetEndpoint()));

            // The actual call to LUIS
            var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);

            // Now process the result from LUIS.
            var bookingDetails = new BookingDetails();

            var (intent, score) = recognizerResult.GetTopScoringIntent();
            if (intent == "Book_flight")
            {
                bookingDetails.Destination = recognizerResult.Entities["Destination"]?.FirstOrDefault()?.ToString();
                bookingDetails.Origin = recognizerResult.Entities["Origin"]?.FirstOrDefault()?.ToString();
                bookingDetails.TravelDate = recognizerResult.Entities["TravelDate"]?.FirstOrDefault()?.ToString();
            }

            return bookingDetails;
        }
    }
}
