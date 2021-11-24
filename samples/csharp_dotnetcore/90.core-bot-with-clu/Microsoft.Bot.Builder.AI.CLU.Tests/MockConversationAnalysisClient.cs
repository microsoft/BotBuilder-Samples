using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Language.Conversations;
using Moq;

namespace Microsoft.Bot.Builder.AI.CLU.Tests
{
    public class MockConversationAnalysisClient : ConversationAnalysisClient
    {
        public ConversationAnalysisClientOptions conversationAnalysisClientOptions;
        public AnalyzeConversationOptions analyzeConversationOptions;
        public MockConversationAnalysisClient() : base()
        {
        }
        public MockConversationAnalysisClient(Uri endpoint, AzureKeyCredential credential, ConversationAnalysisClientOptions options) : base()
        {
            conversationAnalysisClientOptions = options; 
        }
        public override Task<Response<AnalyzeConversationResult>> AnalyzeConversationAsync(AnalyzeConversationOptions options, CancellationToken cancellationToken = default)
        {
            analyzeConversationOptions = options;
            return Task.FromResult(Mock.Of<Response<AnalyzeConversationResult>>());
        }
    }
}
