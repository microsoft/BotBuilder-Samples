using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace InlineImageDownload.Bot
{
    public class ImageDownloadBot : ActivityHandler
    {
        private readonly IConfiguration configuration;

        public ImageDownloadBot(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                var message = turnContext.Activity;
                if (message.Attachments!=null)
                {
                    var attachment = message.Attachments[0];
                    using (HttpClient httpClient = new HttpClient())
                    {
                        // MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                        var token = await new MicrosoftAppCredentials(this.configuration["MicrosoftAppId"], this.configuration["MicrosoftAppPassword"]).GetTokenAsync();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);
                        var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;

                        // You could not use this response message to fetch the image for further processing.
                        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Stream attachmentStream = await responseMessage.Content.ReadAsStreamAsync();
                            attachmentStream.Position = 0;
                            var image = Image.FromStream(attachmentStream);
                            // Saves the image in solution folder
                            image.Save(@"ImageFromUser.png");
                            await turnContext.SendActivityAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");
                        }
                        else
                        {
                            await turnContext.SendActivityAsync($"Resoponse: {responseMessage.ToString()} \n\nContentUrl: {attachment.ContentUrl}\n\nBearerToken: {token}");

                        }
                    }
                }
                else
                {
                    await turnContext.SendActivityAsync($"No image attachment received");
                }
            }
            catch (Exception ex)
            {
                await turnContext.SendActivityAsync(ex.ToString());
            }

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
