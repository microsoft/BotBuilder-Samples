using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RemoteDialog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemoteDialog.Helpers
{
    public class QnAServiceHelper
    {
        private IConfiguration _configuration;
        private QnAMakerEndpoint _endpoint;
        private QnAMakerOptions _options;
        private HttpClient _httpClient;

        public QnAServiceHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._httpClient = new HttpClient();
            this.InitQnAService();
        }

        public async Task<QnAResult[]> QueryQnAService(string query, QnABotState qnAcontext)
        {
            var requestUrl = $"{this._endpoint.Host}/knowledgebases/{this._endpoint.KnowledgeBaseId}/generateanswer";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var jsonRequest = JsonConvert.SerializeObject(
                new
                {
                    question = query,
                    top = this._options.Top,
                    context = qnAcontext,
                    strictFilters = this._options.StrictFilters,
                    metadataBoost = this._options.MetadataBoost,
                    scoreThreshold = this._options.ScoreThreshold,
                }, Formatting.None);

            request.Headers.Add("Authorization", $"EndpointKey {this._endpoint.EndpointKey}");
            request.Content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            var response = await this._httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var contentString = response.Content.ReadAsStringAsync().Result;
            
            var result = JsonConvert.DeserializeObject<QnAResultList>(contentString);

            return result.Answers;
        }

        private void InitQnAService()
        {
            this._options = new QnAMakerOptions
            {
                Top = 3
            };

            var hostname = this._configuration["QnAEndpointHostName"];
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (!hostname.EndsWith("/qnamaker"))
            {
                hostname = string.Concat(hostname, "/qnamaker");
            }

            this._endpoint = new QnAMakerEndpoint
            {
                KnowledgeBaseId = this._configuration["QnAKnowledgebaseId"],
                EndpointKey = this._configuration["QnAAuthKey"],
                Host = hostname
            };
        }
    }
}
