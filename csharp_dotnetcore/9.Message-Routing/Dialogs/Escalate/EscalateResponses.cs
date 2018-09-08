// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MessageRoutingBot.Dialogs.Escalate.Resources;
using MessageRoutingBot.Dialogs.Main.Resources;
using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    /// <summary>
    /// Responses for the <see cref="EscalateDialog"/>.
    /// </summary>
    public class EscalateResponses : TemplateManager
    {
        public const string SendPhone = "sendPhone";

        private LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { SendPhone, (context, data) => EscalateStrings.PHONE_INFO },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="EscalateResponses"/> class.
        /// </summary>
        public EscalateResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
