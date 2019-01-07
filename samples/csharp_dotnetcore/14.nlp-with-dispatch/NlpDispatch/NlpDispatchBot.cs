// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;

namespace NLP_With_Dispatch_Bot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each interaction from the user, an instance of this class is called.
<<<<<<< HEAD
    /// This is a Transient lifetime service.  Transient lifetime services are created
=======
    /// This is a Transient lifetime service. Transient lifetime services are created
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single Turn, should be carefully managed.
    /// </summary>
    public class NlpDispatchBot : IBot
    {
        private const string WelcomeText = "This bot will introduce you to Dispatch for QnA Maker and LUIS. Type a greeting, or a question about the weather to get started";

        /// <summary>
        /// Key in the Bot config (.bot file) for the Home Automation Luis instance.
        /// </summary>
<<<<<<< HEAD
        public static readonly string HomeAutomationLuisKey = "Home Automation";
=======
        private const string HomeAutomationLuisKey = "Home Automation";
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

        /// <summary>
        /// Key in the Bot config (.bot file) for the Weather Luis instance.
        /// </summary>
<<<<<<< HEAD
        public static readonly string WeatherLuisKey = "Weather";
=======
        private const string WeatherLuisKey = "Weather";
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

        /// <summary>
        /// Key in the Bot config (.bot file) for the Dispatch.
        /// </summary>
<<<<<<< HEAD
        public static readonly string DispatchKey = "nlp-with-dispatchDispatch";
=======
        private const string DispatchKey = "nlp-with-dispatchDispatch";
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

        /// <summary>
        /// Key in the Bot config (.bot file) for the QnaMaker instance.
        /// In the .bot file, multiple instances of QnaMaker can be configured.
        /// </summary>
<<<<<<< HEAD
        public static readonly string QnAMakerKey = "sample-qna";
=======
        private const string QnAMakerKey = "sample-qna";
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

        /// <summary>
        /// Services configured from the ".bot" file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="NlpDispatchBot"/> class.
        /// </summary>
        /// <param name="services">Services configured from the ".bot" file.</param>
        public NlpDispatchBot(BotServices services)
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));

            if (!_services.QnAServices.ContainsKey(QnAMakerKey))
            {
<<<<<<< HEAD
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a QnA service named '{DispatchKey}'.");
=======
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a QnA service named '{DispatchKey}'.");
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            }

            if (!_services.LuisServices.ContainsKey(HomeAutomationLuisKey))
            {
<<<<<<< HEAD
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a Luis service named '{HomeAutomationLuisKey}'.");
=======
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a Luis service named '{HomeAutomationLuisKey}'.");
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            }

            if (!_services.LuisServices.ContainsKey(WeatherLuisKey))
            {
<<<<<<< HEAD
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a Luis service named '{WeatherLuisKey}'.");
=======
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a Luis service named '{WeatherLuisKey}'.");
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            }
        }

        /// <summary>
        /// Every conversation turn for our NLP Dispatch Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response, with no stateful conversation.
        /// </summary>
<<<<<<< HEAD
        /// <param name="context">A <see cref="ITurnContext"/> containing all the data needed
=======
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
<<<<<<< HEAD
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message && !context.Responded)
            {
                // Get the intent recognition result
                var recognizerResult = await _services.LuisServices[DispatchKey].RecognizeAsync(context, cancellationToken);
=======
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message && !turnContext.Responded)
            {
                // Get the intent recognition result
                var recognizerResult = await _services.LuisServices[DispatchKey].RecognizeAsync(turnContext, cancellationToken);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
                var topIntent = recognizerResult?.GetTopScoringIntent();

                if (topIntent == null)
                {
<<<<<<< HEAD
                    await context.SendActivityAsync("Unable to get the top intent.");
                }
                else
                {
                    await DispatchToTopIntentAsync(context, topIntent, cancellationToken);
                }
            }
            else if (context.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Send a welcome message to the user and tell them what actions they may perform to use this bot
                await SendWelcomeMessageAsync(context, cancellationToken);
            }
            else
            {
                await context.SendActivityAsync($"{context.Activity.Type} event detected", cancellationToken: cancellationToken);
=======
                    await turnContext.SendActivityAsync("Unable to get the top intent.");
                }
                else
                {
                    await DispatchToTopIntentAsync(turnContext, topIntent, cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Send a welcome message to the user and tell them what actions they may perform to use this bot
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to Dispatch bot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            }
        }

        /// <summary>
        /// Depending on the intent from Dispatch, routes to the right LUIS model or QnA service.
        /// </summary>
        private async Task DispatchToTopIntentAsync(ITurnContext context, (string intent, double score)? topIntent, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string homeAutomationDispatchKey = "l_Home_Automation";
            const string weatherDispatchKey = "l_Weather";
            const string noneDispatchKey = "None";
            const string qnaDispatchKey = "q_sample-qna";

            switch (topIntent.Value.intent)
            {
                case homeAutomationDispatchKey:
                    await DispatchToLuisModelAsync(context, HomeAutomationLuisKey);

                    // Here, you can add code for calling the hypothetical home automation service, passing in any entity information that you need
                    break;
                case weatherDispatchKey:
                    await DispatchToLuisModelAsync(context, WeatherLuisKey);

                    // Here, you can add code for calling the hypothetical weather service,
                    // passing in any entity information that you need
                    break;
                case noneDispatchKey:
                    // You can provide logic here to handle the known None intent (none of the above).
                    // In this example we fall through to the QnA intent.
                case qnaDispatchKey:
                    await DispatchToQnAMakerAsync(context, QnAMakerKey);
                    break;

                default:
                    // The intent didn't match any case, so just display the recognition results.
                    await context.SendActivityAsync($"Dispatch intent: {topIntent.Value.intent} ({topIntent.Value.score}).");
                    break;
            }
        }

        /// <summary>
        /// Dispatches the turn to the request QnAMaker app.
        /// </summary>
        private async Task DispatchToQnAMakerAsync(ITurnContext context, string appName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(context.Activity.Text))
            {
<<<<<<< HEAD
                var results = await _services.QnAServices[appName].GetAnswersAsync(context).ConfigureAwait(false);
=======
                var results = await _services.QnAServices[appName].GetAnswersAsync(context);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
                if (results.Any())
                {
                    await context.SendActivityAsync(results.First().Answer, cancellationToken: cancellationToken);
                }
                else
                {
                    await context.SendActivityAsync($"Couldn't find an answer in the {appName}.");
                }
            }
        }

        /// <summary>
        /// Dispatches the turn to the requested LUIS model.
        /// </summary>
        private async Task DispatchToLuisModelAsync(ITurnContext context, string appName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await context.SendActivityAsync($"Sending your request to the {appName} system ...");
            var result = await _services.LuisServices[appName].RecognizeAsync(context, cancellationToken);

            await context.SendActivityAsync($"Intents detected by the {appName} app:\n\n{string.Join("\n\n", result.Intents)}");

            if (result.Entities.Count > 0)
            {
                await context.SendActivityAsync($"The following entities were found in the message:\n\n{string.Join("\n\n", result.Entities)}");
            }
        }
<<<<<<< HEAD

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to Dispatch bot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
            }
        }
=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
}
