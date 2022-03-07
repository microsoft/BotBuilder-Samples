// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Models;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class CustomQABot : ActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomQABot> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomQABot(IConfiguration configuration, ILogger<CustomQABot> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var customQuestionAnswering = new CustomQuestionAnswering(new QnAMakerEndpoint
            {
                KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                EndpointKey = _configuration["QnAEndpointKey"],
                Host = _configuration["QnAEndpointHostName"],
                QnAServiceType = ServiceType.Language
            },
            null,
            httpClient);

            _logger.LogInformation("Calling QnA Maker");

            bool.TryParse(_configuration["EnablePreciseAnswer"], out var enablePreciseAnswer);
            var options = new QnAMakerOptions { Top = 1, EnablePreciseAnswer = enablePreciseAnswer };

            // The actual call to the QnA Maker service.
            var response = await customQuestionAnswering.GetAnswersAsync(turnContext, options);
            if (response != null && response.Length > 0)
            {
                bool.TryParse(_configuration["DisplayPreciseAnswerOnly"], out var displayPreciseAnswerOnly);

                var longAnswer = new Activity() { Text = response[0].Answer, Type = "message" };
                var shortAnswer = new Activity() { Text = response[0].AnswerSpan?.Text, Type = "message" };
                var activities = new List<Activity>();

                if (enablePreciseAnswer)
                {
                    activities.Add(shortAnswer);
                    if (!displayPreciseAnswerOnly)
                    {
                        activities.Add(longAnswer);
                    }
                }
                else
                {
                    activities.Add(longAnswer);
                }
                await turnContext.SendActivitiesAsync(activities.ToArray(), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
            }
        }
    }
}
