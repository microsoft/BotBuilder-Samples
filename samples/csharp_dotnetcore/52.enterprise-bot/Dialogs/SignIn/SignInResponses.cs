// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using EnterpriseBot.Dialogs.SignIn.Resources;
using Microsoft.Bot.Builder.TemplateManager;

namespace EnterpriseBot
{
    public class SignInResponses : TemplateManager
    {
        // Constants
        public const string SignInPrompt = "namePrompt";
        public const string Succeeded = "haveName";
        public const string Failed = "emailPrompt";

        // Fields
        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                {
                    SignInPrompt,
                    (context, data) => SignInStrings.PROMPT
                },
                {
                    Succeeded,
                    (context, data) => string.Format(SignInStrings.SUCCEEDED, data.name)
                },
                {
                    Failed,
                    (context, data) => SignInStrings.FAILED
                },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        public SignInResponses()
        {
            this.Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
