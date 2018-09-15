// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    /// <summary>
    /// View elements for the <see cref="EscalateDialog"/>.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EscalateView"/> class.
        /// </summary>
        public EscalateView()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
