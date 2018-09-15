// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.TemplateManager;

namespace MessageRoutingBot
{
    /// <summary>
    /// View for the <see cref="OnboardingDialog"/>.
    /// </summary>
    public class OnboardingView : TemplateManager
    {
        // Constants
        public const string Intro = "OnboardingDialog.Intro";
        public const string NamePrompt = "OnboardingDialog.NamePrompt";
        public const string HaveName = "OnboardingDialog.HaveName";
        public const string EmailPrompt = "OnboardingDialog.EmailPrompt";
        public const string HaveEmail = "OnboardingDialog.HaveEmail";
        public const string LocationPrompt = "OnboardingDialog.LocationPrompt";
        public const string HaveLocation = "OnboardingDialog.HaveLocation";

        // Fields
        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { NamePrompt, (context, data) => "What is your name?" },
                { HaveName, (context, data) => $"Hi, {data.name}!" },
                { EmailPrompt, (context, data) => "What is your email?" },
                { HaveEmail, (context, data) => $"Got it. I've added {data.email} as your primary contact address." },
                { LocationPrompt, (context, data) => "Finally, where are you located?" },
                { HaveLocation, (context, data) => $"Thanks, {data.name}. I've added {data.location} as your primary location. You're all set up!" },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingView"/> class.
        /// </summary>
        public OnboardingView()
        {
            this.Register(new DictionaryRenderer(_responseTemplates));
        }
    }
}
