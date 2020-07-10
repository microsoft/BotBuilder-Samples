// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;

namespace TaskModuleFactorySample.Tests.Mocks
{
    public class MockQnAMaker : ITelemetryQnAMaker
    {
        public MockQnAMaker(QueryResult[] defaultAnswer)
        {
            TestAnswers = new Dictionary<string, QueryResult[]>();
            DefaultAnswer = defaultAnswer;
        }

        public bool LogPersonalInformation { get; set; } = false;

        public IBotTelemetryClient TelemetryClient { get; set; } = new NullBotTelemetryClient();

        private Dictionary<string, QueryResult[]> TestAnswers { get; set; }

        private QueryResult[] DefaultAnswer { get; set; }

        public void RegisterAnswers(Dictionary<string, QueryResult[]> utterances)
        {
            foreach (var utterance in utterances)
            {
                TestAnswers.Add(utterance.Key, utterance.Value);
            }
        }

        public Task<QueryResult[]> GetAnswersAsync(ITurnContext context, QnAMakerOptions options = null)
        {
            var text = context.Activity.Text;

            var mockResult = TestAnswers.GetValueOrDefault(text, DefaultAnswer);
            return Task.FromResult(mockResult);
        }

        public Task<QueryResult[]> GetAnswersAsync(ITurnContext turnContext, QnAMakerOptions options, Dictionary<string, string> telemetryProperties, Dictionary<string, double> telemetryMetrics = null)
        {
            var text = turnContext.Activity.Text;

            var mockResult = TestAnswers.GetValueOrDefault(text, DefaultAnswer);
            return Task.FromResult(mockResult);
        }
    }
}