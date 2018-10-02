// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { Client, GraphError } from "@microsoft/microsoft-graph-client";
import { User } from "@microsoft/microsoft-graph-types";

export class GraphClient {
    private readonly _token: string;

    constructor(token: string) {
        this._token = token;
    }

    public getMe(): Promise<User> {
        return new Promise((resolve, reject) => {
            const client = this.getAuthenticatedClient();
            client.api("/me").select("displayName")
            .get((err: GraphError, res: User) => {
                if (err) { return reject(err); }
                return resolve(res);
            });
        });
    }

    private getAuthenticatedClient(): Client {
        return Client.init({
            authProvider: (done) => {
                done(null, this._token);
            },
        });
    }
}
