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
    /// Represents a bot that can process incoming activities.
    /// For each interaction from the user, an instance of this class is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single Turn, should be carefully managed.
    /// </summary>
    public class NlpDispatchBot : IBot
    {
        /// <summary>
        /// Key in the Bot config (.bot file) for the Home Automation Luis instance.
        /// </summary>
        public static readonly string HomeAutomationLuisKey = "homeautomation.luis";

        /// <summary>
        /// Key in the Bot config (.bot file) for the Weather Luis instance.
        /// </summary>
        public static readonly string WeatherLuisKey = "weather.luis";

        /// <summary>
        /// Key in the Bot config (.bot file) for the Dispatch.
        /// </summary>
        public static readonly string DispatchKey = "bot-dispatch";

        /// <summary>
        /// Key in the Bot config (.bot file) for the QnaMaker instance.
        /// In the .bot file, multiple instances of QnaMaker can be configured.
        /// </summary>
        public static readonly string QnAMakerKey = "sample.qna";

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
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a QnA service named '{DispatchKey}'.");
            }

            if (!_services.LuisServices.ContainsKey(HomeAutomationLuisKey))
            {
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a Luis service named '{HomeAutomationLuisKey}'.");
            }

            if (!_services.LuisServices.ContainsKey(WeatherLuisKey))
            {
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a Luis service named '{WeatherLuisKey}'.");
            }
        }

        /// <summary>
        /// Every Conversation turn for our NLP Dispatch Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response, with no stateful conversation.
        /// </summary>
        /// <param name="context">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message && !context.Responded)
            {
                // Get the intent recognition result
                var recognizerResult = await _services.LuisServices[DispatchKey].RecognizeAsync(context, cancellationToken);
                var topIntent = recognizerResult?.GetTopScoringIntent();

                if (topIntent == null)
                {
                    await context.SendActivityAsync("Unable to get the top intent.");
                }
                else
                {
                    if (topIntent.Value.score < 0.3)
                    {
                        await context.SendActivityAsync("I'm not very sure what you want but will try to send your request.");
                    }

                    await DispatchToTopIntentAsync(context, topIntent);
                }
            }
        }

        /// <summary>
        /// Depending on the intent from Dispatch, routes to the right Luis model or Qna service.
        /// </summary>
        private async Task DispatchToTopIntentAsync(ITurnContext context, (string intent, double score)? topIntent)
        {
            const string homeAutomationDispatchKey = "l_homeautomation-LUIS";
            const string weatherDispatchKey = "l_weather-LUIS";
            const string noneDispatchKey = "None";
            const string qnaDispatchKey = "q_sample_qna";

            switch (topIntent.Value.intent.ToLowerInvariant())
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
        /// Dispatches the turn to the request QnaMaker app.
        /// </summary>
        private async Task DispatchToQnAMakerAsync(ITurnContext context, string appName)
        {
            if (!string.IsNullOrEmpty(context.Activity.Text))
            {
                var results = await _services.QnAServices[appName].GetAnswersAsync(context).ConfigureAwait(false);
                if (results.Any())
                {
                    await context.SendActivityAsync(results.First().Answer);
                }
                else
                {
                    await context.SendActivityAsync($"Couldn't find an answer in the {appName}.");
                }
            }
        }

        /// <summary>
        /// Dispatches the turn to the requested Luis model.
        /// </summary>
        private async Task DispatchToLuisModelAsync(ITurnContext context, string appName)
        {
            await context.SendActivityAsync($"Sending your request to the {appName} system ...");
            var result = await _services.LuisServices[appName].RecognizeAsync(context, CancellationToken.None);

            await context.SendActivityAsync($"Intents detected by the {appName} app:\n\n{string.Join("\n\n", result.Intents)}");

            if (result.Entities.Count > 0)
            {
                await context.SendActivityAsync($"The following entities were found in the message:\n\n{string.Join("\n\n", result.Entities)}");
            }
        }
    }
}
