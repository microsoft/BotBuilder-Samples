// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.AI.OpenAI;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
    /// </summary>
    public class AzureOpenAIClient : ICompletion
    {
        private readonly global::Azure.AI.OpenAI.OpenAIClient openAIClient;
        private readonly string deploymentId;
        // Use your own storage to store the conversation history
        private static readonly Dictionary<string, List<string>> conversationHistory = new();

        public AzureOpenAIClient(string apiKey, string endpoint, string deploymentId)
        {
            openAIClient = new(new Uri(endpoint), new AzureKeyCredential(apiKey));
            this.deploymentId = deploymentId;
        }

        public async Task<string> GenerateCompletionAsync(ITurnContext<IMessageActivity> turnContext)
        {
            try
            {
                var completionsOptions = new CompletionsOptions();
                var chatHistory = RefreshConversationHistory(turnContext, turnContext.Activity.Text, "user");
                completionsOptions.Prompt.Add($"{string.Join("\\n", chatHistory)}\\n<|im_start|>assistant");
                completionsOptions.Stop.Add("<|im_end|>");
                completionsOptions.MaxTokens = 2048;

                var completionsResponse = await openAIClient.GetCompletionsAsync(deploymentId, completionsOptions);
                var completion = completionsResponse?.Value?.Choices[0]?.Text ?? "no result";
                
                RefreshConversationHistory(turnContext, completion, "assistant");
                return completion;
            }
            catch(Exception e)
            {
                return e.Message;
            }
          
        }

        private List<string> RefreshConversationHistory(ITurnContext<IMessageActivity> turnContext, string message, string role)
        {
            var conversationId = turnContext.Activity.Conversation.Id;

            // https://learn.microsoft.com/en-us/azure/cognitive-services/openai/chatgpt-quickstart?tabs=command-line&pivots=rest-api#understanding-the-prompt-structure
            var currentMessage = $"<|im_start|>{role}\\n{message}\\n<|im_end|>";
            if (conversationHistory.TryGetValue(conversationId, out var history))
            {
                history.Add(currentMessage);
            }
            else
            {
                history = new List<string> { currentMessage };
                conversationHistory.Add(conversationId, history);
            }

            return history;
        }
    }
}
