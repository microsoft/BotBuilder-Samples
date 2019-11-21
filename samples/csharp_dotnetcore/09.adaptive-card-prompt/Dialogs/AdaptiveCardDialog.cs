// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.Bot.Builder.BotBuilderSamples;

namespace Microsoft.BotBuilderSamples
{
    public class AdaptiveCardDialog : ComponentDialog
    {

        public AdaptiveCardDialog()
            : base(nameof(AdaptiveCardDialog))
        {
            var cardPath = Path.Combine("./Resources/", "adaptiveCard.json");
            var cardJson = File.ReadAllText(cardPath);
            var cardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };

            var promptSettings = new AdaptiveCardPromptSettings()
            {
                Card = cardAttachment
            };

            var adaptiveCardPrompt = new AdaptiveCardPrompt(nameof(AdaptiveCardPrompt), promptSettings);

            AddDialog(adaptiveCardPrompt);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowCardAsync,
                DisplayResultsAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ShowCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call the prompt - Note: PromptOptions is a required argument for StepContext and DialogContext, but doesn't need to be filled out
            return await stepContext.PromptAsync(nameof(AdaptiveCardPrompt), new PromptOptions());
        }

        private async Task<DialogTurnResult> DisplayResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Use the result
            var result = (stepContext.Result as AdaptiveCardPromptResult).Data as JObject;
            var resultArray = result.Properties().Select(p => $"Key: {p.Name}  |  Value: {p.Value}").ToList();
            var resultString = string.Join("\n\n", resultArray);

            await stepContext.Context.SendActivityAsync(resultString);

            return await stepContext.EndDialogAsync();
        }
    }
}
