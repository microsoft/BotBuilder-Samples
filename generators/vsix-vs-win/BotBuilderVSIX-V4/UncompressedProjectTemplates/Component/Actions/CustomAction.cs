// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v$templateversion$

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace $safeprojectname$.Actions
{
    public class CustomAction : Dialog
    {
        /// <summary>
        /// Class identifier.
        /// </summary>
        [JsonProperty("$kind")]
        public const string Kind = "$safeprojectname$.CustomAction";

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAction"/> class.
        /// </summary>
        /// <param name="sourceFilePath">Optional, full path of the source file that contains the caller.</param>
        /// <param name="sourceLineNumber">optional, line number in the source file at which the method is called.</param>
        [JsonConstructor]
        public CustomAction([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Gets or sets memory path to bind to property1 (ex: conversation.width).
        /// </summary>
        /// <value>
        /// Memory path to bind to property1 (ex: conversation.width).
        /// </value>
        [JsonProperty("property1")]
        public StringExpression Property1 { get; set; }

        /// <summary>
        /// Gets or sets the property path to store the custom action result in.
        /// </summary>
        /// <value>
        /// The property path to store the dialog result in.
        /// </value>
        [JsonProperty("resultProperty")]
        public StringExpression ResultProperty { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
                // Retrieve property values.
                var property1 = Property1.GetValue(dc.State);

                // Perform operations, store the result in the property defined,
                // then  return the result.
                var result = property1;
                
                if (this.ResultProperty != null)
                {
                    dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
                }
                
                return dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
        }
    }
}
