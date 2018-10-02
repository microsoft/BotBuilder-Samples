// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { User } from "@microsoft/microsoft-graph-types";
import { TokenResponse, TurnContext } from "botbuilder";
import { ComponentDialog, DialogTurnResult, OAuthPrompt, WaterfallDialog, WaterfallStepContext } from "botbuilder-dialogs";
import { GraphClient } from "../../serviceClients/graphClient";
import { SignInResponses } from "./signInResponses";

export class SignInDialog extends ComponentDialog {
    private readonly _loginPrompt: string = "loginPrompt";
    private readonly _connectionName: string;
    private readonly _responder: SignInResponses;

    constructor(connectionName: string) {
        super("SignInDialog");
        this.initialDialogId = "SignInDialog";
        this._connectionName = connectionName;
        this._responder = new SignInResponses();

        this.addDialog(new WaterfallDialog(this.initialDialogId, [
            this.askToLogin.bind(this),
            this.finishAuthDialog.bind(this),
        ]));
        this.addDialog(new OAuthPrompt(this._loginPrompt, {
            connectionName: this._connectionName,
            text: "Please sign in to access this bot.",
            title: "Sign In",
        }));
    }

    private askToLogin(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        return sc.prompt(this._loginPrompt, {});
    }

    private async finishAuthDialog(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        if (sc.result) {
            const tokenResponse: TokenResponse = sc.result;

            if (tokenResponse.token) {
                const user = await this.getProfile(sc.context, tokenResponse);
                await this._responder.replyWith(sc.context, SignInResponses.Succeeded, { name: user.displayName });
                return await sc.endDialog(tokenResponse);
            }

        } else {
            await this._responder.replyWith(sc.context, SignInResponses.Failed);
        }

        return await sc.endDialog();
    }

    private getProfile(context: TurnContext, tokenResponse: TokenResponse): Promise<User> {
        const client: GraphClient = new GraphClient(tokenResponse.token);
        return client.getMe();
    }
}
