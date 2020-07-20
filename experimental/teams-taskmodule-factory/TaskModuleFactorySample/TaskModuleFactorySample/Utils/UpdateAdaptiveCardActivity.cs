using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using ITSMSkill.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using TaskModuleFactorySample.Extensions;

namespace TaskModuleFactorySample.Utils
{
    public class UpdateAdaptiveCardActivity : ITeamsActivity<AdaptiveCard>
    {
        private readonly IConnectorClient _connectorClient;

        public UpdateAdaptiveCardActivity(IConnectorClient connectorClient)
        {
            _connectorClient = connectorClient;
        }

        public async Task<ResourceResponse> UpdateTaskModuleActivityAsync(
            ITurnContext context,
            ActivityReference activityReference,
            AdaptiveCard updateAdaptiveCard,
            CancellationToken cancellationToken)
        {
            Activity reply = context.Activity.CreateReply();
            reply.Attachments = new List<Microsoft.Bot.Schema.Attachment>
            {
                new Microsoft.Bot.Schema.Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = updateAdaptiveCard
                },
            };

            var teamsChannelActivity = reply.CreateConversationToTeamsChannel(
                new TeamsChannelData
                {
                    Channel = new ChannelInfo(id: activityReference.ThreadId),
                });

            var response = await _connectorClient.Conversations.UpdateActivityAsync(
                activityReference.ThreadId,
                activityReference.ActivityId,
                teamsChannelActivity,
                cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
