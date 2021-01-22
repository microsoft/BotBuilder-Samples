// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { Client } = require("@microsoft/microsoft-graph-client");

/**
 * This class is a wrapper for the Microsoft Graph API.
 * See: https://developer.microsoft.com/en-us/graph for more information.
 */
class SimpleGraphClient {
    constructor(token) {
        if (!token || !token.trim()) {
            throw new Error("SimpleGraphClient: Invalid token received.");
        }

        this._token = token;

        // Get an Authenticated Microsoft Graph client using the token issued to the user.
        this.graphClient = Client.init({
            authProvider: (done) => {
                done(null, this._token); // First parameter takes an error if you can't get an access token.
            },
        });
    }

    /**
     *
     * @param {string} searchQuery text to search the inbox for.
     */
    async searchMailInbox(searchQuery) {
        // Searches the user's mail Inbox using the Microsoft Graph API
        return await this.graphClient
            .api("me/mailfolders/inbox/messages")
            .search(searchQuery)
            .get();
    }
    async GetMyProfile() {
        return await this.graphClient.api("/me").get();
    }
    // async GetPhoto() {
    //     return await this.graphClient.api("/me/photo/$value").get();
    // }
}

exports.SimpleGraphClient = SimpleGraphClient;
