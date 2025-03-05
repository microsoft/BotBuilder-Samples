// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BotState, CardFactory } from 'botbuilder';
import { Dialog, DialogState } from 'botbuilder-dialogs';
import { MainDialog } from '../dialogs/mainDialog';
import { DialogBot } from './dialogBot';

const WelcomeCard = require('../../resources/welcomeCard.json');

export class DialogAndWelcomeBot extends DialogBot {
    constructor(conversationState: BotState, userState: BotState, dialog: Dialog) {
        super(conversationState, userState, dialog);

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (const member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    const welcomeCard = CardFactory.adaptiveCard(WelcomeCard);
                    await context.sendActivity({ attachments: [welcomeCard] });
                    await (dialog as MainDialog).run(context, conversationState.createProperty<DialogState>('DialogState'));
                }
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}
