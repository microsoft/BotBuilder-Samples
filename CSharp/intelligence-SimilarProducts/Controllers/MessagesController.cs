namespace SimilarProducts.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Connector;
    using Services;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IImageSearchService imageService = new BingImageSearchService();

        /// <summary>
        /// Maximum number of hero cards to be returned in the carousel. If this number is greater than 5, skype throws an exception.
        /// </summary>
        private const int MaxCardCount = 5;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string message = null;
                bool replied = false;

                try
                {
                    var images = await this.GetSimilarImagesAsync(activity, connector);

                    if (images != null && images.Any())
                    {
                        Activity reply = activity.CreateReply("Here are some visually similar products I found");
                        reply.Type = ActivityTypes.Message;
                        reply.AttachmentLayout = "carousel";
                        reply.Attachments = this.BuildImageAttachments(images.Take(MaxCardCount));
                        await connector.Conversations.ReplyToActivityAsync(reply);
                        replied = true;
                    }
                    else
                    {
                        message = "Couldn't find similar products images for this one";
                    }
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

                if (!replied)
                {
                    Activity reply = activity.CreateReply(message);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                await this.HandleSystemMessage(activity);
            }

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        /// <summary>
        /// Gets a list of visually similar products asynchronously by checking the type of the image (stream vs URL)
        /// and calling the appropriate image service method.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="connector">The connector.</param>
        /// <returns>List of visually similar products' images.</returns>
        /// <exception cref="ArgumentException">The activity doesn't contain a valid image attachment or an image URL.</exception>
        private async Task<IList<ImageResult>> GetSimilarImagesAsync(Activity activity, ConnectorClient connector)
        {
            var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
            if (imageAttachment != null)
            {
                using (var stream = await GetImageStream(connector, imageAttachment))
                {
                    return await this.imageService.GetSimilarProductImagesAsync(stream);
                }
            }

            string url;
            if (TryParseAnchorTag(activity.Text, out url))
            {
                return await this.imageService.GetSimilarProductImagesAsync(url);
            }

            if (Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute))
            {
                return await this.imageService.GetSimilarProductImagesAsync(activity.Text);
            }

            // If we reach here then the activity is neither an image attachment nor an image URL.
            throw new ArgumentException("The activity doesn't contain a valid image attachment or an image URL.");
        }

        private IList<Attachment> BuildImageAttachments(IEnumerable<ImageResult> images)
        {
            var attachments = new List<Attachment>();

            foreach (var image in images)
            {
                var plAttachment = new Attachment { ContentType = "application/vnd.microsoft.card.hero" };

                //Construct Card
                var plCard = new HeroCard
                {
                    Title = image.Name,
                    Subtitle = image.HostPageDisplayUrl,
                    Images = new List<CardImage>()
                };

                //Add Card Image
                var img = new CardImage { Url = image.ThumbnailUrl };
                plCard.Images.Add(img);

                //Add Card Buttons
                plCard.Buttons = new List<CardAction>();
                var plButtonBuy = new CardAction();
                var plButtonSearch = new CardAction();

                //Buy Button
                plButtonBuy.Title = "Buy from merchant";
                plButtonBuy.Type = "openUrl";
                plButtonBuy.Value = image.HostPageUrl;

                //Search More button 
                plButtonSearch.Title = "Find more in Bing";
                plButtonSearch.Type = "openUrl";
                plButtonSearch.Value = image.WebSearchUrl;

                plCard.Buttons.Add(plButtonBuy);
                plCard.Buttons.Add(plButtonSearch);
                plAttachment.Content = plCard;

                attachments.Add(plAttachment);
            }

            return attachments;
        }

        /// <summary>
        /// Gets the image stream.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <param name="imageAttachment">The image attachment.</param>
        /// <returns></returns>
        private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(imageAttachment.ContentType));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        /// <summary>
        /// Gets the href value in an anchor element.
        /// </summary>
        ///  Skype transforms raw urls to html. Here we extract the href value from the url
        /// <param name="text">Anchor tag html.</param>
        /// <param name="url">Url if valid anchor tag, null otherwise</param>
        /// <returns>True if valid anchor element</returns>
        private static bool TryParseAnchorTag(string text, out string url)
        {
            var regex = new Regex("^<a href=\"(?<href>[^\"]*)\">[^<]*</a>$", RegexOptions.IgnoreCase);
            url = regex.Matches(text).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
            return url != null;
        }


        /// <summary>
        /// Gets the JwT token of the bot. 
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>JwT token of the bot</returns>
        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
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
                        response.Text = "Hi! I am SimilarProducts Bot. I can find you similar products" +
                                        " Try sending me an image or an image URL.";

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