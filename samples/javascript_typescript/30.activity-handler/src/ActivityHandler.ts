// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
import { ActivityTypes, TurnContext } from 'botbuilder';

export type BotHandler = (context: TurnContext, next: () => Promise<void>) => Promise<void>;

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
export class ActivityHandler {
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
     * Receives all ContactRelationUpdate activities
     * @remarks
     * Contact relation update activities signal a change in the relationship between the recipient and a user within the channel.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onContactRelationUpdate(handler: BotHandler): this {
        return this.on('ContactRelationUpdate', handler);
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
    public oMembersAdded(handler: BotHandler): this {
        return this.on('MembersAdded', handler);
    }

    /**
     * Receives only ConversationUpdate activities representing members being removed.
     * @remarks
     * context.activity.membersRemoved will include at least one entry.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public oMembersRemoved(handler: BotHandler): this {
        return this.on('MembersRemoved', handler);
    }

    /**
     * Receives all EndOfConversation activities.
     * @remarks
     * End of conversation activities signal the end of a conversation from the recipient's perspective.
     * This may be because the conversation has been completely ended, or because the recipient has been
     * removed from the conversation in a way that is indistinguishable from it ending.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onEndOfConversation(handler: BotHandler): this {
        return this.on('EndOfConversation', handler);
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
     * Receives all Invoke activities.
     * @remarks
     * Invoke activities are the synchronous counterpart to event activities.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onInvoke(handler: BotHandler): this {
        return this.on('Invoke', handler);
    }

    public onInstallationUpdate(handler: BotHandler): this {
        return this.on('InstallationUpdate', handler);
    }

    public onMessageDelete(handler: BotHandler): this {
        return this.on('MessageDelete', handler);
    }

    public onMessageUpdate(handler: BotHandler): this {
        return this.on('MessageUpdate', handler);
    }

    public onMessageReaction(handler: BotHandler): this {
        return this.on('MessageReaction', handler);
    }

    public onMessageReactionAdded(handler: BotHandler): this {
        return this.on('MessageReactionAdded', handler);
    }

    public onMessageReactionRemoved(handler: BotHandler): this {
        return this.on('MessageReactionRemoved', handler);
    }

    public onTyping(handler: BotHandler): this {
        return this.on('Typing', handler);
    }

    public onHandoff(handler: BotHandler): this {
        return this.on('Handoff', handler);
    }

    public onCreateConversation(handler: BotHandler): this {
        return this.on('CreateConversation', handler);
    }

   public onContinueConversation(handler: BotHandler): this {
        return this.on('ContinueConversation', handler);
   }

    public onUnrecognizedActivityType(handler: BotHandler): this {
        return this.on('UnrecognizedActivityType', handler);
    }

    // Fires last, and can be used to manage dialogs
    public onDialog(handler: BotHandler): this {
        return this.on('Dialog', handler);
    }

    public async run(context: TurnContext): Promise<void> {

        // Allow the dialog system to be triggered at the end of the chain
        const runDialogs = async (): Promise<void> => {
            await this.handle(context, 'Dialog', async () => {
                // noop
            });
        };

        // List of all Activity Types:
        // https://github.com/Microsoft/botbuilder-js/blob/master/libraries/botframework-schema/src/index.ts#L1627
        // TODO: Implement handlers for all valid activity types
        await this.handle(context, 'Turn', async () => {
            switch (context.activity.type) {
                case ActivityTypes.Message:
                    await this.handle(context, 'Message', runDialogs);
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    await this.handle(context, 'ContactRelationUpdate', runDialogs);
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
                case ActivityTypes.EndOfConversation:
                    await this.handle(context, 'EndOfConversation', runDialogs);
                    break;
                case ActivityTypes.Event:
                    await this.handle(context, 'Event', async () => {
                        if (context.activity.name === 'createConversation') {
                            await this.handle(context, 'CreateConversation', runDialogs);
                        } else if (context.activity.name === 'continueConversation') {
                            await this.handle(context, 'ContinueConversation', runDialogs);
                        } else {
                            await runDialogs();
                        }
                    });
                    break;
                case ActivityTypes.Invoke:
                    await this.handle(context, 'Invoke', runDialogs);
                    break;
                case ActivityTypes.InstallationUpdate:
                    await this.handle(context, 'InstallationUpdate', runDialogs);
                    break;
                case ActivityTypes.MessageDelete:
                    await this.handle(context, 'MessageDelete', runDialogs);
                    break;
                case ActivityTypes.MessageUpdate:
                    await this.handle(context, 'MessageUpdate', runDialogs);
                    break;
                case ActivityTypes.MessageReaction:
                    await this.handle(context, 'MessageReaction', runDialogs);
                    if (context.activity.reactionsAdded && context.activity.reactionsAdded.length) {
                        await this.handle(context, 'MessageReactionAdded', runDialogs);
                    }
                    if (context.activity.reactionsRemoved && context.activity.reactionsRemoved.length) {
                        await this.handle(context, 'MessageReactionRemoved', runDialogs);
                    }
                    break;
                case ActivityTypes.Typing:
                    await this.handle(context, 'Typing', runDialogs);
                    break;
                case ActivityTypes.Handoff:
                    await this.handle(context, 'Handoff', runDialogs);
                    break;
                default:
                    // handler for unknown or unhandled types
                    await this.handle(context, 'UnrecognizedActivityType', runDialogs);
                    break;
            }
        });
    }

    private on(type: string, handler: BotHandler) {
        if (!this.handlers[type]) {
            this.handlers[type] = [handler];
        } else {
            this.handlers[type].push(handler);
        }
        return this;
    }

    private async handle(context: TurnContext, type: string,  onNext: () => Promise<void>): Promise<void> {
        async function runHandler(index: number): Promise<void> {
            if (index < handlers.length) {
                await handlers[index](context, () => runHandler(index + 1));
            } else {
                await onNext();
            }
        }

        const handlers = this.handlers[type] || [];
        await runHandler(0);
    }

}
