// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// NamePrompt show how to extend the TextPrompt class to perform input validation.
    /// </summary>
    public class NamePrompt : TextPrompt
    {
        private const int NameLengthMinValue = 3;

        public NamePrompt(string dialogId)
            : base(dialogId, async (context, promptContext, cancellationToken) =>
            {
                // Validate that the user entered a minimum length for their name
                var value = promptContext.Recognized.Value?.Trim() ?? string.Empty;
                if (value.Length > NameLengthMinValue)
                {
                    promptContext.End(value);
                }
                else
                {
                    await context.SendActivityAsync($"Names needs to be at least `{NameLengthMinValue}` characters long.").ConfigureAwait(false);
                }
            })
        {
        }
    }
}