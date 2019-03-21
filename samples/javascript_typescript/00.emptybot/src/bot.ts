// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityHandler } from 'botbuilder';

export class MyBot extends ActivityHandler {
    constructor() {
        super();
        this.onMembersAdded(async context => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello world!');
                }
            }
        });
    }
}
