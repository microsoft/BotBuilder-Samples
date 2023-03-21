// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
    /// </summary>
    public class AzureOpenAIClient : ICompletion
    {
        private readonly global::Azure.AI.OpenAI.OpenAIClient openAIClient;
        private readonly string deploymentId;

        public AzureOpenAIClient(string apiKey, string endpoint, string deploymentId)
        {
            openAIClient = new(new Uri(endpoint), new AzureKeyCredential(apiKey));
            this.deploymentId = deploymentId;
        }

        public async Task<string> GenerateCompletionAsync(IEnumerable<ChatMessage> history)
        {
            try
            {
                var completionsOptions = new CompletionsOptions();
                completionsOptions.Prompt.Add(BuildMessage(history));
                completionsOptions.Stop.Add("<|im_end|>");
                completionsOptions.MaxTokens = 2048;

                var completionsResponse = await openAIClient.GetCompletionsAsync(deploymentId, completionsOptions);
                return completionsResponse?.Value?.Choices[0]?.Text ?? string.Empty;
            }
            catch(Exception e)
            {
                return e.Message;
            }
          
        }

        private static string BuildMessage(IEnumerable<ChatMessage> messages)
        {
            // https://learn.microsoft.com/en-us/azure/cognitive-services/openai/chatgpt-quickstart?tabs=command-line&pivots=rest-api#understanding-the-prompt-structure
            var message = new StringBuilder();
            foreach (var chatMessage in messages)
            {
                message.Append($"<|im_start|>{chatMessage.Role}\\n{chatMessage.Content}\\n<|im_end|>\\n");
            }
            message.Append("<|im_start|>assistant\\n");
            return message.ToString();
        }
    }
}
