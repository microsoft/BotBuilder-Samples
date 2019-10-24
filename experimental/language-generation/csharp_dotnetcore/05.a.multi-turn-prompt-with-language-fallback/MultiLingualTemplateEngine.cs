// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections.Generic;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.LanguageGeneration;
using ActivityBuilder = Microsoft.Bot.Builder.Dialogs.Adaptive.Generators.ActivityGenerator;
using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    public class MultiLingualTemplateEngineManager
    {
        public Dictionary<string, TemplateEngine> TemplateEnginesPerLocale { get; set; } = new Dictionary<string, TemplateEngine>();
        private LanguagePolicy LangFallBackPolicy;

        public MultiLingualTemplateEngineManager(Dictionary<string, List<string>> lgFilesPerLocale)
        {
            if (lgFilesPerLocale == null)
            {
                throw new ArgumentNullException(nameof(lgFilesPerLocale));
            }

            InternalConstructor(lgFilesPerLocale);
        }

        public Activity GenerateActivityForLocale(string templateName, object data, WaterfallStepContext stepContext)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (stepContext == null || stepContext.Context == null || stepContext.Context.Activity == null)
            {
                throw new ArgumentNullException(nameof(stepContext));
            }
            return InternalGenerateActivityForLocale(templateName, data, stepContext.Context.Activity.Locale);

        }

        public Activity GenerateActivityForLocale(string templateName, object data, ITurnContext turnContext)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (turnContext == null || turnContext.Activity == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }
            return InternalGenerateActivityForLocale(templateName, data, turnContext.Activity.Locale);
        }

        public Activity GenerateActivityForLocale(string templateName, WaterfallStepContext stepContext)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (stepContext == null || stepContext.Context == null || stepContext.Context.Activity == null)
            {
                throw new ArgumentNullException(nameof(stepContext));
            }
            return InternalGenerateActivityForLocale(templateName, null, stepContext.Context.Activity.Locale);
        }

        public Activity GenerateActivityForLocale(string templateName, TurnContext turnContext)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (turnContext == null || turnContext.Activity == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }
            return InternalGenerateActivityForLocale(templateName, null, turnContext.Activity.Locale);
        }

        private Activity InternalGenerateActivityForLocale(string templateName, object data, string locale)
        {
            var iLocale = locale == null ? "" : locale;

            if (TemplateEnginesPerLocale.ContainsKey(iLocale))
            {
                return ActivityBuilder.GenerateFromLG(TemplateEnginesPerLocale[locale].EvaluateTemplate(templateName, data));
            }
            var locales = new string[] { string.Empty };
            if (!LangFallBackPolicy.TryGetValue(iLocale, out locales))
            {
                if (!LangFallBackPolicy.TryGetValue(string.Empty, out locales))
                {
                    throw new Exception($"No supported language found for {locale}");
                }
            }

            foreach (var fallBackLocale in locales)
            {
                if (TemplateEnginesPerLocale.ContainsKey(fallBackLocale))
                {
                    return ActivityBuilder.GenerateFromLG(TemplateEnginesPerLocale[fallBackLocale].EvaluateTemplate(templateName, data));
                }
            }
            return new Activity();
        }

        private void InternalConstructor(Dictionary<string, List<string>> lgFilesPerLocale, Dictionary<string, string> fallBackPolicy = null)
        {
            foreach (KeyValuePair<string, List<string>> filesPerLocale in lgFilesPerLocale)
            {
                TemplateEnginesPerLocale[filesPerLocale.Key] = new TemplateEngine();
                TemplateEnginesPerLocale[filesPerLocale.Key].AddFiles(filesPerLocale.Value);
            }
            LangFallBackPolicy = new LanguagePolicy();
        }

    }
}