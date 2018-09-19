// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ConnectionStatus } from 'botframework-webchat';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { BotAdapter, TurnContext } from 'botbuilder-core';

/**
 * Custom BotAdapter used for deploying a bot in a browser.
 */
export class WebChatAdapter extends BotAdapter {
    constructor() {
        super();
        this.activity$ = new Subject();
        this.botConnection = {
            connectionStatus$: new BehaviorSubject(ConnectionStatus.Online),
            activity$: this.activity$.share(),
            end() {
                debugger
            },
            postActivity: activity => {
                const id = Date.now().toString();
                return Observable.fromPromise(this
                    .onReceive(Object.assign({}, activity, {
                        id,
                        conversation: { id: 'bot' },
                        channelId: 'WebChat'
                    }))
                    .then(() => id)
                )
            }
        }
    }

    /**
     * This WebChatAdapter implements the sendActivities method which is called by the TurnContext class.
     * It's also possible to write a custom TurnContext with different methods of accessing an adapter.
     * @param {TurnContext} context
     * @param {Activity[]} activities
     */
    sendActivities(context, activities) {
        console.log(Date.now().toString());
        const sentActivities = activities.map(activity => Object.assign({}, activity, {
            id: Date.now().toString(),
            channelId: 'WebChat',
            conversation: { id: 'bot' },
            from: { id: 'bot' },
            timestamp: Date.now()
        }));

        sentActivities.forEach(activity => this.activity$.next(activity));

        return Promise.resolve(sentActivities.map(activity => {
            id: activity.id
        }));
    }    
    
    /**
     * Registers the business logic for the adapter, it takes a handler that takes a TurnContext object as a parameter.
     * @param {function} logic The driver code of the developer's bot application. This code receives and responds to user messages.
    */
    processActivity(logic) {
        this.logic = logic;
        return this;
    }

    /**
     * Runs the bot's middleware pipeline in addition to any business logic, if `this.logic` is found.
     * @param {Activity} activity 
     */
    onReceive(activity) {
        const context = new TurnContext(this, activity);

        // Runs the middleware pipeline followed by any registered business logic.
        // If no business logic has been registered via processActivity, a default
        // value is provided as to not break the bot.
        return this.runMiddleware(context, this.logic || function () { });
    }
}
