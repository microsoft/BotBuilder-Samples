// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    public class CancelView : TemplateManager
    {
        public const string ConfirmPrompt = "ConfirmCancelPrompt";
        public const string CancelConfirmed = "CancelConfirmed";
        public const string CancelDenied = "CancelDenied";

        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { ConfirmPrompt, (context, data) => "Are you sure you want to cancel?" },
                { CancelConfirmed, (context, data) => "Ok, let's start over." },
                { CancelDenied, (context, data) => "Ok, let's keep going." },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        public CancelView()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
