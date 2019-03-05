// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public static class LuisHelper
    {
        public static async Task<BookingDetails> ExecuteLuisQuery(IConfiguration configuration, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // Create the LUIS client from configuration.
            var luisService = new LuisService
            {
                AppId = configuration["BotServices:Luis-Booking-AppId"],
                AuthoringKey = configuration["BotServices:Luis-Booking-AuthoringKey"],
                Region = configuration["BotServices:Luis-Booking-Region"],
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
                // We need to get the result from the LUIS JSON which at every level returns an array.
                bookingDetails.Destination = recognizerResult.Entities["To"]?.FirstOrDefault()?["Airport"]?.FirstOrDefault()?.FirstOrDefault()?.ToString();
                bookingDetails.Origin = recognizerResult.Entities["From"]?.FirstOrDefault()?["Airport"]?.FirstOrDefault()?.FirstOrDefault()?.ToString();

                // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
                // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                bookingDetails.TravelDate = recognizerResult.Entities["datetime"]?.FirstOrDefault()?["timex"]?.FirstOrDefault()?.ToString().Split('T')[0];
            }

            return bookingDetails;
        }
    }
}
