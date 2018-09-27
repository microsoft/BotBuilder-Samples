import { ComponentDialog, WaterfallDialog, WaterfallStepContext, DialogTurnResult, OAuthPrompt } from "botbuilder-dialogs"
import { TokenResponse, TurnContext } from "botbuilder";
import { SignInResponses } from "./signInResponses";
import { GraphClient } from "../../serviceClients/graphClient";
import { User } from "@microsoft/microsoft-graph-types";

export class SignInDialog extends ComponentDialog {
    private readonly loginPrompt: string = 'loginPrompt';
    private readonly _connectionName: string;
    private readonly _responder: SignInResponses;

    constructor(connectionName: string) {
        super('SignInDialog');
        this._connectionName = connectionName;
        this.initialDialogId = 'SignInDialog';
        this._responder = new SignInResponses();

        this.addDialog(new WaterfallDialog(this.initialDialogId, [this.askToLogin, this.finishAuthDialog]));
        this.addDialog(new OAuthPrompt(this.loginPrompt, {
            connectionName: this._connectionName,
            title: 'Sign In',
            text: 'Please sign in to access this bot.'
        }));
    }

    private askToLogin(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        return sc.prompt(this.loginPrompt, {});
    }

    private async finishAuthDialog(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        if (sc.result) {
            const tokenResponse: TokenResponse = sc.result;

            if (tokenResponse.token) {
                const user = await this.getProfile(sc.context, tokenResponse);
                await this._responder.ReplyWith(sc.context, SignInResponses.Succeeded, { name: user.displayName });
                return await sc.endDialog(tokenResponse);
            }

        } else {
            await this._responder.ReplyWith(sc.context, SignInResponses.Failed);
        }
        
        return await sc.endDialog();
    }

    private getProfile(context: TurnContext, tokenResponse: TokenResponse): Promise<User> {
        const client: GraphClient = new GraphClient(tokenResponse.token);
        return client.getMe();
    }
}