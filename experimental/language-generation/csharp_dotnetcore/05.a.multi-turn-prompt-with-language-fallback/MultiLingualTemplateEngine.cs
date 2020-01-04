// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections.Generic;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.LanguageGeneration;
using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;

namespace Microsoft.BotBuilderSamples
{
    public class MultiLingualTemplateEngine
    {
        public Dictionary<string, TemplateEngine> TemplateEnginesPerLocale { get; set; } = new Dictionary<string, TemplateEngine>();
        private LanguagePolicy LangFallBackPolicy;

        public MultiLingualTemplateEngine(Dictionary<string, List<string>> lgFilesPerLocale)
        {
            if (lgFilesPerLocale == null)
            {
                throw new ArgumentNullException(nameof(lgFilesPerLocale));
            }

            foreach (KeyValuePair<string, List<string>> filesPerLocale in lgFilesPerLocale)
            {
                TemplateEnginesPerLocale[filesPerLocale.Key] = new TemplateEngine();
                TemplateEnginesPerLocale[filesPerLocale.Key].AddFiles(filesPerLocale.Value);
            }
            LangFallBackPolicy = new LanguagePolicy();        
        }

        public Activity GenerateActivity(string templateName, object data, WaterfallStepContext stepContext)
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
            return InternalGenerateActivity(templateName, data, stepContext.Context.Activity.Locale);

        }

        public Activity GenerateActivity(string templateName, object data, ITurnContext turnContext)
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
            return InternalGenerateActivity(templateName, data, turnContext.Activity.Locale);
        }

        public Activity GenerateActivity(string templateName, WaterfallStepContext stepContext)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (stepContext == null || stepContext.Context == null || stepContext.Context.Activity == null)
            {
                throw new ArgumentNullException(nameof(stepContext));
            }
            return InternalGenerateActivity(templateName, null, stepContext.Context.Activity.Locale);
        }

        public Activity GenerateActivity(string templateName, TurnContext turnContext)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (turnContext == null || turnContext.Activity == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }
            return InternalGenerateActivity(templateName, null, turnContext.Activity.Locale);
        }

        private Activity InternalGenerateActivity(string templateName, object data, string locale)
        {
            var iLocale = locale == null ? "" : locale;

            if (TemplateEnginesPerLocale.ContainsKey(iLocale))
            {
                return ActivityFactory.CreateActivity(TemplateEnginesPerLocale[locale].EvaluateTemplate(templateName, data).ToString());
            }
            var locales = new string[] { string.Empty };
            if (!LangFallBackPolicy.TryGetValue(iLocale, out locales))
            {
                if (!LangFallBackPolicy.TryGetValue(string.Empty, out locales))
                {
                    throw new Exception($"No supported language found for {iLocale}");
                }
            }

            foreach (var fallBackLocale in locales)
            {
                if (TemplateEnginesPerLocale.ContainsKey(fallBackLocale))
                {
                    return ActivityFactory.CreateActivity(TemplateEnginesPerLocale[fallBackLocale].EvaluateTemplate(templateName, data).ToString());
                }
            }
            return new Activity();
        }
    }
}
