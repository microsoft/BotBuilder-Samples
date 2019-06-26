// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QnAPrompting.Models;

namespace QnAPrompting.Helpers
{
    public class QnAService : IQnAService
    {
        private readonly HttpClient _httpClient;
        private readonly QnAMakerEndpoint _endpoint;
        private readonly QnAMakerOptions _options;

        public QnAService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            (_options, _endpoint) = InitQnAService(configuration);
        }

        public async Task<QnAResult[]> QueryQnAServiceAsync(string query, QnABotState qnAcontext)
        {
            var requestUrl = $"{_endpoint.Host}/knowledgebases/{_endpoint.KnowledgeBaseId}/generateanswer";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var jsonRequest = JsonConvert.SerializeObject(
                new
                {
                    question = query,
                    top = _options.Top,
                    context = qnAcontext,
                    strictFilters = _options.StrictFilters,
                    metadataBoost = _options.MetadataBoost,
                    scoreThreshold = _options.ScoreThreshold,
                }, Formatting.None);

            request.Headers.Add("Authorization", $"EndpointKey {_endpoint.EndpointKey}");
            request.Content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();


            var contentString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<QnAResultList>(contentString);

            return result.Answers;
        }

        private static (QnAMakerOptions options, QnAMakerEndpoint endpoint) InitQnAService(IConfiguration configuration)
        {
            var options = new QnAMakerOptions
            {
                Top = 3
            };

            var hostname = configuration["QnAEndpointHostName"];
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (!hostname.EndsWith("/qnamaker"))
            {
                hostname = string.Concat(hostname, "/qnamaker");
            }

            var endpoint = new QnAMakerEndpoint
            {
                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                EndpointKey = configuration["QnAEndpointKey"],
                Host = hostname
            };

            return (options, endpoint);
        }
    }
}
