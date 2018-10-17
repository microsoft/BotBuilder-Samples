'use strict';
var __importStar = (this && this.__importStar) || function(mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result['default'] = mod;
    return result;
};
Object.defineProperty(exports, '__esModule', { value: true });

/**
 * @module botbuilder
 */
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
const botbuilderCore = require('botbuilder-core');
const readline = __importStar(require('readline'));
const console = require('console');

/**
 * Lets a user communicate with a bot from a console window.
 *
 * @remarks
 * The following example shows the typical adapter setup:
 *
 *
 * ```JavaScript
 * const { ConsoleAdapter } = require('botbuilder');
 *
 * const adapter = new ConsoleAdapter();
 * const closeFn = adapter.listen(async (context) => {
 *    await context.sendActivity(`Hello World`);
 * });
 * ```
 */
class ConsoleAdapter extends botbuilderCore.BotAdapter {
    /**
     * Creates a new ConsoleAdapter instance.
     * @param reference (Optional) reference used to customize the address information of activities sent from the adapter.
     */
    constructor(reference) {
        super();
        this.nextId = 0;
        this.reference = Object.assign({ channelId: 'console', user: { id: 'user', name: 'User1' }, bot: { id: 'bot', name: 'Bot' }, conversation: { id: 'convo1', name: '', isGroup: false }, serviceUrl: '' }, reference);
    }
    /**
     * Begins listening to console input. A function will be returned that can be used to stop the
     * bot listening and therefore end the process.
     *
     * @remarks
     * Upon receiving input from the console the flow is as follows:
     *
     * - An 'message' activity will be created containing the users input text.
     * - A revokable `TurnContext` will be created for the activity.
     * - The context will be routed through any middleware registered with [use()](#use).
     * - The bots logic handler that was passed in will be executed.
     * - The promise chain setup by the middleware stack will be resolved.
     * - The context object will be revoked and any future calls to its members will result in a
     *   `TypeError` being thrown.
     *
     * ```JavaScript
     * const closeFn = adapter.listen(async (context) => {
     *    const utterance = context.activity.text.toLowerCase();
     *    if (utterance.includes('goodbye')) {
     *       await context.sendActivity(`Ok... Goodbye`);
     *       closeFn();
     *    } else {
     *       await context.sendActivity(`Hello World`);
     *    }
     * });
     * ```
     * @param logic Function which will be called each time a message is input by the user.
     */
    listen(logic) {
        const rl = this.createInterface({ input: process.stdin, output: process.stdout, terminal: false });
        rl.on('line', (line) => {
            // Initialize activity
            const activity = botbuilderCore.TurnContext.applyConversationReference({
                type: botbuilderCore.ActivityTypes.Message,
                id: (this.nextId++).toString(),
                timestamp: new Date(),
                text: line
            }, this.reference, true);
            // Create context and run middleware pipe
            const context = new botbuilderCore.TurnContext(this, activity);
            this.runMiddleware(context, logic)
                .catch((err) => { this.printError(err.toString()); });
        });
        return () => {
            rl.close();
        };
    }
    /**
     * Lets a bot proactively message the user.
     *
     * @remarks
     * The processing steps for this method are very similar to [listen()](#listen)
     * in that a `TurnContext` will be created which is then routed through the adapters middleware
     * before calling the passed in logic handler. The key difference being that since an activity
     * wasn't actually received it has to be created.  The created activity will have its address
     * related fields populated but will have a `context.activity.type === undefined`.
     *
     * ```JavaScript
     * function delayedNotify(context, message, delay) {
     *    const reference = TurnContext.getConversationReference(context.activity);
     *    setTimeout(() => {
     *       adapter.continueConversation(reference, async (ctx) => {
     *          await ctx.sendActivity(message);
     *       });
     *    }, delay);
     * }
     * ```
     * @param reference A `ConversationReference` saved during a previous message from a user.  This can be calculated for any incoming activity using `TurnContext.getConversationReference(context.activity)`.
     * @param logic A function handler that will be called to perform the bots logic after the the adapters middleware has been run.
     */
    continueConversation(reference, logic) {
        // Create context and run middleware pipe
        const activity = botbuilderCore.TurnContext.applyConversationReference({}, reference, true);
        const context = new botbuilderCore.TurnContext(this, activity);
        return this.runMiddleware(context, logic)
            .catch((err) => { this.printError(err.toString()); });
    }
    /**
     * Logs a set of activities to the console.
     *
     * @remarks
     * Calling `TurnContext.sendActivities()` or `TurnContext.sendActivity()` is the preferred way of
     * sending activities as that will ensure that outgoing activities have been properly addressed
     * and that any interested middleware has been notified.
     * @param context Context for the current turn of conversation with the user.
     * @param activities List of activities to send.
     */
    sendActivities(context, activities) {
        const that = this;
        // tslint:disable-next-line:promise-must-complete
        return new Promise((resolve, reject) => {
            const responses = [];
            function next(i) {
                if (i < activities.length) {
                    responses.push({});
                    const a = activities[i];
                    switch (a.type) {
                    case 'delay':
                        setTimeout(() => next(i + 1), a.value);
                        break;
                    case botbuilderCore.ActivityTypes.Message:
                        if (a.attachments && a.attachments.length > 0) {
                            const append = a.attachments.length === 1
                                ? `(1 attachment)` : `(${ a.attachments.length } attachments)`;
                            that.print(`${ a.text } ${ append }`);
                        } else {
                            that.print(a.text || '');
                        }
                        next(i + 1);
                        break;
                    default:
                        that.print(`[${ a.type }]`);
                        next(i + 1);
                        break;
                    }
                } else {
                    resolve(responses);
                }
            }
            next(0);
        });
    }
    /**
     * Not supported for the ConsoleAdapter.  Calling this method or `TurnContext.updateActivity()`
     * will result an error being returned.
     */
    updateActivity(context, activity) {
        return Promise.reject(new Error(`ConsoleAdapter.updateActivity(): not supported.`));
    }
    /**
     * Not supported for the ConsoleAdapter.  Calling this method or `TurnContext.deleteActivity()`
     * will result an error being returned.
     */
    deleteActivity(context, reference) {
        return Promise.reject(new Error(`ConsoleAdapter.deleteActivity(): not supported.`));
    }
    /**
     * Allows for mocking of the console interface in unit tests.
     * @param options Console interface options.
     */
    createInterface(options) {
        return readline.createInterface(options);
    }
    /**
     * Logs text to the console.
     * @param line Text to print.
     */
    print(line) {
        console.log(line);
    }
    /**
     * Logs an error to the console.
     * @param line Error text to print.
     */
    printError(line) {
        console.error(line);
    }
}
exports.ConsoleAdapter = ConsoleAdapter;
