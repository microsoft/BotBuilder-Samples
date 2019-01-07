// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class TranslationMiddleware : IMiddleware
    {
        private readonly string[] _nativeLanguages;
        private readonly Translator _translator;
        private readonly ConfiguredLanguageDictionary _userConfiguredLanguageDictionary;
        private readonly Dictionary<string, List<string>> _patterns;
        private readonly bool _toUserLanguage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMiddleware"/> class.
        /// </summary>
        /// <param name="nativeLanguages">The languages supported by your app.</param>
        /// <param name="translatorKey">Your subscription key for the Microsoft Translator Text API.</param>
        /// <param name="toUserLanguage">Indicates whether to translate messages sent from the bot into the user's language.</param>
        /// <param name="httpClient">An alternate HTTP client to use.</param>
        /// <param name="defaultLocale">Default locale to use when underlying user locale is undefined.</param>
        public TranslationMiddleware(string[] nativeLanguages, string translatorKey, bool toUserLanguage = false, HttpClient httpClient = null, string defaultLocale = "en")
        {
            if (string.IsNullOrWhiteSpace(defaultLocale))
            {
                throw new ArgumentNullException(nameof(defaultLocale));
            }

            AssertValidNativeLanguages(nativeLanguages);
            this._nativeLanguages = nativeLanguages;
            if (string.IsNullOrEmpty(translatorKey))
            {
                throw new ArgumentNullException(nameof(translatorKey));
            }

            this._translator = new Translator(translatorKey, httpClient);
            _patterns = new Dictionary<string, List<string>>();
            _userConfiguredLanguageDictionary = new ConfiguredLanguageDictionary();
            _toUserLanguage = toUserLanguage;
            DefaultLocale = defaultLocale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMiddleware"/> class.
        /// </summary>
        /// <param name="nativeLanguages">The languages supported by your app.</param>
        /// <param name="translatorKey">Your subscription key for the Microsoft Translator Text API.</param>
        /// <param name="patterns">List of regex patterns, indexed by language identifier,
        /// that can be used to flag text that should not be translated.</param>
        /// /// <param name="userConfiguredLanguageDictionary">Custom languages dictionary object, used to store all the different languages dictionaries
        /// configured by the user to overwrite the translator output to certain vocab by the custom dictionary translation.</param>
        /// <param name="toUserLanguage">Indicates whether to translate messages sent from the bot into the user's language.</param>
        /// <param name="defaultLocale">Default locale to use when underlying user locale is undefined.</param>
        /// <remarks>Each pattern the <paramref name="patterns"/> describes an entity that should not be translated.
        /// For example, in French <c>je m’appelle ([a-z]+)</c>, which will avoid translation of anything coming after je m’appelle.</remarks>
        /// <param name="httpClient">An alternate HTTP client to use.</param>
        public TranslationMiddleware(string[] nativeLanguages, string translatorKey, Dictionary<string, List<string>> patterns, ConfiguredLanguageDictionary userConfiguredLanguageDictionary, bool toUserLanguage = false, HttpClient httpClient = null, string defaultLocale = "en")
            : this(nativeLanguages, translatorKey, toUserLanguage, httpClient, defaultLocale)
        {
            this._translator = new Translator(translatorKey, patterns, userConfiguredLanguageDictionary, httpClient);
        }

        /// <summary>
        /// Gets the default locale to use when underlying user locale is undefined.
        /// </summary>
        /// <value>The default locale that will be used when the underlying user locale is undefined.</value>
        public virtual string DefaultLocale { get; }

        /// <summary>
        /// Processess an incoming activity.
        /// </summary>
        /// <param name="turnContext">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                IMessageActivity message = turnContext.Activity.AsMessageActivity();
                if (message != null)
                {
                    if (!string.IsNullOrWhiteSpace(message.Text))
                    {
                        // determine the language we are using for this conversation
                        var sourceLanguage = string.Empty;
                        var targetLanguage = string.Empty;
                        sourceLanguage = await _translator.DetectAsync(message.Text).ConfigureAwait(false); // awaiting user language detection using Microsoft Translator API.

                        targetLanguage = _nativeLanguages.Contains(sourceLanguage) ? sourceLanguage : _nativeLanguages.FirstOrDefault() ?? "en";
                        TranslateMessageAsync(turnContext, message, sourceLanguage, targetLanguage, _nativeLanguages.Contains(sourceLanguage)).Wait();

                        turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
                        {
                            // Translate messages sent to the user to user language
                            if (_toUserLanguage)
                            {
                                List<Task> tasks = new List<Task>();
                                foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                                {
                                    tasks.Add(TranslateMessageAsync(newContext, currentActivity.AsMessageActivity(), targetLanguage, sourceLanguage, false));
                                }

                                if (tasks.Any())
                                {
                                    await Task.WhenAll(tasks).ConfigureAwait(false);
                                }
                            }

                            return await nextSend().ConfigureAwait(false);
                        });

                        turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
                        {
                            // Translate messages sent to the user to user language
                            if (activity.Type == ActivityTypes.Message)
                            {
                                if (_toUserLanguage)
                                {
                                    await TranslateMessageAsync(newContext, activity.AsMessageActivity(), targetLanguage, sourceLanguage, false).ConfigureAwait(false);
                                }
                            }

                            return await nextUpdate().ConfigureAwait(false);
                        });
                    }
                }
            }

            await next(cancellationToken).ConfigureAwait(false);
        }

        private static void AssertValidNativeLanguages(string[] nativeLanguages)
        {
            if (nativeLanguages == null)
            {
                throw new ArgumentNullException(nameof(nativeLanguages));
            }
        }

        /// <summary>
        /// Translates the <see cref="Activity.Text"/> of a message.
        /// </summary>
        /// <param name="context">The current turn context.</param>
        /// <param name="message">The activity containing the text to translate.</param>
        /// <param name="sourceLanguage">An identifier for the language to translate from.</param>
        /// <param name="targetLanguage">An identifier for the language to translate to.</param>
        /// <param name="inNativeLanguages">should only use native langauges.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>When the task completes successfully, the <see cref="Activity.Text"/> property
        /// of the message contains the translated text.</remarks>
        private async Task TranslateMessageAsync(ITurnContext context, IMessageActivity message, string sourceLanguage, string targetLanguage, bool inNativeLanguages)
        {
            if (!inNativeLanguages && sourceLanguage != targetLanguage)
            {
                // if we have text and a target language
                if (!string.IsNullOrWhiteSpace(message.Text) && !string.IsNullOrEmpty(targetLanguage))
                {
                    if (targetLanguage == sourceLanguage)
                    {
                        return;
                    }

                    var text = message.Text;
                    string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    var translateResult = await this._translator.TranslateArrayAsync(lines, sourceLanguage, targetLanguage).ConfigureAwait(false);
                    string[] translateResultText = new string[translateResult.Count];

                    for (int id = 0; id < translateResult.Count; id++)
                    {
                        translateResultText[id] = translateResult[id].GetTranslatedMessage();
                    }

                    message.Text = string.Join(Environment.NewLine, translateResultText);
                }
            }
        }
    }
}
