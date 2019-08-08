// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsFileBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            bool messageWithFileDownloadInfo = turnContext.Activity.Attachments?[0].ContentType == FileDownloadInfo.ContentType;
            if (messageWithFileDownloadInfo)
            {
                Attachment file = turnContext.Activity.Attachments[0];
                FileDownloadInfo fileDownload = JObject.FromObject(file.Content).ToObject<FileDownloadInfo>();

                string filePath = "Files\\" + file.Name;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(fileDownload.DownloadUrl, filePath);
                }

                var reply = ((Activity)turnContext.Activity).CreateReply();
                reply.TextFormat = "xml";
                reply.Text = $"Complete downloading <b>{file.Name}</b>";
                await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
            }
            else
            {
                string filename = "teams-logo.png";
                string filePath = "Files\\" + filename;
                long fileSize = new FileInfo(filePath).Length;
                await this.SendFileCardAsync(turnContext, filename, fileSize).ConfigureAwait(false);
            }
        }

        protected override async Task<InvokeResponse> OnFileConsent(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            var query = JObject.FromObject(turnContext.Activity.Value).ToObject<FileConsentCardResponse>();
            return await HandleFileConsentResponseAsync(turnContext, query);
        }

        private async Task SendFileCardAsync(ITurnContext turnContext, string filename, long filesize)
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

            Activity replyActivity = turnContext.Activity.CreateReply();
            replyActivity.Attachments = new List<Attachment>()
            {
                fileCard.ToAttachment(filename),
            };

            await turnContext.SendActivityAsync(replyActivity).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles file consent response asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The query object of file consent user's response.</param>
        /// <returns>Task tracking operation.</returns>
        public async Task<InvokeResponse> HandleFileConsentResponseAsync(ITurnContext turnContext, FileConsentCardResponse query)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>Received user's consent</b> <pre>{JObject.FromObject(query).ToString()}</pre>";
            await turnContext.SendActivityAsync(reply).ConfigureAwait(false);

            JToken context = JObject.FromObject(query.Context);

            if (query.Action.Equals("accept"))
            {
                try
                {
                    string filePath = "Files\\" + context["filename"];
                    string fileUploadUrl = query.UploadInfo.UploadUrl;
                    long fileSize = new FileInfo(filePath).Length;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Content-Length", fileSize.ToString());
                        client.Headers.Add("Content-Range", $"bytes 0-{fileSize - 1}/{fileSize}");
                        using (Stream fileStream = File.OpenRead(filePath))
                        using (Stream requestStream = client.OpenWrite(new Uri(fileUploadUrl), "PUT"))
                        {
                            fileStream.CopyTo(requestStream);
                        }
                    }

                    await this.FileUploadCompletedAsync(turnContext, query).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await this.FileUploadFailedAsync(turnContext, e.ToString()).ConfigureAwait(false);
                }
            }

            if (query.Action.Equals("decline"))
            {
                reply.Text = $"Declined. We won't upload file <b>{context["filename"]}</b>.";
                await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
            }

            return new InvokeResponse { Status = 200 };
        }

        private async Task FileUploadCompletedAsync(ITurnContext turnContext, FileConsentCardResponse query)
        {
            var downloadCard = new FileInfoCard()
            {
                UniqueId = query.UploadInfo.UniqueId,
                FileType = query.UploadInfo.FileType,
            };

            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>File uploaded.</b> Your file <b>{query.UploadInfo.Name}</b> is ready to download";
            reply.Attachments = new List<Attachment>
            {
                downloadCard.ToAttachment(query.UploadInfo.Name, query.UploadInfo.ContentUrl),
            };

            await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
        }

        private async Task FileUploadFailedAsync(ITurnContext turnContext, string error)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>File upload failed.</b> Error: <pre>{error}</pre>";
            await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
        }
    }
}
