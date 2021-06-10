// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const path = require('path');
const axios = require('axios');
const { TurnContext, MessageFactory, TeamsActivityHandler } = require('botbuilder');
const { MicrosoftAppCredentials } = require('botframework-connector');
const { geneFileName, getFileSize, writeFile } = require('../services/fileService');
const FILES_DIR = 'files';

class TeamsFileUploadBot extends TeamsActivityHandler {
    constructor() {
        super();
        this.onMessage(async (context, next) => {
            TurnContext.removeRecipientMention(context.activity);
            const attachments = context.activity.attachments;
            const imageRegex = /image\/.*/;
            if (attachments && attachments[0] && attachments[0].contentType === 'application/vnd.microsoft.teams.file.download.info') {
                const file = attachments[0];
                const config = {
                    responseType: 'stream'
                };
                const filePath = path.join(FILES_DIR, file.name);
                await writeFile(file.content.downloadUrl, config, filePath);
                const reply = MessageFactory.text(`<b>${ file.name }</b> received and saved.`);
                reply.textFormat = 'xml';
                await context.sendActivity(reply);
            } else if (attachments && attachments[0] && imageRegex.test(attachments[0].contentType)) {
                await this.processInlineImage(context);
            } else {
                const filename = 'teams-logo.png';
                const stats = fs.statSync(path.join(FILES_DIR, filename));
                const fileSize = stats.size;
                await this.sendFileCard(context, filename, fileSize);
            }
            await next();
        });
    }

    async sendFileCard(context, filename, filesize) {
        const consentContext = { filename: filename };

        const fileCard = {
            description: 'This is the file I want to send you',
            sizeInBytes: filesize,
            acceptContext: consentContext,
            declineContext: consentContext
        };
        const asAttachment = {
            content: fileCard,
            contentType: 'application/vnd.microsoft.teams.card.file.consent',
            name: filename
        };
        await context.sendActivity({ attachments: [asAttachment] });
    }

    async handleTeamsFileConsentAccept(context, fileConsentCardResponse) {
        try {
            const fname = path.join(FILES_DIR, fileConsentCardResponse.context.filename);
            const fileInfo = fs.statSync(fname);
            const fileContent = Buffer.from(fs.readFileSync(fname, 'binary'), 'binary');
            await axios.put(
                fileConsentCardResponse.uploadInfo.uploadUrl,
                fileContent, {
                    headers: {
                        'Content-Type': 'image/png',
                        'Content-Length': fileInfo.size,
                        'Content-Range': `bytes 0-${ fileInfo.size - 1 }/${ fileInfo.size }`
                    }
                });
            await this.fileUploadCompleted(context, fileConsentCardResponse);
        } catch (e) {
            await this.fileUploadFailed(context, e.message);
        }
    }

    async handleTeamsFileConsentDecline(context, fileConsentCardResponse) {
        const reply = MessageFactory.text(`Declined. We won't upload file <b>${ fileConsentCardResponse.context.filename }</b>.`);
        reply.textFormat = 'xml';
        await context.sendActivity(reply);
    }

    async fileUploadCompleted(context, fileConsentCardResponse) {
        const downloadCard = {
            uniqueId: fileConsentCardResponse.uploadInfo.uniqueId,
            fileType: fileConsentCardResponse.uploadInfo.fileType
        };
        const asAttachment = {
            content: downloadCard,
            contentType: 'application/vnd.microsoft.teams.card.file.info',
            name: fileConsentCardResponse.uploadInfo.name,
            contentUrl: fileConsentCardResponse.uploadInfo.contentUrl
        };
        const reply = MessageFactory.text(`<b>File uploaded.</b> Your file <b>${ fileConsentCardResponse.uploadInfo.name }</b> is ready to download`);
        reply.textFormat = 'xml';
        reply.attachments = [asAttachment];
        await context.sendActivity(reply);
    }

    async fileUploadFailed(context, error) {
        const reply = MessageFactory.text(`<b>File upload failed.</b> Error: <pre>${ error }</pre>`);
        reply.textFormat = 'xml';
        await context.sendActivity(reply);
    }

    async processInlineImage(context) {
        const file = context.activity.attachments[0];
        const credentials = new MicrosoftAppCredentials(process.env.MicrosoftAppId, process.env.MicrosoftAppPassword);
        const botToken = await credentials.getToken();
        const config = {
            headers: { Authorization: `Bearer ${ botToken }` },
            responseType: 'stream'
        };
        const fileName = await geneFileName(FILES_DIR);
        const filePath = path.join(FILES_DIR, fileName);
        await writeFile(file.contentUrl, config, filePath);
        const fileSize = await getFileSize(filePath);
        const reply = MessageFactory.text(`Image <b>${ fileName }</b> of size <b>${ fileSize }</b> bytes received and saved.`);
        const inlineAttachment = this.getInlineAttachment(fileName);
        reply.attachments = [inlineAttachment];
        await context.sendActivity(reply);
    }

    getInlineAttachment(fileName) {
        const imageData = fs.readFileSync(path.join(FILES_DIR, fileName));
        const base64Image = Buffer.from(imageData).toString('base64');
        return {
            name: fileName,
            contentType: 'image/png',
            contentUrl: `data:image/png;base64,${ base64Image }`
        };
    }
}

module.exports.TeamsFileUploadBot = TeamsFileUploadBot;
