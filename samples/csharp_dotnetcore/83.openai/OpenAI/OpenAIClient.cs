// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    // https://github.com/betalgo/openai
    // https://platform.openai.com/docs/libraries
    // nodejs version: https://www.npmjs.com/package/openai
    /// </summary>
    public class OpenAIClient : ICompletion
    {
        private readonly OpenAIService openAIService;

        public OpenAIClient(string apiKey, string? organization = null)
        {
            var options = new OpenAiOptions()
            {
                ApiKey = apiKey,
                Organization = organization
            };
            openAIService = new OpenAIService(options);
        }

        public async Task<string> GenerateCompletionAsync(string prompt)
        {
            var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromUser(prompt)
                },
                MaxTokens = 2048,
                // Change the mode here
                Model = Models.ChatGpt3_5Turbo0301
            });

            if (completionResult.Successful && completionResult.Choices.Count > 0)
            {
                return completionResult.Choices.First()?.Message?.Content ?? "no result";
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }

                return $"{completionResult.Error.Code}: {completionResult.Error.Message}";
            }
        }
    }
}
