// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples.Components.Actions
{
    /// <summary>
    /// Executes a set of actions while an expression evaluates to true.
    /// </summary>
    public class DoWhile : ActionScope
    {
        /// <summary>
        /// Class identifier.
        /// </summary>
        [JsonProperty("$kind")]
        public const string Kind = "BotBuilderSamples.DoWhile";

        /// <summary>
        /// Initializes a new instance of the <see cref="DoWhile"/> class.
        /// </summary>
        /// <param name="sourceFilePath">Optional, full path of the source file that contains the caller.</param>
        /// <param name="sourceLineNumber">optional, line number in the source file at which the method is called.</param>
        [JsonConstructor]
        public DoWhile([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Gets or sets an optional expression which if is true will disable this action.
        /// </summary>
        /// <value>
        /// A boolean expression. 
        /// </value>
        [JsonProperty("disabled")]
        public BoolExpression Disabled { get; set; }

        /// <summary>
        /// Gets or sets the memory expression evaludating to a condition which determines if the DoWhile should continue.
        /// </summary>
        /// <value>
        /// The memory expression which determines if the DoWhile should continue.
        /// </value>
        [JsonProperty("condition")]
        public BoolExpression Condition { get; set; } = false;

        /// <summary>
        /// Gets or sets an optional expression which will ensure the loop runs once, before checking the condition.
        /// </summary>
        /// <value>
        /// A boolean expression. 
        /// </value>
        [JsonProperty("runOnce")]
        public BoolExpression RunOnce { get; set; } = true;

        /// <summary>
        /// Called when the dialog is started and pushed onto the dialog stack.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> for the current turn of conversation.</param>
        /// <param name="options">Optional, initial information to pass to the dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options is CancellationToken)
            {
                throw new ArgumentException($"{nameof(options)} cannot be a cancellation token");
            }

            if (Disabled != null && Disabled.GetValue(dc.State))
            {
                return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if(Condition == null)
            {
                throw new InvalidOperationException("Condition is required for DoWhile.");
            }

            // If RunOnce is false, end the dialog if Condition evaluates to true.
            if (RunOnce != null && RunOnce.GetValue(dc.State) == false)
            {
                if (Condition.GetValue(dc.State) == false)
                {
                    return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }

            return await base.BeginDialogAsync(dc, options, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when the dialog's action ends.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> for the current turn of conversation.</param>
        /// <param name="result">Optional, value returned from the dialog that was called. The type
        /// of the value returned is dependent on the child dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task<DialogTurnResult> OnEndOfActionsAsync(DialogContext dc, object result = null, CancellationToken cancellationToken = default)
        {
            if (Condition.GetValue(dc.State))
            {
                return await BeginActionAsync(dc, 0, cancellationToken).ConfigureAwait(false);
            }

            return await base.OnEndOfActionsAsync(dc, result, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Called when a returning control to this dialog with an <see cref="ActionScopeResult"/>
        /// with the property ActionCommand set to <c>BreakLoop</c>.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of the conversation.</param>
        /// <param name="actionScopeResult">Contains the actions scope result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task<DialogTurnResult> OnBreakLoopAsync(DialogContext dc, ActionScopeResult actionScopeResult, CancellationToken cancellationToken = default)
        {
            return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when a returning control to this dialog with an <see cref="ActionScopeResult"/>
        /// with the property ActionCommand set to <c>ContinueLoop</c>.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of the conversation.</param>
        /// <param name="actionScopeResult">Contains the actions scope result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task<DialogTurnResult> OnContinueLoopAsync(DialogContext dc, ActionScopeResult actionScopeResult, CancellationToken cancellationToken = default)
        {
            if (Condition.GetValue(dc.State))
            {
                return await BeginActionAsync(dc, 0, cancellationToken).ConfigureAwait(false);
            }

            return await base.OnEndOfActionsAsync(dc, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Builds the compute Id for the dialog.
        /// </summary>
        /// <returns>A string representing the compute Id.</returns>
        protected override string OnComputeId()
        {
            return $"{GetType().Name}({Condition?.ToString()})";
        }
    }
}
