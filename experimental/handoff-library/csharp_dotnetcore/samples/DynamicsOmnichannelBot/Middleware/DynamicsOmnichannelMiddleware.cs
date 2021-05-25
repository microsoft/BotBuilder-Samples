using DynamicsOmnichannelBot.DynamicsOmnichannel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsOmnichannelBot.Middleware
{
    public class DynamicsOmnichannelMiddleware : IMiddleware
    {
        private const string Tags = "tags";
        private const string DeliveryMode = "deliveryMode";

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            turnContext.OnSendActivities(async (sendTurnContext, activities, nextSend) =>
            {
                #region Message handling
                IEnumerable<Activity> messageActivities = activities.Where((activity) => activity.Type == ActivityTypes.Message);

                // Set "bridged" flag to allow channel to bridge message to the C2
                foreach (var messageActivity in messageActivities)
                {
                    Dictionary<string, object> channelData = messageActivity.ChannelData as Dictionary<string, object> ?? new Dictionary<string, object>();
                    channelData[DeliveryMode] = "bridged";
                    messageActivity.ChannelData = channelData;
                }
                #endregion

                #region Handoff Event handling
                Activity handOffEventActivity = activities.Where((activity) => activity.Type == ActivityTypes.Event && activity.Name == "handoff.initiate").FirstOrDefault();

                // Events are not supported by Teams channel, Hand off event is passed as a special message to Omnichannel
                if (handOffEventActivity != null)
                {
                    dynamic handOffContext = handOffEventActivity.Value;

                    // Construct Omnichannel command
                    Command escalationCommand = new Command
                    {
                        Type = CommandType.Escalate,
                        Context = handOffContext.Context.ToObject<Dictionary<string, object>>()
                    };

                    handOffEventActivity.Type = ActivityTypes.Message;
                    handOffEventActivity.Text = handOffContext.MessageToAgent;

                    // Set escalation context in channel data
                    handOffEventActivity.ChannelData = BuildCommandChannelData(escalationCommand, handOffEventActivity.ChannelData as Dictionary<string, object>);

                    // Clearing out Value since it is unused
                    handOffEventActivity.Value = null;
                }
                #endregion

                #region End of Conversation handling
                Activity endOfConversationActivity = activities.Where((activity) => activity.Type == ActivityTypes.EndOfConversation).FirstOrDefault();

                if (endOfConversationActivity != null)
                {
                    // Construct Omnichannel command
                    Command endOfConversationCommand = new Command
                    {
                        Type = CommandType.EndConversation,
                    };

                    endOfConversationActivity.Type = ActivityTypes.Message;
                    endOfConversationActivity.Text = "End of conversation";

                    // Set end of conversation context in channel data
                    endOfConversationActivity.ChannelData = BuildCommandChannelData(endOfConversationCommand, endOfConversationActivity.ChannelData as Dictionary<string, object>);
                }
                #endregion

                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);
                return responses;
            });

            await next(cancellationToken).ConfigureAwait(false);
        }

        private static Dictionary<string, object> BuildCommandChannelData(Command command, Dictionary<string, object> channelData)
        {
            channelData ??= new Dictionary<string, object>();
            channelData[Tags] = JsonConvert.SerializeObject(command);

            return channelData;
        }
    }
}
