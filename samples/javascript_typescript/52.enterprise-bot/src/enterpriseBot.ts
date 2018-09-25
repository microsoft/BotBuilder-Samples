// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ConversationState, UserState, TurnContext } from 'botbuilder';
import { BotServices } from './botServices';
import { DialogSet, DialogTurnStatus } from 'botbuilder-dialogs';
import { MainDialog } from './dialogs/main/mainDialog';

/**
 * Demonstrates the following concepts:
 * 
 */
export class EnterpriseBot {
    private readonly _botServices: BotServices;
    private readonly _conversationState: ConversationState;
    private readonly _userState: UserState;
    private readonly _dialogs: DialogSet;
    /**
     * Constructs the three pieces necessary for this bot to operate:
     *
     * @param {ConversationState} conversationState property accessor
     * @param {UserState} userState property accessor
     * @param {BotConfiguration} botConfig contents of the .bot file
     */
    constructor(botServices: BotServices, conversationState: ConversationState, userState: UserState) {
        if (!botServices) throw ('Missing parameter.  botServices is required');
        if (!conversationState) throw ('Missing parameter.  conversationState is required');
        if (!userState) throw ('Missing parameter.  userState is required');

        this._botServices = botServices;
        this._conversationState = conversationState;
        this._userState = userState;
        this._dialogs = new DialogSet(this._conversationState.createProperty('EnterpriseBot'));
        this._dialogs.add(new MainDialog(this._botServices, this._conversationState, this._userState));
    }

    /**
     * Driver code that does one of the following:
     * 1. Nothing
     * @param {Context} context turn context from the adapter
     */
    public onTurn = async (context: TurnContext) => {
        const dc = await this._dialogs.createContext(context);
        const result = await dc.continueDialog();

        if (result.status === DialogTurnStatus.empty) {
            await dc.beginDialog('MainDialog');
        }
    }
};