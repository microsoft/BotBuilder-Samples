// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace Microsoft.Form.Extensions
{
    /// <summary>
    /// Control what is collected in a form.
    /// </summary>
    public class ControlForm : Dialog
    {
        /// <summary>
        /// Class identifier.
        /// </summary>
        [JsonProperty("$kind")]
        public const string Kind = "Microsoft.ControlForm";

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlForm"/> class.
        /// </summary>
        /// <param name="callerPath">Optional, source file full path.</param>
        /// <param name="callerLine">Optional, line number in source file.</param>
        public ControlForm([CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
        {
            RegisterSourceLocation(callerPath, callerLine);
        }

        /// <summary>
        /// Gets or sets an optional expression which if is true will disable this action.
        /// </summary>
        /// <example>
        /// "user.age > 18".
        /// </example>
        /// <value>
        /// A boolean expression. 
        /// </value>
        [JsonProperty("disabled")]
        public BoolExpression Disabled { get; set; }

        /// <summary>
        /// Gets or sets a list of property names to push to the front of dialog.requiredPropties.
        /// </summary>
        /// <value>A list of property names.</value>
        [JsonProperty("askFirst")]
        public ArrayExpression<string> AskFirst { get; set; }

        /// <summary>
        /// Gets or sets a list of property names to remove from dialog.requiredPropties.
        /// </summary>
        /// <value>A list of property names.</value>
        [JsonProperty("askLast")]
        public ArrayExpression<string> AskLast { get; set; }

        /// <summary>
        /// Gets or sets a list of property names to remove from dialog.requiredPropties.
        /// </summary>
        /// <value>A list of property names.</value>
        [JsonProperty("noAsk")]
        public ArrayExpression<string> NoAsk { get; set; }

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

            var askFirst = AskFirst?.GetValue(dc) ?? new List<string>();
            var askLast = AskLast?.GetValue(dc) ?? new List<string>(); 
            var noAsk = NoAsk?.GetValue(dc) ?? new List<string>();
            var required = dc.State.GetValue<List<string>>(DialogPath.RequiredProperties) ?? new List<string>();

            // Ensure there is no overlap
            askFirst.RemoveAll(p => noAsk.Contains(p));
            askLast.RemoveAll(p => noAsk.Contains(p) || askFirst.Contains(p));
            required.RemoveAll(p => noAsk.Contains(p) || askFirst.Contains(p) || askLast.Contains(p));
            required = askFirst.Union(required).Union(askLast).ToList();
            dc.State.SetValue(DialogPath.RequiredProperties, required);
            return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
