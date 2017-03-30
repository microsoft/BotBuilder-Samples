using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Diagnostics;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
//get facebook channel data models
using Azure_Bot_Generic_CSharp.Models;

namespace Azure_Bot_Generic_CSharp
{
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
                Trace.WriteLine("Got Message");
                Trace.WriteLine(activity.ToString());
                //create connector service
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //check for a Facebook message here
                if (activity.ChannelId == "facebook")
                {
                    //send acknowledgement
                    Activity reply = activity.CreateReply("Facebook Messenger message detected! Beep boop. [o o]");
                    await connector.Conversations.ReplyToActivityAsync(reply);

                    //send to the share button dialog
                    await Conversation.SendAsync(activity, () => new ShareButtonDialog());
                }
                else
                {
                    // calculate something for us to return
                    int length = (activity.Text ?? string.Empty).Length;

                    // return our reply to the user
                    Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}