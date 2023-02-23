// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class OpenAIBot : ActivityHandler
    {
        private readonly IConfiguration configuration;
        public OpenAIBot(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var resolvers = GetCompletionResolvers();
            if (resolvers.Count == 0)
            {
                var text = "Please fill in at least one resolver property in appsettings";
                await turnContext.SendActivityAsync(MessageFactory.Text(text, text), cancellationToken);
            }
            else
            {
                foreach (var resolver in resolvers)
                {
                    var result = await resolver.GenerateCompletionAsync(turnContext.Activity.Text);
                    var replyText = $"result from {resolver.GetType().Name}:\r\n{result}";
                    await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
                }
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

        private List<ICompletion> GetCompletionResolvers()
        {
            var resolvers = new List<ICompletion>();

            // OpenAI
            var openAIKey = configuration.GetValue<string>("OpenAIKey");
            if (!string.IsNullOrEmpty(openAIKey))
            {
                resolvers.Add(new OpenAIClient(configuration.GetValue<string>("OpenAIKey")));
            }

            // Azure OpenAI
            var azureOpenAIKey = configuration.GetValue<string>("AzureOpenAIKey");
            var azureOpenAIEndpoint = configuration.GetValue<string>("AzureOpenAIEndpoint");
            var azureOpenAIDeploymentId = configuration.GetValue<string>("AzureOpenAIDeploymentId");
            if (!string.IsNullOrEmpty(azureOpenAIKey)
                && !string.IsNullOrEmpty(azureOpenAIEndpoint)
                && !string.IsNullOrEmpty(azureOpenAIDeploymentId))
            {
                resolvers.Add(new AzureOpenAIClient(azureOpenAIKey, azureOpenAIEndpoint, azureOpenAIDeploymentId));
            }

            return resolvers;
        }
    }
}
