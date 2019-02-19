namespace LuisBot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Http;
    using Dialogs;
    using LuisBot.Controllers;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                if (!await TranslatorController.IsUserChangedLangauage(activity).ConfigureAwait(false))
                {
                    await TranslatorController.TranslateActivityTextAsync(activity).ConfigureAwait(false);
                    await Conversation.SendAsync(activity, () => new RootLuisDialog());
                }
            }
            else
            {
                await this.HandleSystemMessage(activity).ConfigureAwait(false);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if(message.Type == ActivityTypes.ConversationUpdate)
            {
                for (int i = 0; i < message.MembersAdded.Count; i++)
                {
                    if (message.MembersAdded[i].Id != message.Recipient.Id)
                    {
                        var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                        string reply = "Welcome to Luis Translator sample.\n To set your language please enter \"set my language to {languageId}\"";
                        await connector.Conversations.ReplyToActivityAsync(message.CreateReply(reply)).ConfigureAwait(false);
                        break;
                    }
                }
            }
            else if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

    }
}
