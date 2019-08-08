// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TurnContext } from 'botbuilder-teams/node_modules/botbuilder';
import * as teams from 'botbuilder-teams';
import * as request from 'request';
import * as fs from 'fs';

/**
 * Define data type for accept context
 */
interface ConsentContext {
    filename: string;
}

export class FileBot {
    private readonly fileFolder = './files';
    private readonly activityProc = new teams.TeamsActivityProcessor();

    constructor () {
        this.setupHandlers();
    }

    /**
     * Use onTurn to handle an incoming activity, received from a user, process it, and reply as needed
     * 
     * @param {TurnContext} context on turn context object.
     */
    public async onTurn(turnContext: TurnContext) {
        await this.activityProc.processIncomingActivity(turnContext);
    }

    /**
     *  Set up all activity handlers
     */
    private setupHandlers () {
        this.activityProc.messageActivityHandler = {
            onMessage: async (ctx) => {
                const filename = 'teams-logo.png';
                const fileinfo = fs.statSync(`${this.fileFolder}/${filename}`);
                await this.sendFileCard(ctx, filename, fileinfo.size);
            },

            onMessageWithFileDownloadInfo: async (ctx, file) => {
                await ctx.sendActivity({ textFormat: 'xml', text: `<b>Received File</b> <pre>${JSON.stringify(file, null, 2)}</pre>`});
                let filename: string;
                const err = await new Promise((resolve, reject) => {
                    let r = request(file.downloadUrl);
                    r.on('response', (res) => {
                        const regexp = /filename=\"(.*)\"/gi;
                        filename = regexp.exec( res.headers['content-disposition'] )[1];
                        res.pipe(fs.createWriteStream(`${this.fileFolder}/${filename}`));
                    });
                    r.on('error', (err) => resolve(err));
                    r.on('complete', (res) => resolve());
                });
                if (!err && !!filename) {
                    await ctx.sendActivity({ textFormat: 'xml', text: `Complete downloading <b>${filename}</b>` });
                }
            }
        };

        this.activityProc.invokeActivityHandler = {
            onFileConsent: async (ctx: TurnContext, query: teams.FileConsentCardResponse) => {
                await ctx.sendActivity({ textFormat: 'xml', text: `<b>Received user's consent</b> <pre>${JSON.stringify(query, null, 2)}</pre>`});

                const context: ConsentContext = query.context;

                // 'Accepted' case
                if (query.action === 'accept') {
                    const fname = `${this.fileFolder}/${context.filename}`;
                    const fileInfo = fs.statSync(fname);
                    const file = new Buffer(fs.readFileSync(fname, 'binary'), 'binary');
                    await ctx.sendActivity({ textFormat: 'xml', text: `Uploading <b>${context.filename}</b>`});
                    const r = new Promise((resolve, reject) => {
                        request.put({
                            uri: query.uploadInfo.uploadUrl,
                            headers: {
                                'Content-Length': fileInfo.size,
                                'Content-Range': `bytes 0-${fileInfo.size-1}/${fileInfo.size}`
                            },
                            encoding: null,
                            body: file
                        }, async (err, res) => {
                            if (err) {
                                reject(err);
                            } else {
                                const data = Buffer.from(res.body, 'binary').toString('utf8');
                                resolve(JSON.parse(data));
                            }
                        });
                    });

                    return r.then(async res => {
                        await this.fileUploadCompleted(ctx, query, res);
                        return { status: 200 }
                    }).catch(async err => {
                        await this.fileUploadFailed(ctx, err);
                        return { status: 500, body: `File upload failed: ${JSON.stringify(err)}` }
                    });
                }

                // 'Declined' case
                if (query.action === 'decline') {
                    await ctx.sendActivity({ textFormat: 'xml', text: `Declined. We won't upload file <b>${context.filename}</b>.`} );
                }

                return { status: 200 };
            }
        };
    }

    private async sendFileCard (ctx: TurnContext, filename: string, filesize: number) {
        const fileCard = teams.TeamsFactory.fileConsentCard(
            filename,
            {
                'description': 'This is the file I want to send you',
                'sizeInBytes': filesize,
                'acceptContext': <ConsentContext> {
                    filename
                },
                'declineContext': <ConsentContext> {
                    filename
                }
            }
        );
        await ctx.sendActivities([ { attachments: [fileCard] } ]);
    }

    private async fileUploadCompleted (ctx: TurnContext, query: teams.FileConsentCardResponse, response: any) {
        const downloadCard = teams.TeamsFactory.fileInfoCard(
            query.uploadInfo.name,
            query.uploadInfo.contentUrl,
            {
                'uniqueId': query.uploadInfo.uniqueId,
                'fileType': query.uploadInfo.fileType
            }
        );
        await ctx.sendActivities([
            {
                textFormat: 'xml', 
                text: `<b>File Upload Completed</b> <pre>${JSON.stringify(response, null, 2)}</pre>`
            },
            { 
                textFormat: 'xml',
                text: `Your file <b>${query.context.filename}</b> is ready to download`,
                attachments: [downloadCard] 
            }
        ]);
    }

    private async fileUploadFailed (ctx: TurnContext, error: any) {
        await ctx.sendActivity({
            textFormat: 'xml', 
            text: `<b>File Upload Failed</b> <pre>${JSON.stringify(error, null, 2)}</pre>`
        });
    }
}
