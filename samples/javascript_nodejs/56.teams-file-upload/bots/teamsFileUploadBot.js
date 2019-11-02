// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const path = require('path');

const {
    TeamsActivityHandler
} = require('botbuilder');

class TeamsFileUploadBot extends TeamsActivityHandler {
    constructor() {
        super();
        this.onMessage(async (context, next) => {
            const messageWithFileDownloadInfo = context.activity.attachments[0].contentType == FileDownloadInfo.contentType;
            if (messageWithFileDownloadInfo) {
                const file = context.activity.attachments[0];
                const fileDownload = file.content;

                const filePath = path.join('Files', file.name);

                // var client = _clientFactory.CreateClient();
                // var response = await client.GetAsync(fileDownload.DownloadUrl);

                // using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                // {
                //     await response.Content.CopyToAsync(fileStream);
                // }

                const reply = MessageFactory.Text(`Complete downloading <b>${ file.Name }</b>`);
                reply.textFormat = "xml";

                await context.sendActivity(reply);
            } else {
                const filename = "teams-logo.png";
                const stats = fs.statSync(path.join("Files", filename));
                const fileSize = stats.size;
                await sendFileCard(context, filename, fileSize);
            }

            await next();
        });
    }

    async sendFileCard(context, filename, filesize) {
        const consentContext = { filename: filename };

        const fileCard = {
            description = 'This is the file I want to send you',
            sizeInBytes = filesize,
            acceptContext = consentContext,
            declineContext = consentContext
        };

        const asAttachment = {
            content = fileCard,
            contentType = 'application/vnd.microsoft.teams.card.file.consent',
            name = filename,
        };

        await context.sendActivity({ attachments: [ asAttachment ]});
    }

    async handleTeamsFileConsentAccept(context, fileConsentCardResponse) {
        try {
            const filePath = path.join("Files", fileConsentCardResponse.context.filename);
            const stats = fs.statSync(filePath);
            const fileSize = stats.Length;
            
            // var client = _clientFactory.CreateClient();
            // using (var fileStream = File.OpenRead(filePath))
            // {
            //     var fileContent = new StreamContent(fileStream);
            //     fileContent.Headers.ContentLength = fileSize;
            //     fileContent.Headers.ContentRange = new ContentRangeHeaderValue(0, fileSize - 1, fileSize);
            //     await client.PutAsync(fileConsentCardResponse.UploadInfo.UploadUrl, fileContent);
            // }

            await fileUploadCompleted(context, fileConsentCardResponse);
        }
        catch (e) {
            await fileUploadFailed(context, e.message);
        }
    }

    async handleTeamsFileConsentDecline(context, fileConsentCardResponse) {
        const reply = MessageFactory.text(`Declined. We won't upload file <b>${ fileConsentCardResponse.context.filename }</b>.`);
        reply.textFormat = 'xml';
        await context.sendActivity(reply);
    }

    async fileUploadCompleted(context, fileConsentCardResponse) {
        const downloadCard = {
            uniqueId = fileConsentCardResponse.uploadInfo.uniqueId,
            fileType = fileConsentCardResponse.uploadInfo.fileType,
        };

        const asAttachment = {
            content = downloadCard,
            contentType = 'application/vnd.microsoft.teams.card.file.info',
            name = fileConsentCardResponse.uploadInfo.name,
            contentUrl = fileConsentCardResponse.uploadInfo.contentUrl,
        };

        const reply = MessageFactory.text(`<b>File uploaded.</b> Your file <b>${ fileConsentCardResponse.uploadInfo.name }</b> is ready to download`);
        reply.textFormat = "xml";
        reply.attachments = [ asAttachment ];
        await context.sendActivity(reply);
    }

    async fileUploadFailed(context, error) {
        const reply = MessageFactory.text(`<b>File upload failed.</b> Error: <pre>${ error }</pre>`);
        reply.textFormat = 'xml';
        await context.sendActivity(reply);
    }
}

module.exports.TeamsFileUploadBot = TeamsFileUploadBot;
