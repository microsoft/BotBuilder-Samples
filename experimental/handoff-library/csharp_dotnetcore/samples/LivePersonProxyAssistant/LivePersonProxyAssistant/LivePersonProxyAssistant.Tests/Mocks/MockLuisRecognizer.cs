// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;

namespace LivePersonProxyAssistant.Tests.Mocks
{
    public class MockLuisRecognizer : LuisRecognizer
    {
        private static LuisApplication mockApplication = new LuisApplication()
        {
            ApplicationId = "testappid",
            Endpoint = "testendpoint",
            EndpointKey = "testendpointkey"
        };

        public MockLuisRecognizer(IRecognizerConvert defaultIntent)
            : base(new LuisRecognizerOptionsV3(mockApplication))
        {
            TestUtterances = new Dictionary<string, IRecognizerConvert>();
            DefaultIntent = defaultIntent;
        }

        private Dictionary<string, IRecognizerConvert> TestUtterances { get; set; }

        private IRecognizerConvert DefaultIntent { get; set; }

        public void RegisterUtterances(Dictionary<string, IRecognizerConvert> utterances)
        {
            foreach (var utterance in utterances)
            {
                TestUtterances.Add(utterance.Key, utterance.Value);
            }
        }

        public override Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text;

            var mockResult = TestUtterances.GetValueOrDefault(text, DefaultIntent);
            return Task.FromResult((T)mockResult);
        }
    }
}