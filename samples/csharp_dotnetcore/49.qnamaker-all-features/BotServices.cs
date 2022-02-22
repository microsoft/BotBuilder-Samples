// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {
        public BotServices(IConfiguration configuration)
        {
            var qnaServiceType = validateQnAServiceType(configuration["QnAServiceType"]);
            var hostName = GetHostname(configuration["QnAEndpointHostName"], qnaServiceType);
            if (qnaServiceType == Constants.LanguageQnAServiceType)
            {
                QnAMakerService = new CustomQuestionAnswering(new QnAMakerEndpoint
                {
                    KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                    Host = hostName,
                    EndpointKey = GetEndpointKey(configuration),
                    QnAServiceType = qnaServiceType
                });
            }
            else
            {
                QnAMakerService = new QnAMaker(new QnAMakerEndpoint
                {
                    KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                    Host = hostName,
                    EndpointKey = GetEndpointKey(configuration),
                    QnAServiceType = qnaServiceType
                });
            }

        }

        private string validateQnAServiceType(string qnaServiceType)
        {
            return string.Equals(qnaServiceType?.ToLower(), Constants.LanguageQnAServiceType, System.StringComparison.OrdinalIgnoreCase) == true ? Constants.LanguageQnAServiceType : "";
        }

        public IQnAMakerClient QnAMakerService { get; private set; }

        private static string GetHostname(string hostname, string qnaServiceType = "")
        {
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (!string.Equals(qnaServiceType, Constants.LanguageQnAServiceType, System.StringComparison.OrdinalIgnoreCase)
                && !string.Equals(qnaServiceType, "v2", System.StringComparison.OrdinalIgnoreCase)
                && !hostname.Contains("/v5.0") && !hostname.EndsWith("/qnamaker"))
            {
                hostname = string.Concat(hostname, "/qnamaker");
            }

            return hostname;
        }

        private static string GetEndpointKey(IConfiguration configuration)
        {
            var endpointKey = configuration["QnAEndpointKey"];

            if (string.IsNullOrWhiteSpace(endpointKey))
            {
                // This features sample is copied as is for "azure bot service" default "createbot" template.
                // Post this sample change merged into "azure bot service" template repo, "Azure Bot Service"
                // will make the web app config change to use "QnAEndpointKey".But, the the old "QnAAuthkey"
                // required for backward compact. This is a requirement from docs to keep app setting name
                // consistent with "QnAEndpointKey". This is tracked in Github issue:
                // https://github.com/microsoft/BotBuilder-Samples/issues/2532

                endpointKey = configuration["QnAAuthKey"];
            }

            return endpointKey;

        }
    }
}
