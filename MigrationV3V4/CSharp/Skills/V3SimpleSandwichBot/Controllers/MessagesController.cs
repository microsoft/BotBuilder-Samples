// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Net.Http;
using System.Web.Http.Description;
using System.Diagnostics;
using Microsoft.Bot.Connector.SkillAuthentication;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using System.Threading;

namespace Microsoft.Bot.Sample.SimpleSandwichBot
{
    [SkillBotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<SandwichOrder> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        if (activity.Text.ToLower().Contains("end") || activity.Text.ToLower().Contains("stop"))
                        {
                            await ConversationHelper.ClearState(activity);
                            await ConversationHelper.EndConversation(activity, endOfConversationCode: EndOfConversationCodes.UserCancelled);
                        }
                        else
                        {
                            await Conversation.SendAsync(activity, MakeRootDialog);
                        }

                        break;
                    case ActivityTypes.EndOfConversation:
                        Trace.TraceInformation($"EndOfConversation: {activity}");

                        // Clear the dialog stack if the root bot has ended the conversation.
                        await ConversationHelper.ClearState(activity);

                        break;
                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}
