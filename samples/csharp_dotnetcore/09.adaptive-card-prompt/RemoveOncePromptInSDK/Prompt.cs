// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// THIS IS ONLY HERE BECAUSE BOT.BUILDER-DIALOGS.PROMPT.ATTEMPTCOUNT IS INTERNAL
// TODO: REMOVE THIS FROM SAMPLE ONCE ADPATIVECARDPROMPT IS IN SDK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.BotBuilderSamples
{
    /// <summary>
    /// Defines the core behavior of prompt dialogs.
    /// </summary>
    /// <typeparam name="T">The type of value the prompt returns.</typeparam>
    /// <remarks>When the prompt ends, it should return a <typeparamref name="T"/> object that
    /// represents the value that was prompted for.
    /// Use <see cref="DialogSet.Add(Dialog)"/> or <see cref="ComponentDialog.AddDialog(Dialog)"/>
    /// to add a prompt to a dialog set or component dialog, respectively.
    /// Use <see cref="DialogContext.PromptAsync(string, PromptOptions, CancellationToken)"/> or
    /// <see cref="DialogContext.BeginDialogAsync(string, object, CancellationToken)"/> to start the prompt.
    /// If you start a prompt from a <see cref="WaterfallStep"/> in a <see cref="WaterfallDialog"/>,
    /// then the prompt result will be available in the next step of the waterfall.
    /// </remarks>
    public abstract class Prompt<T> : Dialog
    {
        internal const string AttemptCountKey = "AttemptCount";

        private const string PersistedOptions = "options";
        private const string PersistedState = "state";
        private readonly PromptValidator<T> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Prompt{T}"/> class.
        /// Called from constructors in derived classes to initialize the <see cref="Prompt{T}"/> class.
        /// </summary>
        /// <param name="dialogId">The ID to assign to this prompt.</param>
        /// <param name="validator">Optional, a <see cref="PromptValidator{T}"/> that contains additional,
        /// custom validation for this prompt.</param>
        /// <remarks>The value of <paramref name="dialogId"/> must be unique within the
        /// <see cref="DialogSet"/> or <see cref="ComponentDialog"/> to which the prompt is added.</remarks>
        public Prompt(string dialogId, PromptValidator<T> validator = null)
            : base(dialogId)
        {
            if (string.IsNullOrWhiteSpace(dialogId))
            {
                throw new ArgumentNullException(nameof(dialogId));
            }

            _validator = validator;
        }

        /// <summary>
        /// Called when a prompt dialog is pushed onto the dialog stack and is being activated.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of the conversation.</param>
        /// <param name="options">Optional, additional information to pass to the prompt being started.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>If the task is successful, the result indicates whether the prompt is still
        /// active after the turn has been processed by the prompt.</remarks>
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (options is CancellationToken)
            {
                throw new ArgumentException($"{nameof(options)} cannot be a cancellation token");
            }

            if (!(options is PromptOptions))
            {
                throw new ArgumentOutOfRangeException(nameof(options), "Prompt options are required for Prompt dialogs");
            }

            // Ensure prompts have input hint set
            var opt = (PromptOptions)options;
            if (opt.Prompt != null && string.IsNullOrEmpty(opt.Prompt.InputHint))
            {
                opt.Prompt.InputHint = InputHints.ExpectingInput;
            }

            if (opt.RetryPrompt != null && string.IsNullOrEmpty(opt.RetryPrompt.InputHint))
            {
                opt.RetryPrompt.InputHint = InputHints.ExpectingInput;
            }

            // Initialize prompt state
            var state = dc.ActiveDialog.State;
            state[PersistedOptions] = opt;
            state[PersistedState] = new Dictionary<string, object>
            {
                { AttemptCountKey, 0 },
            };

            // Send initial prompt
            await OnPromptAsync(dc.Context, (IDictionary<string, object>)state[PersistedState], (PromptOptions)state[PersistedOptions], false, cancellationToken).ConfigureAwait(false);
            return EndOfTurn;
        }

        /// <summary>
        /// Called when a prompt dialog is the active dialog and the user replied with a new activity.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of conversation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>If the task is successful, the result indicates whether the dialog is still
        /// active after the turn has been processed by the dialog.
        /// <para>The prompt generally continues to receive the user's replies until it accepts the
        /// user's reply as valid input for the prompt.</para></remarks>
        public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            // Don't do anything for non-message activities
            if (dc.Context.Activity.Type != ActivityTypes.Message)
            {
                return EndOfTurn;
            }

            // Perform base recognition
            var instance = dc.ActiveDialog;
            var state = (IDictionary<string, object>)instance.State[PersistedState];
            var options = (PromptOptions)instance.State[PersistedOptions];
            var recognized = await OnRecognizeAsync(dc.Context, state, options, cancellationToken).ConfigureAwait(false);

            // Increment attempt count
            // Convert.ToInt32 For issue https://github.com/Microsoft/botbuilder-dotnet/issues/1859
            state[AttemptCountKey] = Convert.ToInt32(state[AttemptCountKey]) + 1;

            // Validate the return value
            var isValid = false;
            if (_validator != null)
            {
                var promptContext = new PromptValidatorContext<T>(dc.Context, recognized, state, options);
                isValid = await _validator(promptContext, cancellationToken).ConfigureAwait(false);
            }
            else if (recognized.Succeeded)
            {
                isValid = true;
            }

            // Return recognized value or re-prompt
            if (isValid)
            {
                return await dc.EndDialogAsync(recognized.Value, cancellationToken).ConfigureAwait(false);
            }

            if (!dc.Context.Responded)
            {
                await OnPromptAsync(dc.Context, state, options, true, cancellationToken).ConfigureAwait(false);
            }

            return EndOfTurn;
        }

        /// <summary>
        /// Called when a prompt dialog resumes being the active dialog on the dialog stack, such as
        /// when the previous active dialog on the stack completes.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of the conversation.</param>
        /// <param name="reason">An enum indicating why the dialog resumed.</param>
        /// <param name="result">Optional, value returned from the previous dialog on the stack.
        /// The type of the value returned is dependent on the previous dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>If the task is successful, the result indicates whether the dialog is still
        /// active after the turn has been processed by the dialog.</remarks>
        public override async Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (result is CancellationToken)
            {
                throw new ArgumentException($"{nameof(result)} cannot be a cancellation token");
            }

            // Prompts are typically leaf nodes on the stack but the dev is free to push other dialogs
            // on top of the stack which will result in the prompt receiving an unexpected call to
            // dialogResume() when the pushed on dialog ends.
            // To avoid the prompt prematurely ending we need to implement this method and
            // simply re-prompt the user.
            await RepromptDialogAsync(dc.Context, dc.ActiveDialog, cancellationToken).ConfigureAwait(false);
            return EndOfTurn;
        }

        /// <summary>
        /// Called when a prompt dialog has been requested to re-prompt the user for input.
        /// </summary>
        /// <param name="turnContext">Context for the current turn of conversation with the user.</param>
        /// <param name="instance">The instance of the dialog on the stack.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task RepromptDialogAsync(ITurnContext turnContext, DialogInstance instance, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = (IDictionary<string, object>)instance.State[PersistedState];
            var options = (PromptOptions)instance.State[PersistedOptions];
            await OnPromptAsync(turnContext, state, options, false, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task<bool> OnPreBubbleEventAsync(DialogContext dc, DialogEvent e, CancellationToken cancellationToken)
        {
            if (e.Name == DialogEvents.ActivityReceived && dc.Context.Activity.Type == ActivityTypes.Message)
            {
                // Perform base recognition
                var state = dc.ActiveDialog.State;
                var recognized = await this.OnRecognizeAsync(dc.Context, (IDictionary<string, object>)state[PersistedState], (PromptOptions)state[PersistedOptions]).ConfigureAwait(false);
                return recognized.Succeeded;
            }

            return false;
        }

        /// <summary>
        /// When overridden in a derived class, prompts the user for input.
        /// </summary>
        /// <param name="turnContext">Context for the current turn of conversation with the user.</param>
        /// <param name="state">Contains state for the current instance of the prompt on the dialog stack.</param>
        /// <param name="options">A prompt options object constructed from the options initially provided
        /// in the call to <see cref="DialogContext.PromptAsync(string, PromptOptions, CancellationToken)"/>.</param>
        /// <param name="isRetry">true if this is the first time this prompt dialog instance
        /// on the stack is prompting the user for input; otherwise, false.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task OnPromptAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// When overridden in a derived class, attempts to recognize the user's input.
        /// </summary>
        /// <param name="turnContext">Context for the current turn of conversation with the user.</param>
        /// <param name="state">Contains state for the current instance of the prompt on the dialog stack.</param>
        /// <param name="options">A prompt options object constructed from the options initially provided
        /// in the call to <see cref="DialogContext.PromptAsync(string, PromptOptions, CancellationToken)"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>If the task is successful, the result describes the result of the recognition attempt.</remarks>
        protected abstract Task<PromptRecognizerResult<T>> OnRecognizeAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// When overridden in a derived class, appends choices to the activity when the user is prompted for input.
        /// </summary>
        /// <param name="prompt">The activity to append the choices to.</param>
        /// <param name="channelId">The ID of the user's channel.</param>
        /// <param name="choices">The choices to append.</param>
        /// <param name="style">Indicates how the choices should be presented to the user.</param>
        /// <param name="options">The formatting options to use when presenting the choices.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>If the task is successful, the result contains the updated activity.</remarks>
        protected IMessageActivity AppendChoices(IMessageActivity prompt, string channelId, IList<Choice> choices, ListStyle style, ChoiceFactoryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get base prompt text (if any)
            var text = prompt != null && !string.IsNullOrEmpty(prompt.Text) ? prompt.Text : string.Empty;

            // Create temporary msg
            IMessageActivity msg;
            switch (style)
            {
                case ListStyle.Inline:
                    msg = ChoiceFactory.Inline(choices, text, null, options);
                    break;

                case ListStyle.List:
                    msg = ChoiceFactory.List(choices, text, null, options);
                    break;

                case ListStyle.SuggestedAction:
                    msg = ChoiceFactory.SuggestedAction(choices, text);
                    break;

                case ListStyle.HeroCard:
                    msg = ChoiceFactory.HeroCard(choices, text);
                    break;

                case ListStyle.None:
                    msg = Activity.CreateMessageActivity();
                    msg.Text = text;
                    break;

                default:
                    msg = ChoiceFactory.ForChannel(channelId, choices, text, null, options);
                    break;
            }

            // Update prompt with text, actions and attachments
            if (prompt != null)
            {
                // clone the prompt the set in the options (note ActivityEx has Properties so this is the safest mechanism)
                prompt = JsonConvert.DeserializeObject<Activity>(JsonConvert.SerializeObject(prompt));

                prompt.Text = msg.Text;

                if (msg.SuggestedActions?.Actions != null && msg.SuggestedActions.Actions.Count > 0)
                {
                    prompt.SuggestedActions = msg.SuggestedActions;
                }

                if (msg.Attachments != null && msg.Attachments.Any())
                {
                    if (prompt.Attachments == null)
                    {
                        prompt.Attachments = msg.Attachments;
                    }
                    else
                    {
                        prompt.Attachments = prompt.Attachments.Concat(msg.Attachments).ToList();
                    }
                }

                return prompt;
            }
            else
            {
                msg.InputHint = InputHints.ExpectingInput;
                return msg;
            }
        }
    }
}
