// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {
        public BotServices(IConfiguration configuration)
        {

            var qnaServiceTypeFromConfig = validateQnAServiceType(configuration["QnAServiceType"]);
            var qnaServiceType = Enum.TryParse(qnaServiceTypeFromConfig, true, out ServiceType LanguageService) ? LanguageService : ServiceType.QnAMaker;

            var hostName = GetHostname(configuration["QnAEndpointHostName"], qnaServiceType);
            if (qnaServiceType == ServiceType.Language)
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

        private static string GetHostname(string hostname, ServiceType qnaServiceType = ServiceType.QnAMaker)
        {
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (qnaServiceType == ServiceType.QnAMaker
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
