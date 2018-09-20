// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using MultiLingualBot.Translation;

namespace MultiLingualBot
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class MultiLingualBot : IBot
    {
        private readonly MultiLingualBotAccessors _accessors;

        private DialogSet Dialogs { get; set; }

        private const string English = "en";
        private const string Spanish = "es";

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLingualBot"/> class.
        /// </summary>
        /// <param name="accessors">Bot State Accessors.</param>
        public MultiLingualBot(MultiLingualBotAccessors accessors)
        {
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
        }

        /// <summary>
        /// Run every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string userLanguage = await _accessors.LanguagePreference.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

                bool translate = userLanguage != TranslationSettings.DefaultLanguage;

                if (IsLanguageChangeRequested(turnContext.Activity.Text))
                {
                    // If the user requested a language change through the suggested actions with values "es" or "en",
                    // simply change the user's language preference in the user state.
                    // The translation middleware will catch this setting and translate both ways to the user's
                    // selected language.
                    // If Spanish was selected by the user, the reply below will actually be shown in spanish to the user.
                    await _accessors.LanguagePreference.SetAsync(turnContext, turnContext.Activity.Text);
                    var reply = turnContext.Activity.CreateReply($"Your current language code is: {turnContext.Activity.Text}");

                    await turnContext.SendActivityAsync(reply, cancellationToken);

                    // Save the user profile updates into the user state.
                    await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
                }
                else
                {
                    // Show the user the possible options for language. If the user chooses a different language
                    // than the default, then the translation middleware will pick it up from the user state and
                    // translate messages both ways, i.e. user to bot and bot to user.
                    var reply = turnContext.Activity.CreateReply("Choose your language:");
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "Español", Type = ActionTypes.PostBack, Value = Spanish },
                            new CardAction() { Title = "English", Type = ActionTypes.PostBack, Value = English },
                        },
                    };

                    await turnContext.SendActivityAsync(reply);
                }
            }
        }

        private static bool IsLanguageChangeRequested(string utterance)
        {
            if (string.IsNullOrEmpty(utterance))
            {
                return false;
            }

            utterance = utterance.ToLower().Trim();
            return utterance == Spanish || utterance == English;
        }
    }
}