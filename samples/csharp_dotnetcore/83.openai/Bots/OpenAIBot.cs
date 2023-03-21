// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class OpenAIBot : ActivityHandler
    {
        private readonly IConfiguration configuration;
        private readonly ITranscriptStore store;
        public OpenAIBot(IConfiguration configuration, ITranscriptStore store)
        {
            this.configuration = configuration;
            this.store = store;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var chatHistory = await GetHistoryAsync(turnContext.Activity);
        
            var resolver = GetCompletionResolver();
            var result = await resolver.GenerateCompletionAsync(chatHistory);
            await turnContext.SendActivityAsync(MessageFactory.Text(result, result), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome! Please say anything as the openai prompt.";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private ICompletion GetCompletionResolver()
        {
            /*
            // OpenAI
            var openAIKey = configuration.GetValue<string>("OpenAIKey");
            if (string.IsNullOrEmpty(openAIKey))
            {
                throw new Exception("Please fill in the OpenAIKey in appsettings to use the OpenAI service.");
            }

            return new OpenAIClient(configuration.GetValue<string>("OpenAIKey"));
            */
            // Azure OpenAI
            
            var azureOpenAIKey = configuration.GetValue<string>("AzureOpenAIKey");
            var azureOpenAIEndpoint = configuration.GetValue<string>("AzureOpenAIEndpoint");
            var azureOpenAIDeploymentId = configuration.GetValue<string>("AzureOpenAIDeploymentId");
            if (string.IsNullOrEmpty(azureOpenAIKey)
                || string.IsNullOrEmpty(azureOpenAIEndpoint)
                || string.IsNullOrEmpty(azureOpenAIDeploymentId))
            {
                throw new Exception("Please fill in the properties start with Azure in appsettings to use the AzureOpenAI service.");
            }
            return new AzureOpenAIClient(azureOpenAIKey, azureOpenAIEndpoint, azureOpenAIDeploymentId);
            
        }

        private async Task<IEnumerable<ChatMessage>> GetHistoryAsync(IMessageActivity current)
        {
            var pagedTranscript = await store.GetTranscriptActivitiesAsync(current.ChannelId, current.Conversation.Id);
            var chatMessages = pagedTranscript.Items
                            .Where(a => a.Type == ActivityTypes.Message)
                            .Append(current)
                            .Select(ia => (Activity)ia)
                            .Select(ia => ConvertToChatMessage(ia));

            return chatMessages;
        }

        private static ChatMessage ConvertToChatMessage(IMessageActivity activity)
        {
            var role = ActivityRoleToChatMessageRole(activity.From.Role);
            return new ChatMessage(role, activity.Text);
        }

        private static string ActivityRoleToChatMessageRole(string role) => role switch
        {
            "user" => StaticValues.ChatMessageRoles.User,
            "bot" => StaticValues.ChatMessageRoles.Assistant,
            _ => StaticValues.ChatMessageRoles.System,
        };
    }
}
