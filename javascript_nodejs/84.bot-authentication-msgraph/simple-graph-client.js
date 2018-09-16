// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const request = require('request'); // Pending Deletion.
const axios = require('axios');
const MicrosoftGraph = require('@microsoft/microsoft-graph-client');

/**
 * This class is a wrapper for the Microsoft Graph API. 
 * See: https://developer.microsoft.com/en-us/graph for more information.
 */
class SimpleGraphClient {

    constructor(token) {
        if (!token || !token.trim()) {
            throw new Error('SimpleGraphClient: Invalid token received.');
        }

        this._token = token;

        // Get an Authenticated Microsoft Graph client using the token issued to the user.
        this.graphClient = MicrosoftGraph.Client.init({
            authProvider: (done) => {
                done(null, this._token); // First parameter takes an error if you can't get an access token.
            }
        });
    }

    /**
     * Sends an email on the users behalf using the Microsoft Graph API.
     * @param {*} toAddress 
     * @param {*} subject 
     * @param {*} content 
     */
    async sendMail(toAddress, subject, content) {
        if (!toAddress || !toAddress.trim()) {
            throw new Error('SimpleGraphClient.sendMail(): Invalid `toAddress` parameter received.');
        }
        if (!subject || !subject.trim()) {
            throw new Error('SimpleGraphClient.sendMail(): Invalid `subject`  parameter received.');
        }
        if (!content || !content.trim()) {
            throw new Error('SimpleGraphClient.sendMail(): Invalid `content` parameter received.');
        }

        // Create the message.
        const mail = {
            body: {
                content: content, // `Hi there! I had this message sent from a bot. - Your friend, ${graphData.displayName}!`,
                contentType: 'Text'
            },
            subject: subject, // `Message from a bot!`,
            toRecipients: [{
                emailAddress: {
                    address: toAddress
                }
            }]
        };

        // Send the message.
        return await this.graphClient
            .api('/me/sendMail')
            .post({ message: mail }, (error, res) => {
                if (error) {
                    throw error;
                } else {
                    return res;
                }
            });
    }

    /**
     * Gets recent mail the user has received within the last hour and displays up to 5 of the emails in the bot.
     */
    async getRecentMail() {
        return await this.graphClient
            .api('/me/messages')
            .version('beta')
            .top(5)
            .get().then((res) => {
                return res;
            });
    }

    /**
     * Collects information about the user in the bot.
     */
    async getMe() {
        return await this.graphClient
            .api('/me')
            .get().then((res) => {
                return res;
            });
    }

    /**
     * Collects the user's manager in the bot.
     */
    async getManager() {
        return await this.graphClient
            .api('/me/manager')
            .version('beta')
            .select('displayName')
            .get().then((res) => {
                return res;
            });
    }

    /**
     * Collects the user's photo on MS Graph.
     */
    async getPhoto() {
        const photoResponse = await axios.request({
            "method": "GET",
            "url": "https://graph.microsoft.com/v1.0/me/photo/$value",
            // "responseType": "stream",
            // "responseEncoding": null,
            "headers": {
                "Authorization": "Bearer " + this._token,
                "Accept": "application/json"
            }
        });

        // Grab the content-type header for the data URI.
        const contentType = photoResponse.headers['content-type'];

        // Encode the raw body as a base64 string.
        const base64Body = photoResponse.data.toString("base64");
        // Assign your values to the photo object.
        const photo = {
            'contentType': contentType,
            // Construct a Data URI for the image.
            'base64string': `data:${contentType};base64,${base64Body}`
        };
        photo.data = [
            {
                '@odata.type': '#microsoft.graph.fileAttachment',
                contentBytes: base64Body,
                contentLocation: "https://graph.microsoft.com/v1.0/me/photo/$value",
                isinline: true,
                name: 'mypic.jpg'
            }
        ];

        return photo;

        // const optionsPhoto = {
        //     "url": "https://graph.microsoft.com/v1.0/me/photo/$value",
        //     "responseType": "stream",
        //     "encoding": null, // Tells request this is a binary response
        //     "method": "GET",
        //     "headers": {
        //         "Authorization": "Bearer " + this._token
        //     }
        // };

        // return await request(optionsPhoto, (error, response, body) => {
        //     if (!error && response.statusCode == 200) {
        //         // Grab the content-type header for the data URI
        //         const contentType = response.headers["content-type"];

        //         // Encode the raw body as a base64 string
        //         const base64Body = body.toString("base64");

        //         // Construct a Data URI for the image
        //         const base64DataUri = "data:" + contentType + ";base64," + base64Body;

        //         // Assign your values to the photoResponse object
        //         const photoResponse = {};
        //         photoResponse.data = [
        //             {
        //                 "@odata.type": "#microsoft.graph.fileAttachment",
        //                 "contentBytes": base64Body,
        //                 "contentLocation": optionsPhoto.url,
        //                 "isinline": true,
        //                 "Name": "mypic.jpg"
        //             }
        //         ];
        //         photoResponse.contentType = contentType;
        //         photoResponse.base64string = base64DataUri;

        //         return photoResponse;
        //     } else {
        //         console.log(error);
        //     }
        // });

        // return await this.graphClient
        //     .api('/me/photo/$value')
        //     .get().then((error, res, body) => {
        //         console.log('getPhoto');
        //         console.log(error);
        //         console.log(res);
        //         console.log(body);
        //         if (error) {
        //             console.error(error);
        //             return;
        //         } else {
        //             // Grab the content-type header for the data URI
        //             const contentType = res.headers['content-type'];

        //             // Encode the raw body as a base64 string
        //             const base64Body = body.toString("base64");

        //             // Construct a Data URI for the image
        //             const base64DataUri = 'data:' + contentType + ';base64,' + base64Body;

        //             // Assign your values to the photoResponse object
        //             let photoResponse;
        //             photoResponse.res = [
        //                 {
        //                     '@odata.type': '#microsoft.graph.fileAttachment',
        //                     contentBytes: base64Body,
        //                     contentLocation: optionsPhoto.url,
        //                     isinline: true,
        //                     Name: 'mypic.jpg'
        //                 }
        //             ];
        //             photoResponse.ContentType = contentType;
        //             photoResponse.Base64string = base64DataUri;
        //         }
        //     });
    }
}

exports.SimpleGraphClient = SimpleGraphClient;
