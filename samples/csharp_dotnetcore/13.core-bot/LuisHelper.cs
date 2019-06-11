// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public static class LuisHelper
    {
        public static async Task<BookingDetails> ExecuteLuisQuery(IConfiguration configuration, ILogger logger, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var bookingDetails = new BookingDetails();

            try
            {
                // Create the LUIS settings from configuration.
                var luisApplication = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    "https://" + configuration["LuisAPIHostName"]
                );

                var recognizer = new LuisRecognizer(luisApplication);

                // The actual call to LUIS
                var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);

                var (intent, score) = recognizerResult.GetTopScoringIntent();
                if (intent == "Book_flight")
                {
                    var errors = new List<string>();

                    // We need to get the result from the LUIS JSON which at every level returns an array.
                    var (toValue, toOriginalText) = ExtractCompositeEntity(recognizerResult, "To", "Airport", turnContext.Activity.Text);
                    bookingDetails.Destination = toValue;
                    if (toValue == null && toOriginalText != null)
                    {
                        errors.Add(toOriginalText);
                    }

                    var (fromValue, fromOriginalText) = ExtractCompositeEntity(recognizerResult, "From", "Airport", turnContext.Activity.Text);
                    bookingDetails.Origin = fromValue;
                    if (fromValue == null && fromOriginalText != null)
                    {
                        errors.Add(fromOriginalText);
                    }

                    if (errors.Count > 0)
                    {
                        bookingDetails.ErrorMessage = $"The following airports are not supported: { string.Join(',', errors) }.";
                    }

                    // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
                    // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                    bookingDetails.TravelDate = recognizerResult.Entities["datetime"]?.FirstOrDefault()?["timex"]?.FirstOrDefault()?.ToString().Split('T')[0];
                }
            }
            catch (Exception e)
            {
                logger.LogWarning($"LUIS Exception: {e.Message} Check your LUIS configuration.");
            }

            return bookingDetails;
        }

        private static (string value, string originalText) ExtractCompositeEntity(RecognizerResult result, string name, string listName, string original)
        {
            var value = result.Entities[name]?.FirstOrDefault()?[listName]?.FirstOrDefault()?.FirstOrDefault()?.ToString();
            var originalText = ExtractOringinalText(result.Entities["$instance"]?[name], original);
            return (value, originalText);
        }

        private static string ExtractOringinalText(JToken instanceMetadata, string original)
        {
            if (instanceMetadata != null)
            {
                int? startIndex = instanceMetadata.FirstOrDefault()?["startIndex"]?.ToObject<int?>();
                int? endIndex = instanceMetadata.FirstOrDefault()?["endIndex"]?.ToObject<int?>();

                if (startIndex.HasValue && endIndex.HasValue)
                {
                    return original.Substring(startIndex.Value, endIndex.Value - startIndex.Value);
                }
            }

            return null;
        }
    }
}
