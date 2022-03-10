// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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
        private readonly ILogger<CustomQABot> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _endpointKey;
        private readonly string _hostname;
        private readonly string _knowledgeBaseId;
        private readonly string _defaultWelcome = "Hello and Welcome";
        private readonly bool _enablePreciseAnswer = true;
        private readonly bool _displayPreciseAnswerOnly = false;

        public CustomQABot(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CustomQABot> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            var welcomeMsg = configuration["DefaultWelcomeMessage"];
            if (!string.IsNullOrWhiteSpace(welcomeMsg))
            {
                _defaultWelcome = welcomeMsg;
            }

            _hostname = configuration["QnAEndpointHostName"];
            if (string.IsNullOrEmpty(_hostname))
            {
                throw new ArgumentException(nameof(_hostname));
            }

            _endpointKey = configuration["QnAEndpointKey"];
            if (string.IsNullOrEmpty(_endpointKey))
            {
                throw new ArgumentException(nameof(_endpointKey));
            }

            _knowledgeBaseId = configuration["QnAKnowledgebaseId"];
            if (string.IsNullOrEmpty(_knowledgeBaseId))
            {
                throw new ArgumentException(nameof(_knowledgeBaseId));
            }

            _enablePreciseAnswer = !bool.TryParse(configuration["EnablePreciseAnswer"], out var enablePreciseAnswer) || enablePreciseAnswer;
            _displayPreciseAnswerOnly = !bool.TryParse(configuration["DisplayPreciseAnswerOnly"], out var displayPreciseAnswerOnly) || displayPreciseAnswerOnly;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            using var httpClient = _httpClientFactory.CreateClient();

            // Custom Question Answering Client initialized with QnAMakerEndpoint.
            var customQuestionAnswering = GetCustomQuestionAnsweringClient(httpClient);

            var options = new QnAMakerOptions { Top = 1, EnablePreciseAnswer = _enablePreciseAnswer };

            // The actual call to the Custom Question Answering service.
            _logger.LogInformation("Calling Custom Question Answering");
            var response = await customQuestionAnswering.GetAnswersAsync(turnContext, options);

            if (response != null && response.Length > 0)
            {
                var activities = new List<Activity>();
                var longAnswer = new Activity() { Text = response[0].Answer, Type = ActivityTypes.Message };
                if (response[0].AnswerSpan?.Text.Length > 0)
                {
                    var shortAnswer = new Activity() { Text = response[0].AnswerSpan?.Text, Type = ActivityTypes.Message };
                    activities.Add(shortAnswer);
                    if (!_displayPreciseAnswerOnly)
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
                await turnContext.SendActivityAsync(MessageFactory.Text("No answers were found."), cancellationToken);
            }
        }

        private CustomQuestionAnswering GetCustomQuestionAnsweringClient(HttpClient httpClient) => new(
            new QnAMakerEndpoint
            {
                KnowledgeBaseId = _knowledgeBaseId,
                EndpointKey = _endpointKey,
                Host = _hostname,
                QnAServiceType = ServiceType.Language
            },
            null,
            httpClient);

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(_defaultWelcome, _defaultWelcome), cancellationToken);
                }
            }
        }
    }
}
