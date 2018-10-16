// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ConversationState, TurnContext, UserState } from "botbuilder";
import { DialogSet, DialogState, DialogTurnStatus } from "botbuilder-dialogs";
import { BotServices } from "./botServices";
import { MainDialog } from "./dialogs/main/mainDialog";

/**
 * Main entry point and orchestration for bot.
 */
export class EnterpriseBot {
    private readonly _botServices: BotServices;
    private readonly _conversationState: ConversationState;
    private readonly _userState: UserState;
    private readonly _dialogs: DialogSet;

    /**
     * Constructs the three pieces necessary for this bot to operate.
     * @constructor
     * @param {BotServices} botServices Services included in the bot (LUIS, QnA, Dispatcher, etc...).
     * @param {ConversationState} conversationState Bot conversation state.
     * @param {UserState} userState Bot user state.
     */
    constructor(botServices: BotServices, conversationState: ConversationState, userState: UserState) {
        if (!botServices) { throw new Error(("Missing parameter.  botServices is required")); }
        if (!conversationState) { throw new Error(("Missing parameter.  conversationState is required")); }
        if (!userState) { throw new Error(("Missing parameter.  userState is required")); }

        this._botServices = botServices;
        this._conversationState = conversationState;
        this._userState = userState;

        this._dialogs = new DialogSet(this._conversationState.createProperty<DialogState>("EnterpriseBot"));
        this._dialogs.add(new MainDialog(this._botServices, this._conversationState, this._userState));
    }

    /**
     * Run every turn of the conversation. Handles orchestration of messages.
     * @param {TurnContext} turnContext Turn context from the adapter.
     */
    public async onTurn(turnContext: TurnContext): Promise<void> {
        const dc = await this._dialogs.createContext(turnContext);
        const result = await dc.continueDialog();

        if (result.status === DialogTurnStatus.empty) {
            await dc.beginDialog("MainDialog");
        }
    }
}
