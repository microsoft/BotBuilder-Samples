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
        public const string confirmPrompt = "Cancel.ConfirmCancelPrompt";
        public const string cancelConfirmed = "Cancel.CancelConfirmed";
        public const string cancelDenied = "Cancel.CancelDenied";

        // Fields
        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { confirmPrompt, (context, data) => CancelStrings.CANCEL_PROMPT },
                { cancelConfirmed, (context, data) => CancelStrings.CANCEL_CONFIRMED },
                { cancelDenied, (context, data) => CancelStrings.CANCEL_DENIED },
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
