namespace ImageCaption.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Connector;
    using Services;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly ICaptionService captionService = new MicrosoftCognitiveCaptionService();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string message;

                try
                {
                    message = await this.GetCaptionAsync(activity);
                }
                catch (ArgumentException e)
                {
                    message = "Did you upload an image? I'm more of a visual person. " +
                        "Try sending me an image or an image URL";

                    Trace.TraceError(e.ToString());
                }
                catch (Exception e)
                {
                    message = "Oops! Something went wrong. Try again later.";

                    Trace.TraceError(e.ToString());
                }

                Activity reply = activity.CreateReply(message);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                await this.HandleSystemMessage(activity);
            }

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        /// <summary>
        /// Gets the caption asynchronously by checking the type of the image (stream vs URL)
        /// and calling the appropriate caption service method.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns>The caption if found</returns>
        /// <exception cref="ArgumentException">The activity doesn't contain a valid image attachment or an image URL.</exception>
        private async Task<string> GetCaptionAsync(Activity activity)
        {
           
            var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
            if (imageAttachment != null)
            {
                var req = WebRequest.Create(imageAttachment.ContentUrl);

                using (var stream = req.GetResponse().GetResponseStream())
                {
                    return await this.captionService.GetCaptionAsync(stream);
                }
            }
            else if (Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute))
            {
                return await this.captionService.GetCaptionAsync(activity.Text);
            }

            // If we reach here then the activity is neither an image attachment nor an image URL.
            throw new ArgumentException("The activity doesn't contain a valid image attachment or an image URL.");
        }
        /// <summary>
        /// Handles the system activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns>Activity</returns>
        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Greet the user the first time the bot is added to a conversation.
                    if (activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
                    {
                        var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                        var response = activity.CreateReply();
                        response.Text = "Hi! I am ImageCaption Bot. I can understand the content of any image" +
                                        " and try to describe it as well as any human. Try sending me an image or an image URL.";

                        await connector.Conversations.ReplyToActivityAsync(response);
                    }
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing that the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return null;
        }
    }
}