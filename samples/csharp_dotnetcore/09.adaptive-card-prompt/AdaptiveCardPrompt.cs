using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

// SINCE ADAPTIVECARDPROMPT ISN'T YET IN THE SDK, EVERYTHING IN RemoveOncePromptInSDK WILL BE DELETED
// LIKEWISE, EVERYTHING IN THIS FILE CONTAINING Bot.Builder.CustomDialogs WILL CHANGE TO THE REGULAR,
// Bot.Builder.Dialogs VERSION

namespace Microsoft.Bot.Builder.BotBuilderSamples
{
    /// <summary>
    /// Waits for Adaptive Card Input to be received.
    /// </summary>
    /// <remarks>
    /// This prompt is similar to ActivityPrompt but provides features specific to Adaptive Cards:
    ///   * Card can be passed in constructor or as prompt/reprompt activity attachment
    ///   * Includes validation for specified required input fields
    ///   * Displays custom message if user replies via text and not card input
    ///   * Ensures input is only valid if it comes from the appropriate card (not one shown previous to prompt)
    /// DO NOT USE WITH CHANNELS THAT DON'T SUPPORT ADAPTIVE CARDS.
    /// </remarks>
    public class AdaptiveCardPrompt : Dialog
    {
        private const string PersistedOptions = "options";
        private const string PersistedState = "state";
        private bool usesCustomPromptId = false;

        private PromptValidator<object> _validator;
        private string _promptId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveCardPrompt"/> class.
        /// </summary>
        /// <param name="dialogId">Unique ID of the dialog within its parent `DialogSet` or `ComponentDialog`.</param>
        /// <param name="validator">(optional) Validator that will be called each time a new activity is received.</param>
        /// <param name="settings">(optional) Additional settings for AdaptiveCardPrompt behavior.</param>
        public AdaptiveCardPrompt(string dialogId, PromptValidator<object> validator = null, AdaptiveCardPromptSettings settings = null)
            : base(dialogId)
        {
            if (settings == null)
            {
                settings = new AdaptiveCardPromptSettings();
            }

            _validator = validator;
            InputFailMessage = settings.InputFailMessage ?? "Please fill out the Adaptive Card";

            RequiredInputIds = settings.RequiredInputIds ?? new List<string>();
            MissingRequiredInputsMessage = settings.MissingRequiredInputsMessage ?? "The following inputs are required";

            AttemptsBeforeCardRedisplayed = settings.AttemptsBeforeCardRedisplayed ?? 3;

            Card = settings.Card;

            if (!string.IsNullOrEmpty(settings.PromptId))
            {
                _promptId = settings.PromptId;
                usesCustomPromptId = true;
            }
        }

        public string PromptId
        {
            get
            {
                return _promptId;
            }

            set
            {
                usesCustomPromptId = string.IsNullOrEmpty(value) ? false : true;
                _promptId = value;
            }
        }

        private string InputFailMessage { get; set; }

        private List<string> RequiredInputIds { get; set; }

        private string MissingRequiredInputsMessage { get; set; }

        private int? AttemptsBeforeCardRedisplayed { get; set; }

        private Attachment Card { get; set; }

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
            if (recognized.Succeeded)
            {
                if (_validator != null)
                {
                    var promptContext = new PromptValidatorContext<object>(dc.Context, recognized, state, options);
                    isValid = await _validator(promptContext, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    isValid = true;
                }
            }


            // Return recognized value or re-prompt
            if (isValid)
            {
                return await dc.EndDialogAsync(recognized.Value).ConfigureAwait(false);
            }
            else
            {
                // Re-prompt, conditionally display card again
                if (Convert.ToInt32(state[Prompt<int>.AttemptCountKey]) % AttemptsBeforeCardRedisplayed == 0)
                {
                    await OnPromptAsync(dc.Context, state, options, true, cancellationToken).ConfigureAwait(false);
                }

                return Dialog.EndOfTurn;
            }
        }

        protected virtual async Task OnPromptAsync(ITurnContext context, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken)
        {
            // Only the most recently-promted card submission is acccepted
            _promptId = usesCustomPromptId ? _promptId : Guid.NewGuid().ToString();

            var prompt = isRetry && options.RetryPrompt != null ? options.RetryPrompt : options.Prompt;

            // Create a prompt if user didn't pass it in through PromptOptions
            if (prompt == null || prompt.Attachments?.Count == 0)
            {
                prompt = MessageFactory.Text(!string.IsNullOrEmpty(prompt.Text) ? prompt.Text : string.Empty);
                prompt.Attachments = new List<Attachment>();
            }

            // Use card passed into PromptOptions or if it doesn't exist, use the one passed in from the constructor
            var card = prompt.Attachments.Count > 0 ? prompt.Attachments[0] : Card;

            ValidateIsCard(card, isRetry);

            prompt.Attachments[0] = AddPromptIdToCard(card);

            await context.SendActivityAsync(prompt, cancellationToken).ConfigureAwait(false);
        }

        protected virtual async Task<PromptRecognizerResult<object>> OnRecognizeAsync(ITurnContext context, CancellationToken cancellationToken)
        {
            // Ignore user input that doesn't come from adaptive card
            if (string.IsNullOrWhiteSpace(context.Activity.Text) && context.Activity.Value != null)
            {
                var value = JObject.FromObject(context.Activity.Value);

                // Validate it comes from the correct card - This is only a worry while the prompt/dialog has not ended
                if (value["promptId"].ToString() != _promptId)
                {
                    return new PromptRecognizerResult<object>() { Succeeded = false };
                }

                // Check for required input data, if specified in AdpativeCardPromptOptions
                var missingIds = new List<string>();
                foreach (var id in RequiredInputIds)
                {
                    if (value[id] == null || string.IsNullOrWhiteSpace(value[id].ToString()))
                    {
                        missingIds.Add(id);
                    }
                }

                // Alert user to missing data
                if (missingIds.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(MissingRequiredInputsMessage))
                    {
                        await context.SendActivityAsync($"{MissingRequiredInputsMessage}: {string.Join(", ", missingIds)}", cancellationToken: cancellationToken).ConfigureAwait(false);
                    }

                    return new PromptRecognizerResult<object>() { Succeeded = false };
                }

                return new PromptRecognizerResult<object>() { Succeeded = true, Value = context.Activity.Value };
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(InputFailMessage))
                {
                    await context.SendActivityAsync(InputFailMessage, cancellationToken: cancellationToken).ConfigureAwait(false);
                }

                return new PromptRecognizerResult<object>() { Succeeded = false };
            }
        }

        private void ValidateIsCard(Attachment cardAttachment, bool isRetry)
        {
            var adaptiveCardType = "application/vnd.microsoft.card.adaptive";

            if (cardAttachment == null || cardAttachment.Content == null)
            {
                var cardLocation = isRetry ? "retryPrompt" : "prompt";
                throw new NullReferenceException($"No Adaptive Card provided. Include in the constructor or PromptOptions.{cardLocation}.Attachments[0]");
            }
            else if (string.IsNullOrEmpty(cardAttachment.ContentType) || cardAttachment.ContentType != adaptiveCardType)
            {
                throw new ArgumentException($"Attachment is not a valid Adaptive Card.\n" +
                                    "Ensure card.contentType is '${ adaptiveCardType }'\n" +
                                    "and card.content contains the card json");
            }
        }

        private Attachment AddPromptIdToCard(Attachment cardAttachment)
        {
            cardAttachment.Content = DeepSearchJsonForActionsAndAddPromptId((JObject)cardAttachment.Content);
            return cardAttachment;
        }

        private JObject DeepSearchJsonForActionsAndAddPromptId(JObject json)
        {
            foreach (var obj in json)
            {
                // Search for all submits in actions
                if (obj.Key == "actions")
                {
                    // Must convert json to list so that we can make changes to it
                    var i = 0;
                    foreach (var action in (obj.Value as JArray).ToObject<List<JObject>>())
                    {
                        json[obj.Key][i] = CheckAction((JObject)json[obj.Key][i]);
                        i++;
                    }
                }

                // Search for all submits in selectActions
                else if (obj.Key == "selectAction")
                {
                    json[obj.Key] = CheckAction((JObject)obj.Value);
                }

                // Recursively search all other objects
                else if (obj.Value is JObject)
                {
                    json[obj.Key] = DeepSearchJsonForActionsAndAddPromptId((JObject)obj.Value);
                }
            }

            return json;
        }

        private JObject CheckAction(JObject action)
        {
            var submitAction = "Action.Submit";
            var showCardAction = "Action.ShowCard";

            if (!string.IsNullOrWhiteSpace(action["type"].ToString()) && action["type"].ToString() == submitAction)
            {
                action["data"] = action["data"] ?? new JObject();
                action["data"]["promptId"] = _promptId;
            }
            else if (!string.IsNullOrWhiteSpace(action["type"].ToString()) && action["type"].ToString() == showCardAction)
            {
                // Recursively search Action.ShowCard for Submits within the nested card.
                // Note that there can't be a nested card in a select action
                // because Action.ShowCard is not supported in select actions.
                return DeepSearchJsonForActionsAndAddPromptId(action);
            }

            return action;
        }
    }
}
