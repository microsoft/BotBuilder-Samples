// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Bot.Builder.AI.CLU.Tests
{
    public class CluUtilTests
    {
        private const string inputObject = @"{
            'query': 'book me a flight from Cairo to London',
            'prediction': {
                'intents': [
                    {
                        'category': 'BookFlight',
                        'confidenceScore': 0.99607867
                    },
                    {
                        'category': 'GetWeather',
                        'confidenceScore': 0.0018851843
                    },
                    {
                        'category': 'None',
                        'confidenceScore': 0.0017508692
                    },
                    {
                        'category': 'Cancel',
                        'confidenceScore': 0.0002852016
                    }
                ],
                'entities': [
                    {
                        'category': 'fromCity',
                        'text': 'Cairo',
                        'offset': 22,
                        'length': 5,
                        'confidenceScore': 0.53672844
                    },
                    {
                        'category': 'fromCity',
                        'text': 'London',
                        'offset': 31,
                        'length': 6,
                        'confidenceScore': 0.79554665
                    }
                ],
                'topIntent': 'BookFlight',
                'projectType': 'conversation'
            }
        }";

        private Dictionary<string, IntentScore> expectedOutput = new Dictionary<string, IntentScore>
        {
            { "BookFlight", new IntentScore { Score = 0.99607867 } },
            { "GetWeather", new IntentScore { Score = 0.0018851843 } },
            { "None", new IntentScore { Score = 0.0017508692 } },
            { "Cancel", new IntentScore { Score = 0.0002852016 } }
        };

        [Fact]
        public async Task CluUtilsGetIntents()
        {
            var response = JObject.Parse(inputObject);
            var actualOutput = CluUtil.GetIntents((JObject)response["prediction"]);
            Assert.Equal(JsonConvert.SerializeObject(expectedOutput), JsonConvert.SerializeObject(actualOutput));
            
        }
    }
}
