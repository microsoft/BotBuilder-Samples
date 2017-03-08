namespace TestBot
{
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;

    public sealed class ActivityLogger : IActivityLogger
    {
        private readonly IBotData botData;

        public ActivityLogger(IBotData botData)
        {
            this.botData = botData;
        }

        public async Task LogAsync(IActivity activity)
        {
            var message = activity.AsMessageActivity();
            if (message != null && message.From.Name.Equals(WebConfigurationManager.AppSettings["BotId"]) && message.Attachments.Count > 0)
            {
                var jsonAttachments = JsonConvert.SerializeObject(message.Attachments, Formatting.Indented);

                // we access and update the received instance cause if get/save from bot state, 
                // the ETag changes and saving this cached instance later fails (with this we are using the loaded data)
                this.botData.PrivateConversationData.SetValue(Constants.LastJsonKey, jsonAttachments);
            }
        }
    }
}