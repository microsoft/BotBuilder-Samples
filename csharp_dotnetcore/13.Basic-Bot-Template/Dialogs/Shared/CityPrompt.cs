// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    ///  CityPrompt show how to extend the TextPrompt class to perform input validation.
    /// </summary>
    public class CityPrompt : TextPrompt
    {
        // Minimum lengh requirements for city and name
        private const int CityLengthMinValue = 5;

        public CityPrompt(string dialogId)
            : base(dialogId, async (context, promptContext, cancellationToken) =>
            {
                // Validate that the user entered a minimum lenght for their name
                var value = promptContext.Recognized.Value?.Trim() ?? string.Empty;
                if (value.Length > CityLengthMinValue)
                {
                    promptContext.End(value);
                }
                else
                {
                    await context.SendActivityAsync($"City names needs to be at least `{CityLengthMinValue}` characters long.").ConfigureAwait(false);
                }
            })
        {
        }
    }
}
