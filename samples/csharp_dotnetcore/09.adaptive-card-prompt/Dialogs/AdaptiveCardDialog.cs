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
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples
{
    public class AdaptiveCardDialog : ComponentDialog
    {
        public readonly string SIMPLE_ADAPTIVE_CARD_PROMPT = "SIMPLE";
        public readonly string COMPLEX_ADAPTIVE_CARD_PROMPT = "COMPLEX";

        public AdaptiveCardDialog()
            : base(nameof(AdaptiveCardDialog))
        {
            // Load adaptive cards
            var simpleAdaptiveCard = CreateCard(Path.Combine("./Resources/", "simpleAdaptiveCard.json"));
            var simpleSettings = new AdaptiveCardPromptSettings()
            {
                Card = simpleAdaptiveCard
            };
            AddDialog(new AdaptiveCardPrompt(SIMPLE_ADAPTIVE_CARD_PROMPT, simpleSettings));

            var complexAdaptiveCard = CreateCard(Path.Combine("./Resources/", "complexAdaptiveCard.json"));
            var complexSettings = new AdaptiveCardPromptSettings()
            {
                Card = complexAdaptiveCard,
                RequiredInputIds = new string[] { "textInput" },
                PromptId = "complexCard"
            };
            AddDialog(new AdaptiveCardPrompt(COMPLEX_ADAPTIVE_CARD_PROMPT, complexSettings, ValidateInput));

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                WhichCardAsync,
                ShowCardAsync,
                DisplayResultsAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> WhichCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = "Please choose which card to display:\n\n" +
            "* **Simple**: Displays basic actions of AdaptiveCardPrompt\n" +
            "* **Complex**: Display a complex adaptive card that catches common user errors with a validator";
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(text),
                Choices = ChoiceFactory.ToChoices(new string[] { "Simple", "Complex" })
            });
        }

        private async Task<DialogTurnResult> ShowCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var wrongCard = CreateCard(Path.Combine("./Resources/", "wrongAdaptiveCard.json"));
            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Simple":
                    // Call the prompt - Note: PromptOptions is a required argument for StepContext and DialogContext, but doesn't need to be filled out
                    return await stepContext.PromptAsync(SIMPLE_ADAPTIVE_CARD_PROMPT, new PromptOptions());
                case "Complex":
                    // Attach an additional card to the prompt so the user can try to reproduce the "wrong card id" error
                    return await stepContext.PromptAsync(COMPLEX_ADAPTIVE_CARD_PROMPT, new PromptOptions()
                    {
                        Prompt = new Activity()
                        {
                            Attachments = new Attachment[] { wrongCard }
                        }
                    });
                default:
                    break;
            }
            return await stepContext.EndDialogAsync();
        }

        private async Task<DialogTurnResult> DisplayResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Success!");

            // Send the result to the user
            var result = (stepContext.Result as AdaptiveCardPromptResult).Data as JObject;
            var resultArray = result.Properties().Select(p => $"* **{p.Name}**: {p.Value}").ToList();
            var resultString = string.Join("\n\n", resultArray);

            await stepContext.Context.SendActivityAsync(resultString);

            await stepContext.Context.SendActivityAsync("Type anything to start again.");

            return await stepContext.EndDialogAsync();
        }

        private Attachment CreateCard(string filePath)
        {
            var cardJson = File.ReadAllText(filePath);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };
        }

        private async Task<bool> ValidateInput(Bot.Builder.BotBuilderSamples.PromptValidatorContext<AdaptiveCardPromptResult> promptContext, CancellationToken cancellationToken)
        {
            switch (promptContext.Recognized.Value.Error)
            {
                case AdaptiveCardPromptErrors.None:
                    break;
                case AdaptiveCardPromptErrors.UserInputDoesNotMatchCardId:
                    await promptContext.Context.SendActivityAsync("Please submit input on the correct adaptive card");
                    return false;
                case AdaptiveCardPromptErrors.MissingRequiredIds:
                    var missingIds = promptContext.Recognized.Value.MissingIds;
                    await promptContext.Context.SendActivityAsync(
                        "Please fill out all of the required inputs\n\n" +
                        "Required inputs:\n\n" +
                        $"{ string.Join("\n\n", missingIds) }");
                    return false;
                case AdaptiveCardPromptErrors.UserUsedTextInput:
                    await promptContext.Context.SendActivityAsync("Please use the card to submit input");
                    return false;
                default:
                    break;
            }
            return true;
        }
    }
}
