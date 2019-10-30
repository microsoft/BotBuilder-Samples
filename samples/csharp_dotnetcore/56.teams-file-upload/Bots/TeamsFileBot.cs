// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsFileBot : TeamsActivityHandler
    {
        /*
         * You can install this bot at any scope. You can @mention the bot and it will present you with the file prompt. You can accept and 
         * the file will be uploaded, or you can decline and it won't.
         */

        private readonly IHttpClientFactory _clientFactory;

        public TeamsFileBot(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            bool messageWithFileDownloadInfo = turnContext.Activity.Attachments?[0].ContentType == FileDownloadInfo.ContentType;
            if (messageWithFileDownloadInfo)
            {
                var file = turnContext.Activity.Attachments[0];
                var fileDownload = JObject.FromObject(file.Content).ToObject<FileDownloadInfo>();

                string filePath = Path.Combine("Files", file.Name);

                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync(fileDownload.DownloadUrl);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }

                var reply = ((Activity)turnContext.Activity).CreateReply();
                reply.TextFormat = "xml";
                reply.Text = $"Complete downloading <b>{file.Name}</b>";
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else
            {
                string filename = "teams-logo.png";
                string filePath = Path.Combine("Files", filename);
                long fileSize = new FileInfo(filePath).Length;
                await SendFileCardAsync(turnContext, filename, fileSize, cancellationToken);
            }
        }

        private async Task SendFileCardAsync(ITurnContext turnContext, string filename, long filesize, CancellationToken cancellationToken)
        {
            var consentContext = new Dictionary<string, string>
            {
                { "filename", filename },
            };

            var fileCard = new FileConsentCard
            {
                Description = "This is the file I want to send you",
                SizeInBytes = filesize,
                AcceptContext = consentContext,
                DeclineContext = consentContext,
            };

            var asAttachment = new Attachment
            {
                Content = fileCard,
                ContentType = FileConsentCard.ContentType,
                Name = filename,
            };

            var replyActivity = turnContext.Activity.CreateReply();
            replyActivity.Attachments = new List<Attachment>() { asAttachment };

            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }

        protected override async Task OnTeamsFileConsentAcceptAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            try
            {
                JToken context = JObject.FromObject(fileConsentCardResponse.Context);

                string filePath = Path.Combine("Files", context["filename"].ToString());
                long fileSize = new FileInfo(filePath).Length;
                var client = _clientFactory.CreateClient();
                using (var fileStream = File.OpenRead(filePath))
                {
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentLength = fileSize;
                    fileContent.Headers.ContentRange = new ContentRangeHeaderValue(0, fileSize - 1, fileSize);
                    await client.PutAsync(fileConsentCardResponse.UploadInfo.UploadUrl, fileContent, cancellationToken);
                }

                await FileUploadCompletedAsync(turnContext, fileConsentCardResponse, cancellationToken);
            }
            catch (Exception e)
            {
                await FileUploadFailedAsync(turnContext, e.ToString(), cancellationToken);
            }
        }

        protected override async Task OnTeamsFileConsentDeclineAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            JToken context = JObject.FromObject(fileConsentCardResponse.Context);

            var reply = ((Activity)turnContext.Activity).CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"Declined. We won't upload file <b>{context["filename"]}</b>.";
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task FileUploadCompletedAsync(ITurnContext turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            var downloadCard = new FileInfoCard
            {
                UniqueId = fileConsentCardResponse.UploadInfo.UniqueId,
                FileType = fileConsentCardResponse.UploadInfo.FileType,
            };

            var asAttachment = new Attachment
            {
                Content = downloadCard,
                ContentType = FileInfoCard.ContentType,
                Name = fileConsentCardResponse.UploadInfo.Name,
                ContentUrl = fileConsentCardResponse.UploadInfo.ContentUrl,
            };

            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>File uploaded.</b> Your file <b>{fileConsentCardResponse.UploadInfo.Name}</b> is ready to download";
            reply.Attachments = new List<Attachment> { asAttachment };

            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task FileUploadFailedAsync(ITurnContext turnContext, string error, CancellationToken cancellationToken)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>File upload failed.</b> Error: <pre>{error}</pre>";
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
