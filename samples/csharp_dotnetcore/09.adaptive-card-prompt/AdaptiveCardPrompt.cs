// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

// SINCE ADAPTIVECARDPROMPT ISN'T YET IN THE SDK, EVERYTHING IN RemoveOncePromptInSDK WILL BE DELETED AND THIS WILL BE REMOVED FROM THE SAMPLE

namespace Microsoft.Bot.Builder.BotBuilderSamples
{
    public enum AdaptiveCardPromptErrors
    {
        /// <summary>
        /// No known user errors.
        /// </summary>
        None,

        /// <summary>
        /// Error presented if developer specifies AdaptiveCardPromptSettings.promptId,
        /// but user submits adaptive card input on a card where the ID does not match.
        /// This error will also be present if developer AdaptiveCardPromptSettings.promptId,
        /// but forgets to add the promptId to every <submit>.data.promptId in your Adaptive Card.
        /// </summary>
        UserInputDoesNotMatchCardId,

        /// <summary>
        /// Error presented if developer specifies AdaptiveCardPromptSettings.requiredIds,
        /// but user does not submit input for all required input id's on the adaptive card.
        /// </summary>
        MissingRequiredIds,

        /// <summary>
        /// Error presented if user enters plain text instead of using Adaptive Card's input fields.
        /// </summary>
        UserUsedTextInput
    }

    /// <summary>
    /// Waits for Adaptive Card Input to be received.
    /// </summary>
    /// <remarks>
    /// This prompt is similar to ActivityPrompt but provides features specific to Adaptive Cards:
    ///   * Optionally allow specified input fields to be required
    ///   * Optionally ensures input is only valid if it comes from the appropriate card (not one shown previous to prompt)
    ///   * Provides ability to handle variety of common user errors related to Adaptive Cards
    /// DO NOT USE WITH CHANNELS THAT DON'T SUPPORT ADAPTIVE CARDS.
    /// </remarks>
    public class AdaptiveCardPrompt : Dialog
    {
        private const string PersistedOptions = "options";
        private const string PersistedState = "state";

        private readonly PromptValidator<AdaptiveCardPromptResult> _validator;
        private readonly string[] _requiredInputIds;
        private readonly string _promptId;
        private readonly Attachment _card;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveCardPrompt"/> class.
        /// </summary>
        /// <param name="dialogId">Unique ID of the dialog within its parent `DialogSet` or `ComponentDialog`.</param>
        /// <param name="validator">(optional) Validator that will be called each time a new activity is received.</param>
        /// <param name="settings">(optional) Additional settings for AdaptiveCardPrompt behavior.</param>
        public AdaptiveCardPrompt(string dialogId, AdaptiveCardPromptSettings settings, PromptValidator<AdaptiveCardPromptResult> validator = null)
            : base(dialogId)
        {
            if (settings == null || settings.Card == null)
            {
                throw new ArgumentNullException("AdaptiveCardPrompt requires a card in `AdaptiveCardPromptSettings.card`");
            }

            _validator = validator;

            _requiredInputIds = settings.RequiredInputIds ?? null;

            ThrowIfNotAdaptiveCard(settings.Card);
            _card = settings.Card;

            _promptId = settings.PromptId;
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Initialize prompt state
            var state = dc.ActiveDialog.State;
            state[PersistedOptions] = options;
            state[PersistedState] = new Dictionary<string, object>
            {
                { Prompt<int>.AttemptCountKey, 0 },
            };

            // Send initial prompt
            await OnPromptAsync(dc.Context, (IDictionary<string, object>)state[PersistedState], (PromptOptions)state[PersistedOptions], false, cancellationToken).ConfigureAwait(false);

            return Dialog.EndOfTurn;
        }

        // Override ContinueDialogAsync so that we can catch Activity.Value (which is ignored, by default)
        public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            // Perform base recognition
            var instance = dc.ActiveDialog;
            var state = (IDictionary<string, object>)instance.State[PersistedState];
            var options = (PromptOptions)instance.State[PersistedOptions];
            var recognized = await OnRecognizeAsync(dc.Context, cancellationToken).ConfigureAwait(false);

            // Increment attempt count
            // Convert.ToInt32 For issue https://github.com/Microsoft/botbuilder-dotnet/issues/1859
            state[Prompt<int>.AttemptCountKey] = Convert.ToInt32(state[Prompt<int>.AttemptCountKey]) + 1;

            var isValid = false;
            if (_validator != null)
            {
                var promptContext = new PromptValidatorContext<AdaptiveCardPromptResult>(dc.Context, recognized, state, options);
                isValid = await _validator(promptContext, cancellationToken).ConfigureAwait(false);
            }
            else if (recognized.Succeeded)
            {
                isValid = true;
            }

            // Return recognized value or re-prompt
            if (isValid)
            {
                return await dc.EndDialogAsync(recognized.Value).ConfigureAwait(false);
            }
            else
            {
                // Re-prompt
                if (!dc.Context.Responded)
                {
                    await OnPromptAsync(dc.Context, state, options, true, cancellationToken).ConfigureAwait(false);
                }

                return Dialog.EndOfTurn;
            }
        }

        protected virtual async Task OnPromptAsync(ITurnContext context, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken)
        {
            // Since card is passed in via AdaptiveCardPromptSettings, PromptOptions may not be used.
            // Ensure we're working with RetryPrompt, as applicable
            var prompt = isRetry && options.RetryPrompt != null ? options.RetryPrompt : options.Prompt;

            // Clone the correct prompt (or new Activity, if null) so that we don't affect the one saved in state
            var clonedPrompt = JObject.FromObject(prompt ?? new Activity()).ToObject<Activity>();

            // The actual AdaptiveCard should be tightly-coupled to its AdaptiveCardPromptSettings, which means it would be instantiated in
            //   a Dialog's constructor and passed in when using AddDialog(AdaptiveCardPrompt) as opposed to passing into Activity.Attachments.
            //   The actual prompt is typically called by using stepContext.PromptAsync() in a WaterfallDialog step, where PromptOptions is
            //   required by DialogContext. However, PromptOptions isn't really required (or useful) for AdaptiveCardPrompt.
            // This next code block allows a user to simply call by setting Activity.Type to ActivityTypes.Message
            // "PromptAsync(nameof(AdaptiveCardPrompt), new PromptOptions())", instead of the uglier
            // "PromptAsync(nameof(AdaptiveCardPrompt), new PromptOptions(){ Prompt = MessageFactory.Text(string.Empty) })"
            // Note: PromptOptions does not need to be set in Node, since it compiles to JavaScript
            //   and the card gets sent without setting Activity.Type
            if (clonedPrompt.Type == null)
            {
                clonedPrompt.Type = ActivityTypes.Message;
            }

            // Add Adaptive Card as last attachment (user input should go last), keeping any others
            if (clonedPrompt.Attachments == null)
            {
                clonedPrompt.Attachments = new List<Attachment>();
            }

            clonedPrompt.Attachments.Add(_card);

            await context.SendActivityAsync(clonedPrompt, cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task<PromptRecognizerResult<AdaptiveCardPromptResult>> OnRecognizeAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            // Ignore user input that doesn't come from adaptive card
            if (string.IsNullOrWhiteSpace(context.Activity.Text) && context.Activity.Value != null)
            {
                var data = JObject.FromObject(context.Activity.Value);

                // Validate it comes from the correct card - This is only a worry while the prompt/dialog has not ended
                if (!string.IsNullOrEmpty(_promptId) && data["promptId"]?.ToString() != _promptId)
                {
                    return Task.FromResult(new PromptRecognizerResult<AdaptiveCardPromptResult>()
                    {
                        Succeeded = false,
                        Value = new AdaptiveCardPromptResult()
                        {
                            Data = data,
                            Error = AdaptiveCardPromptErrors.UserInputDoesNotMatchCardId
                        }
                    });
                }

                // Check for required input data, if specified in AdaptiveCardPromptSettings
                var missingIds = new List<string>();
                foreach (var id in _requiredInputIds ?? Enumerable.Empty<string>())
                {
                    if (data[id] == null || string.IsNullOrWhiteSpace(data[id].ToString()))
                    {
                        missingIds.Add(id);
                    }
                }

                // User did not submit inputs that were required
                if (missingIds.Count > 0)
                {
                    return Task.FromResult(new PromptRecognizerResult<AdaptiveCardPromptResult>()
                    {
                        Succeeded = false,
                        Value = new AdaptiveCardPromptResult()
                        {
                            Data = data,
                            Error = AdaptiveCardPromptErrors.MissingRequiredIds,
                            MissingIds = missingIds
                        }
                    });
                }

                return Task.FromResult(new PromptRecognizerResult<AdaptiveCardPromptResult>()
                {
                    Succeeded = true,
                    Value = new AdaptiveCardPromptResult() { Data = data }
                });
            }
            else
            {
                // User used text input instead of card input
                return Task.FromResult(new PromptRecognizerResult<AdaptiveCardPromptResult>()
                {
                    Succeeded = false,
                    Value = new AdaptiveCardPromptResult() { Error = AdaptiveCardPromptErrors.UserUsedTextInput }
                });
            }
        }

        private void ThrowIfNotAdaptiveCard(Attachment cardAttachment)
        {
            var adaptiveCardType = "application/vnd.microsoft.card.adaptive";

            if (cardAttachment == null || cardAttachment.Content == null)
            {
                throw new NullReferenceException($"No Adaptive Card provided. Include in the constructor or PromptOptions.Prompt.Attachments");
            }
            else if (string.IsNullOrEmpty(cardAttachment.ContentType) || cardAttachment.ContentType != adaptiveCardType)
            {
                throw new ArgumentException($"Attachment is not a valid Adaptive Card.\n" +
                                    $"Ensure Card.ContentType is '${adaptiveCardType}'\n" +
                                    "and Card.Content contains the card json");
            }
        }
    }
}
