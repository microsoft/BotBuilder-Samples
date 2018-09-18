// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MessageRoutingBot.Dialogs.Onboarding.Resources;
using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    /// <summary>
    /// Responses for the <see cref="OnboardingDialog"/>.
    /// </summary>
    public class OnboardingResponses : TemplateManager
    {
        public const string _namePrompt = "namePrompt";
        public const string _haveName = "haveName";
        public const string _emailPrompt = "emailPrompt";
        public const string _haveEmail = "haveEmail";
        public const string _locationPrompt = "locationPrompt";
        public const string _haveLocation = "haveLocation";

        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                {
                    _namePrompt,
                    (context, data) => OnboardingStrings.NAME_PROMPT
                },
                {
                    _haveName,
                    (context, data) => string.Format(OnboardingStrings.HAVE_NAME, data.name)
                },
                {
                    _emailPrompt,
                    (context, data) => OnboardingStrings.EMAIL_PROMPT
                },
                {
                    _haveEmail,
                    (context, data) => string.Format(OnboardingStrings.HAVE_EMAIL, data.email)
                },
                {
                    _locationPrompt,
                    (context, data) => OnboardingStrings.LOCATION_PROMPT
                },
                {
                    _haveLocation,
                    (context, data) => string.Format(OnboardingStrings.HAVE_LOCATION, data.name, data.location)
                },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingResponses"/> class.
        /// </summary>
        public OnboardingResponses()
        {
            this.Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
