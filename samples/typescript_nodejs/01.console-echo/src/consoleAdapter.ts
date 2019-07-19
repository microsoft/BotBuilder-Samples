/**
 * @module botbuilder
 */
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
import { Activity, ActivityTypes, BotAdapter, ConversationReference, ResourceResponse, TurnContext } from 'botbuilder-core';
import * as readline from 'readline';

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
export class ConsoleAdapter extends BotAdapter {
    private nextId: number = 0;
    private readonly reference: ConversationReference;

    /**
     * Creates a new ConsoleAdapter instance.
     * @param reference (Optional) reference used to customize the address information of activites sent from the adapter.
     */
    constructor(reference?: ConversationReference) {
        super();
        this.reference = {
            bot: { id: 'bot', name: 'Bot' },
            channelId: 'console',
            conversation:  { id: 'convo1', name: '', isGroup: false },
            serviceUrl: '',
            user: { id: 'user', name: 'User1' },
            ...reference
        } as ConversationReference;
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
    public listen(logic: (context: TurnContext) => Promise<void>): () => void {
        const rl: readline.ReadLine = this.createInterface({ input: process.stdin, output: process.stdout, terminal: false });
        rl.on('line', (line: string) => {
            // Initialize activity
            const activity: Partial<Activity> = TurnContext.applyConversationReference(
                {
                    id: (this.nextId++).toString(),
                    text: line,
                    timestamp: new Date(),
                    type: ActivityTypes.Message
                },
                this.reference,
                true
            );

            // Create context and run middleware pipe
            const context: TurnContext = new TurnContext(this, activity);
            this.runMiddleware(context, logic)
                .catch((err: Error) => { this.printError(err.toString()); });
        });

        return (): void => {
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
    public continueConversation(reference: ConversationReference, logic: (context: TurnContext) => Promise<void>): Promise<void> {
            // Create context and run middleware pipe
            const activity: Partial<Activity> = TurnContext.applyConversationReference({}, reference, true);
            const context: TurnContext = new TurnContext(this, activity);

            return this.runMiddleware(context, logic)
                       .catch((err: Error) => { this.printError(err.toString()); });
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
    public sendActivities(context: TurnContext, activities: Array <Partial<Activity>>): Promise<ResourceResponse[]> {
        const that: ConsoleAdapter = this;

        // tslint:disable-next-line:promise-must-complete
        return new Promise((resolve: any, reject: any): void => {
            const responses: ResourceResponse[] = [];
            function next(i: number): void {
                if (i < activities.length) {
                    responses.push({} as ResourceResponse);
                    const a: Partial<Activity> = activities[i];
                    switch (a.type) {
                        case 'delay' as ActivityTypes:
                            setTimeout(() => next(i + 1), a.value);
                            break;
                        case ActivityTypes.Message:
                            if (a.attachments && a.attachments.length > 0) {
                                const append: string = a.attachments.length === 1
                                    ? `(1 attachment)` : `(${a.attachments.length} attachments)`;
                                that.print(`${a.text} ${append}`);
                            } else {
                                that.print(a.text || '');
                            }
                            next(i + 1);
                            break;
                        default:
                            that.print(`[${a.type}]`);
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
    public updateActivity(context: TurnContext, activity: Partial<Activity>): Promise<void> {
        return Promise.reject(new Error(`ConsoleAdapter.updateActivity(): not supported.`));
    }

    /**
     * Not supported for the ConsoleAdapter.  Calling this method or `TurnContext.deleteActivity()`
     * will result an error being returned.
     */
    public deleteActivity(context: TurnContext, reference: Partial<ConversationReference>): Promise<void> {
        return Promise.reject(new Error(`ConsoleAdapter.deleteActivity(): not supported.`));
    }

    /**
     * Allows for mocking of the console interface in unit tests.
     * @param options Console interface options.
     */
    protected createInterface(options: readline.ReadLineOptions): readline.ReadLine {
        return readline.createInterface(options);
    }

    /**
     * Logs text to the console.
     * @param line Text to print.
     */
    protected print(line: string): void {
        console.log(line);
    }

    /**
     * Logs an error to the console.
     * @param line Error text to print.
     */
    protected printError(line: string): void {
        console.error(line);
    }
}
