// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MessageRoutingBot.Dialogs.Cancel.Resources;
using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    /// <summary>
    /// Responses for the <see cref="CancelDialog"/>.
    /// </summary>
    public class CancelResponses : TemplateManager
    {
        // Constants
        public const string _confirmPrompt = "Cancel.ConfirmCancelPrompt";
        public const string _cancelConfirmed = "Cancel.CancelConfirmed";
        public const string _cancelDenied = "Cancel.CancelDenied";

        // Fields
        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { _confirmPrompt, (context, data) => CancelStrings.CANCEL_PROMPT },
                { _cancelConfirmed, (context, data) => CancelStrings.CANCEL_CONFIRMED },
                { _cancelDenied, (context, data) => CancelStrings.CANCEL_DENIED },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelResponses"/> class.
        /// </summary>
        public CancelResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
