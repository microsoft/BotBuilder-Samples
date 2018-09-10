// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiLingualBot.Translation
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class TranslationMiddleware : IMiddleware
    {
        private readonly ITranslator _translator;
        private readonly IStatePropertyAccessor<string> _languageStateProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMiddleware"/> class.
        /// </summary>
        /// <param name="translator">Translator implementation to be used for text translation.</param>
        /// <param name="languageStateProperty">State property for current language.</param>
        public TranslationMiddleware(ITranslator translator, IStatePropertyAccessor<string> languageStateProperty)
        {
            _translator = translator;
            _languageStateProperty = languageStateProperty;
        }

        /// <summary>
        /// Processess an incoming activity.
        /// </summary>
        /// <param name="context">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            var translate = await ShouldTranslateAsync(turnContext);

            if (translate)
            {
                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    turnContext.Activity.Text = await _translator.TranslateAsync(turnContext.Activity.Text, TranslationSettings.DefaultLanguage);
                }
            }

            turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
            {
                string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;
                bool shouldTranslate = userLanguage != TranslationSettings.DefaultLanguage;

                // Translate messages sent to the user to user language
                if (shouldTranslate)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                    {
                        tasks.Add(TranslateMessageActivityAsync(currentActivity.AsMessageActivity(), userLanguage));
                    }

                    if (tasks.Any())
                    {
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                    }
                }

                return await nextSend();
            });

            turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
            {
                string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;
                bool shouldTranslate = userLanguage != TranslationSettings.DefaultLanguage;

                // Translate messages sent to the user to user language
                if (activity.Type == ActivityTypes.Message)
                {
                    if (shouldTranslate)
                    {
                        await TranslateMessageActivityAsync(activity.AsMessageActivity(), userLanguage);
                    }
                }

                return await nextUpdate();
            });

            await next(cancellationToken).ConfigureAwait(false);
        }

        private async Task TranslateMessageActivityAsync(IMessageActivity activity, string targetLocale)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                activity.Text = await _translator.TranslateAsync(activity.Text, targetLocale);
            }
        }

        private async Task<bool> ShouldTranslateAsync(ITurnContext turnContext)
        {
            string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;
            return userLanguage != TranslationSettings.DefaultLanguage;
        }
    }
}
