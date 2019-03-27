// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityHandler } from 'botbuilder';

export class MyBot extends ActivityHandler {
    constructor() {
        super();
        this.onMembersAdded(async (context) => {
            const membersAdded = context.activity.membersAdded;
            for (const member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello world!');
                }
            }
        });
    }
}
