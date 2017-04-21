namespace EmergencyServicesBot
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    public class AgentListener
    {
        // This will send an adhoc message to the user
        public static async Task Resume(
            string toId, 
            string toName, 
            string fromId, 
            string fromName, 
            string conversationId, 
            string message, 
            string serviceUrl = "https://smba.trafficmanager.net/apis/", 
            string channelId = "skype")
        {
            if (!MicrosoftAppCredentials.IsTrustedServiceUrl(serviceUrl))
            {
                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
            }

            try
            {
                var userAccount = new ChannelAccount(toId, toName);
                var botAccount = new ChannelAccount(fromId, fromName);
                var connector = new ConnectorClient(new Uri(serviceUrl));

                IMessageActivity activity = Activity.CreateMessageActivity();

                if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
                {
                    activity.ChannelId = channelId;
                }
                else
                {
                    conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
                }

                activity.From = botAccount;
                activity.Recipient = userAccount;
                activity.Conversation = new ConversationAccount(id: conversationId);
                activity.Text = message;
                activity.Locale = "en-Us";
                await connector.Conversations.SendToConversationAsync((Activity)activity);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
            }
        }
    }
}