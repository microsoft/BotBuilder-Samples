// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class OpenAIBot : ActivityHandler
    {
        private readonly IConfiguration config;
        public OpenAIBot(IConfiguration config)
        {
            this.config = config;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var resolvers = GetCompletionResolvers();
            foreach (var resolver in resolvers)
            {
                var result = await resolver.GenerateCompletionAsync(turnContext.Activity.Text);
                var replyText = $"result from {resolver.GetType()}:\r\n{result}";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
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

        private List<ICompletion> GetCompletionResolvers() => new()
        {
            // OpenAI
            new OpenAIClient(config.GetValue<string>("OpenAIKey")),

            // Azure OpenAI
            new AzureOpenAIClient(
                config.GetValue<string>("AzureOpenAIKey"),
                config.GetValue<string>("AzureOpenAIEndpoint"),
                config.GetValue<string>("AzureOpenAIDeploymentId"))
        };
    }
}
