using System;
using System.IO;
using System.Net.Http;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Solutions;
using Microsoft.Bot.Solutions.Responses;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using LivePersonProxyAssistant.Dialogs;

namespace LivePersonProxyAssistant.Tests.Mocks
{
    public class MockMainDialog : MainDialog
    {
        private MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler();
        private LocaleTemplateManager _templateManager;

        public MockMainDialog(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            // All calls to Generate Answer regardless of host or knowledgebaseId are captured
            _mockHttpHandler.When(HttpMethod.Post, "*/knowledgebases/*/generateanswer")
              .Respond("application/json", GetResponse("QnAMaker_NoAnswer.json"));

            _templateManager = serviceProvider.GetService<LocaleTemplateManager>();
        }

        protected override QnAMakerDialog TryCreateQnADialog(string knowledgebaseId, CognitiveModelSet cognitiveModels)
        {
            if (!cognitiveModels.QnAConfiguration.TryGetValue(knowledgebaseId, out QnAMakerEndpoint qnaEndpoint)
                          || qnaEndpoint == null)
            {
                throw new Exception($"Could not find QnA Maker knowledge base configuration with id: {knowledgebaseId}.");
            }

            // QnAMaker dialog already present on the stack?
            if (Dialogs.Find(knowledgebaseId) == null)
            {
                // Return a QnAMaker dialog using our Http Mock
                return new QnAMakerDialog(
                    knowledgeBaseId: qnaEndpoint.KnowledgeBaseId,
                    endpointKey: qnaEndpoint.EndpointKey,
                    hostName: qnaEndpoint.Host,
                    noAnswer: _templateManager.GenerateActivityForLocale("UnsupportedMessage"),
                    httpClient: new HttpClient(_mockHttpHandler))
                {
                    Id = knowledgebaseId
                };
            }
            else
            {
                return null;
            }
        }

        private Stream GetResponse(string fileName)
        {
            var path = GetFilePath(fileName);
            return File.OpenRead(path);
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.CurrentDirectory, "TestData", fileName);
        }
    }
}
