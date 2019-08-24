/**
 * @module botbuilder-teams
 */
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { ActivityTypes, BotHandler, InvokeResponse, TurnContext } from 'botbuilder';

// NOTE:
// Due to ActivityHandler.on() and ActivityHandler.handle() being private methods, it is not
// trivial to extend the ActivityHandler class from `botbuilder-core`.
//
// As such, the current TeamsActivityHandler is a mirror of the ActivityHandler implementation
// with additional support for Teams specific Activities.

/**
 * InvokeActivityHandlers are used to handle Invoke Activities from the Microsoft Teams channel.
 * The handlers are wrapped in lambda, which will send the InvokeResponse to Teams if a handler returns an InvokeResponse.
 * ```javascript
 * const fileConsentHandler = async (context, next) => {
 *      // do something that returns an InvokeResponse
 *      // then `await next()` to continue processing
 *      const invokeResponse = await processFileConsent();
 *      await next();
 *      return invokeResponse;
 * }
 * 
 * bot.onAcceptFileConsent(fileConsentHandler);
 * ```
 */
export declare type InvokeActivityHandler = (context: TurnContext, next: () => Promise<void>) => Promise<InvokeResponse>;

/**
 * Event-emitting base class bots.
 *
 * @remarks
 * This provides an extensible base class for handling incoming
 * activities in an event-driven way.  Developers may bind one or
 * more handlers for each type of event.
 *
 * To bind a handler to an event, use the `on<Event>()` method, for example:
 *
 * ```Javascript
 * bot.onMessage(async (context, next) => {
    * // do something
    * // then `await next()` to continue processing
    * await next();
    * });
    * ```
    *
    * A series of events will be emitted while the activity is being processed.
    * Handlers can stop the propagation of the event by omitting a call to `next()`.
    *
    * * Turn - emitted for every activity
    * * Type-specific - an event, based on activity.type
    * * Sub-type - any specialized events, based on activity content
    * * Dialog - the final event, used for processing Dialog actions
    *
    * A simple implementation:
    * ```Javascript
    * const bot = new ActivityHandler();
    *
    *  server.post('/api/messages', (req, res) => {
    *      adapter.processActivity(req, res, async (context) => {
    *          // Route to main dialog.
    *          await bot.run(context);
    *      });
    * });
    *
    * bot.onMessage(async (context, next) => {
    *      // do stuff
    *      await context.sendActivity(`Echo: ${ context.activity.text }`);
    *      // proceed with further processing
    *      await next();
    * });
    * ```
    */
export class TeamsActivityHandler {
    private readonly handlers: {[type: string]: BotHandler[]} = {};

    /**
     * Bind a handler to the Turn event that is fired for every incoming activity, regardless of type
     * @remarks
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onTurn(handler: BotHandler): this {
        return this.on('Turn', handler);
    }

    /**
     * Receives all incoming Message activities
     * @remarks
     * Message activities represent content intended to be shown within a conversational interface.
     * Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
     * Note that while most messages do contain text, this field is not always present!
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessage(handler: BotHandler): this {
        return this.on('Message', handler);
    }

    /**
     * Receives all ConversationUpdate activities, regardless of whether members were added or removed
     * @remarks
     * Conversation update activities describe a change in a conversation's members, description, existence, or otherwise.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onConversationUpdate(handler: BotHandler): this {
        return this.on('ConversationUpdate', handler);
    }

    /**
     * Receives only ConversationUpdate activities representing members being added.
     * @remarks
     * context.activity.membersAdded will include at least one entry.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMembersAdded(handler: BotHandler): this {
        return this.on('MembersAdded', handler);
    }

    /**
     * Receives only ConversationUpdate activities representing members being removed.
     * @remarks
     * context.activity.membersRemoved will include at least one entry.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMembersRemoved(handler: BotHandler): this {
        return this.on('MembersRemoved', handler);
    }

    /**
     * Receives only MessageReaction activities, regardless of whether message reactions were added or removed
     * @remarks
     * MessageReaction activities are sent to the bot when a message reacion, such as 'like' or 'sad' are
     * associated with an activity previously sent from the bot.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessageReaction(handler: BotHandler): this {
        return this.on('MessageReaction', handler);
    }

    /**
     * Receives only MessageReaction activities representing message reactions being added.
     * @remarks
     * context.activity.reactionsAdded will include at least one entry.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onReactionsAdded(handler: BotHandler): this {
        return this.on('ReactionsAdded', handler);
    }

    /**
     * Receives only MessageReaction activities representing message reactions being removed.
     * @remarks
     * context.activity.reactionsRemoved will include at least one entry.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onReactionsRemoved(handler: BotHandler): this {
        return this.on('ReactionsRemoved', handler);
    }

    /**
     * Receives all Event activities.
     * @remarks
     * Event activities communicate programmatic information from a client or channel to a bot.
     * The meaning of an event activity is defined by the `name` field.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onEvent(handler: BotHandler): this {
        return this.on('Event', handler);
    }

    /**
     * Receives event activities of type 'tokens/response'
     * @remarks
     * These events occur during the oauth flow
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onTokenResponseEvent(handler: BotHandler): this {
        return this.on('TokenResponseEvent', handler);
    }

    /**
     * UnrecognizedActivityType will fire if an activity is received with a type that has not previously been defined.
     * @remarks
     * Some channels or custom adapters may create Actitivies with different, "unofficial" types.
     * These events will be passed through as UnrecognizedActivityType events.
     * Check `context.activity.type` for the type value.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onUnrecognizedActivityType(handler: BotHandler): this {
        return this.on('UnrecognizedActivityType', handler);
    }

    /**
     * onDialog fires at the end of the event emission process, and should be used to handle Dialog activity.
     * @remarks
     * Sample code:
     * ```javascript
     * bot.onDialog(async (context, next) => {
     *      if (context.activity.type === ActivityTypes.Message) {
     *          const dialogContext = await dialogSet.createContext(context);
     *          const results = await dialogContext.continueDialog();
     *          await conversationState.saveChanges(context);
     *      }
     *
     *      await next();
     * });
     * ```
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onDialog(handler: BotHandler): this {
        return this.on('Dialog', handler);
    }

    /**
     * Receives invoke activities with Activity name of 'fileConsent/invoke'
     * @remarks
     * This type of invoke activity occur during the File Consent flow.
     * For more information, see:
     * https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-files#message-requesting-permission-to-upload
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... }
     */
    public onAcceptFileConsent(handler: InvokeActivityHandler): this {
        return this.on('AcceptFileConsent', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onAcceptFileConsent', context, next);
        });
    }

    /**
     * Receives invoke activities with Activity name of 'fileConsent/invoke'
     * @remarks
     * This type of invoke activity occur during the File Consent flow.
     * For more information, see:
     * https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-files#message-requesting-permission-to-upload
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... }
     */
    public onDeclineFileConsent(handler: InvokeActivityHandler): this {
        return this.on('DeclineFileConsent', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onDeclineFileConsent', context, next);
        });
    }

    /**
     * Receives invoke activities with Activity name of 'actionableMessage/executeAction'
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public on365CardAction(handler: InvokeActivityHandler): this {
        return this.on('365CardAction', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'on365CardAction', context, next);
        });
    }

    /**
     * Receives invoke activities with Activity name of 'signin/verifyState'
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public onSigninStateVerification(handler: InvokeActivityHandler): this {
        return this.on('SigninStateVerification', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onSigninStateVerification', context, next);
        })
    }

    /**
     * Receives invoke activities with Activity name of 'composeExtension/queryLink'
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public onAppBasedLinkQuery(handler: InvokeActivityHandler): this {
        return this.on('AppBasedLinkQuery', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onAppBasedLinkQuery', context, next);
        })
    }

    /**
     * Receives invoke activities with Activity name of 'task/fetch'
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public onTaskModuleFetch(handler: InvokeActivityHandler): this {
        return this.on('TaskModuleFetch', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onTaskModuleFetch', context, next);
        })
    }

    /**
     * Receives invoke activities with Activity name of 'task/submit'
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public onTaskModuleSubmit(handler: InvokeActivityHandler): this {
        return this.on('TaskModuleSubmit', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onTaskModuleSubmit', context, next);
        })
    }

    /**
     * Receives invoke activities with the name 'composeExtension/query'.
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public onMessagingExtensionQuery(handler: InvokeActivityHandler): this {
        return this.on('ComposeExtension/Query', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onMessagingExtensionQuery', context, next);
        });
    }

    /**
     * Receives invoke activities with the name 'composeExtension/submitAction'.
     * @remarks
     * This invoke activity is received when a user 
     * @param handler InvokeActivityHandler a handler in the form async(context, next): Promise<InvokeResponse> => { ... } 
     */
    public onMessagingExtensionSubmit(handler: InvokeActivityHandler): this {
        return this.on('ComposeExtension/SubmitAction', async (context, next) => {
            await TeamsActivityHandler.teamsInvokeWrapper(handler, 'onMessagingExtensionSubmit', context, next);
        });
    }

    /**
     * `run()` is the main "activity handler" function used to ingest activities into the event emission process.
     * @remarks
     * Sample code:
     * ```javascript
     *  server.post('/api/messages', (req, res) => {
     *      adapter.processActivity(req, res, async (context) => {
     *          // Route to main dialog.
     *          await bot.run(context);
     *      });
     * });
     * ```
     *
     * @param context TurnContext A TurnContext representing an incoming Activity from an Adapter
     */
    public async run(context: TurnContext): Promise<void> {

        if (!context) {
            throw new Error(`Missing TurnContext parameter`);
        }

        if (!context.activity) {
            throw new Error(`TurnContext does not include an activity`);
        }

        if (!context.activity.type) {
            throw new Error(`Activity is missing it's type`);
        }
        
        // Allow the dialog system to be triggered at the end of the chain
        const runDialogs = async (): Promise<void> => {
            await this.handle(context, 'Dialog', async () => {
                // noop
            });
        };

        // List of all Activity Types:
        // https://github.com/Microsoft/botbuilder-js/blob/master/libraries/botframework-schema/src/index.ts#L1627
        await this.handle(context, 'Turn', async () => {
            switch (context.activity.type) {
                case ActivityTypes.Message:
                    await this.handle(context, 'Message', runDialogs);
                    break;
                case ActivityTypes.ConversationUpdate:
                    await this.handle(context, 'ConversationUpdate', async () => {
                        if (context.activity.membersAdded && context.activity.membersAdded.length > 0) {
                            await this.handle(context, 'MembersAdded', runDialogs);
                        } else if (context.activity.membersRemoved && context.activity.membersRemoved.length > 0) {
                            await this.handle(context, 'MembersRemoved', runDialogs);
                        } else {
                            await runDialogs();
                        }
                    });
                    break;
                case ActivityTypes.MessageReaction:
                    await this.handle(context, 'MessageReaction', async () => {
                        if (context.activity.reactionsAdded && context.activity.reactionsAdded.length > 0) {
                            await this.handle(context, 'ReactionsAdded', runDialogs);
                        } else if (context.activity.reactionsRemoved && context.activity.reactionsRemoved.length > 0) {
                            await this.handle(context, 'ReactionsRemoved', runDialogs);
                        } else {
                            await runDialogs();
                        }
                    });
                    break;
                case ActivityTypes.Event:
                    await this.handle(context, 'Event', async () => {
                        if (context.activity.name === 'tokens/response') {
                            await this.handle(context, 'TokenResponseEvent', runDialogs);
                        } else {
                            await runDialogs();
                        }
                    });
                    break;
                case ActivityTypes.Invoke:
                    let invokeResponse: InvokeResponse;
                    switch (context.activity.name) {
                        case 'fileConsent/invoke':
                            if (context.activity.value.action === 'accept') {
                                invokeResponse = await this.handle(context, 'AcceptFileConsent', runDialogs);
                            } else {
                                invokeResponse = await this.handle(context, 'DeclineFileConsent', runDialogs);
                            }
                            break;
                        case 'composeExtension/query':
                            await this.handle(context, 'ComposeExtension/Query', runDialogs);
                            break;
                        case 'composeExtension/submitAction':
                            await this.handle(context, 'ComposeExtension/SubmitAction', runDialogs);
                            break;
                        case 'actionableMessage/executeAction':
                            await this.handle(context, '365CardAction', runDialogs);
                            break;
                        case 'task/fetch':
                            await this.handle(context, 'TaskModuleFetch', runDialogs);
                            break;
                        case 'task/submit':
                            await this.handle(context, 'TaskModuleSubmit', runDialogs);
                            break;
                        case 'composeExtension/queryLink':
                            await this.handle(context, 'AppBasedLinkQuery', runDialogs);
                            break;
                        default:
                            // Correct behavior to be determined.
                            return await runDialogs();
                        }
                    if (invokeResponse) {
                        await context.sendActivity({ value: invokeResponse, type: 'invokeResponse' });
                    }
                    break;
                default:
                // handler for unknown or unhandled types
                    await this.handle(context, 'UnrecognizedActivityType', runDialogs);
                    break;
            }
        });
    }

    /**
     * Private method used to bind handlers to events by name
     * @param type string
     * @param handler BotHandler
     */
    private on(type: string, handler: BotHandler) {
        if (!this.handlers[type]) {
            this.handlers[type] = [handler];
        } else {
            this.handlers[type].push(handler);
        }
        return this;
    }

    /**
     * Private method used to fire events and execute any bound handlers
     * @param type string
     * @param handler BotHandler
     */
    private async handle(context: TurnContext, type: string,  onNext: () => Promise<void>): Promise<any> {
        let returnValue: any = null;

        async function runHandler(index: number): Promise<void> {
            if (index < handlers.length) {
                const val = await handlers[index](context, () => runHandler(index + 1));
                // if a value is returned, and we have not yet set the return value,
                // capture it.  This is used to allow InvokeResponses to be returned.
                if (typeof(val) !== 'undefined' && returnValue === null) {
                    returnValue = val;
                }
            } else {
                const val = await onNext();
                if (typeof(val) !== 'undefined') {
                    returnValue = val;
                }
            }
        }

        const handlers = this.handlers[type] || [];
        await runHandler(0);

        return returnValue;
    }

    /**
     * Private method that sends the InvokeResponse from InvokeActivityHandlers
     * @param handler 
     * @param context 
     * @param next 
     */
    protected static async teamsInvokeWrapper(handler: InvokeActivityHandler, handlerName: string, context: TurnContext, next: () => Promise<void>): Promise<void> {
        const invokeResponse = await handler(context, next);
        if (invokeResponse) {
            await context.sendActivity({ value: invokeResponse, type: 'invokeResponse' });
        } else {
            throw new Error(`TeamsActivityHandler.teamsInvokeWrapper(): InvokeResponse not returned from "${handlerName}" handler.`);
        }
    }

}
