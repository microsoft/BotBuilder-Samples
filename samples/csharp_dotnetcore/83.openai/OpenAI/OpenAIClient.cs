// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
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

        // Use your own storage to store the conversation history
        private static readonly Dictionary<string, List<ChatMessage>> conversationHistory = new();

        public OpenAIClient(string apiKey, string? organization = null)
        {
            var options = new OpenAiOptions()
            {
                ApiKey = apiKey,
                Organization = organization
            };

            openAIService = new OpenAIService(options);
        }

        public async Task<string> GenerateCompletionAsync(ITurnContext<IMessageActivity> turnContext)
        {
            var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
            {
                Messages = RefreshConversationHistory(turnContext, ChatMessage.FromUser(turnContext.Activity.Text)),
                MaxTokens = 2048,
                // Change the mode here
                Model = Models.ChatGpt3_5Turbo0301
            });

            if (completionResult.Successful && completionResult.Choices.Count > 0)
            {
                var content = completionResult.Choices.First()?.Message?.Content;
                if (!string.IsNullOrEmpty(content))
                {
                    RefreshConversationHistory(turnContext, ChatMessage.FromAssistance(content));
                }

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

        private List<ChatMessage> RefreshConversationHistory(ITurnContext<IMessageActivity> turnContext, ChatMessage message)
        {
            var conversationId = turnContext.Activity.Conversation.Id;

    
            if (conversationHistory.TryGetValue(conversationId, out var history))
            {
                history.Add(message);
            }
            else
            {
                history = new List<ChatMessage> { message };
                conversationHistory.Add(conversationId, history);
            }

            return history;
        }
    }
}
