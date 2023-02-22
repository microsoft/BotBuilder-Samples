// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.AI.OpenAI;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
    /// </summary>
    public class AzureOpenAI : ICompletion
    {
        private readonly OpenAIClient openAIClient;
        private readonly string deploymentId;

        public AzureOpenAI(string apiKey, string endpoint, string deploymentId)
        {
            openAIClient = new(new Uri(endpoint), new AzureKeyCredential(apiKey));
            this.deploymentId = deploymentId;
        }

        public async Task<string> GenerateCompletionAsync(string prompt)
        {
            try
            {
                var completionsResponse = await openAIClient.GetCompletionsAsync(deploymentId, prompt);
                var completion = completionsResponse.Value.Choices[0].Text;
                return completion;
            }
            catch
            {
                return "Azure Open AI is not ready.";
            }
            
           
        }
    }
}
