// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Ai.LUIS
{
    /// <summary>
    /// A Middleware for running the Luis recognizer.
    /// This could eventually be generalized and moved to the core Bot Builder library
    /// in order to support multiple recognizers.
    /// </summary>
    public class LuisRecognizerMiddleware : IMiddleware
    {
        /// <summary>
        /// The service key to use to retrieve recognition results.
        /// </summary>
        public const string LuisRecognizerResultKey = "LuisRecognizerResult";

        /// <summary>
        /// The value type for a LUIS trace activity.
        /// </summary>
        public const string LuisTraceType = "https://www.luis.ai/schemas/trace";

        /// <summary>
        /// The context label for a LUIS trace activity.
        /// </summary>
        public const string LuisTraceLabel = "Luis Trace";

        /// <summary>
        /// A string used to obfuscate the LUIS subscription key.
        /// </summary>
        public const string Obfuscated = "****";

        private readonly IRecognizer _luisRecognizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuisRecognizerMiddleware"/> class.
        /// </summary>
        /// <param name="luisRecognizer">The LUIS recognizer.</param>
        public LuisRecognizerMiddleware(LuisRecognizer luisRecognizer)
        {
            _luisRecognizer = luisRecognizer;
        }

        /// <summary>
        /// Processess an incoming activity.
        /// </summary>
        /// <param name="context">The context object for this turn.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            BotAssert.ContextNotNull(context);

            if (context.Activity.Type == ActivityTypes.Message)
            {
                var utterance = context.Activity.AsMessageActivity().Text;

                if (!string.IsNullOrWhiteSpace(utterance))
                {
                    var result = await _luisRecognizer.RecognizeAsync(context, CancellationToken.None).ConfigureAwait(false);
                    context.TurnState.Add(LuisRecognizerResultKey, result);

                    var traceActivity = Activity.CreateTraceActivity("LuisRecognizerMiddleware", LuisTraceType);
                    await context.SendActivityAsync(traceActivity).ConfigureAwait(false);
                }
            }

            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
