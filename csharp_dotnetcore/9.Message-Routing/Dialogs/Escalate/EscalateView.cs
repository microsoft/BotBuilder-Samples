// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    public class EscalateView : TemplateManager
    {
        public const string SendPhone = "sendPhone";

        private LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { SendPhone, (context, data) => "Our agents are available 24/7 at 1(800)555-1234." },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        public EscalateView()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
