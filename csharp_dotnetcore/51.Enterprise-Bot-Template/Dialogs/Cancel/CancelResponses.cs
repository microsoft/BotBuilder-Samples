// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using EnterpriseBot.Dialogs.Cancel.Resources;
using Microsoft.Bot.Builder.TemplateManager;

namespace EnterpriseBot
{
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

        public CancelResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
