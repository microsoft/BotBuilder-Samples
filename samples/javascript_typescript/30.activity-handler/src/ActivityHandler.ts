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

    /**
     * Receives all InstallationUpdate activities.
     * @remarks
     * Installation update activities represent an installation or uninstallation of a bot
     * within an organizational unit (such as a customer tenant or "team") of a channel.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onInstallationUpdate(handler: BotHandler): this {
        return this.on('InstallationUpdate', handler);
    }

    /**
     * Receives all MessageDelete activities.
     * @remarks
     * Message delete activities represent a deletion of an existing message activity within a conversation.
     * The deleted activity is referred to by the id and conversation fields within the activity.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessageDelete(handler: BotHandler): this {
        return this.on('MessageDelete', handler);
    }

    /**
     * Receives all MessageUpdate activities.
     * @remarks
     * Message update activities represent an update of an existing message activity within a conversation.
     * The updated activity is referred to by the id and conversation fields within the activity, and the
     * message update activity contains all fields in the revised message activity.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessageUpdate(handler: BotHandler): this {
        return this.on('MessageUpdate', handler);
    }

    /**
     * Receives all MessageReaction activities, regardless of the sub-type of event.
     * @remarks
     * Message reaction activities represent a social interaction on an existing message activity within a conversation.
     * The original activity is referred to by the id and conversation fields within the activity.
     * The from field represents the source of the reaction (i.e., the user that reacted to the message).
     *
     * See related events: onMessageReactionAdded and onMessageReactionRemoved
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessageReaction(handler: BotHandler): this {
        return this.on('MessageReaction', handler);
    }

    /**
     * Receives MessageReaction activities when a reaction is added
     * @remarks
     * The `context.activity.reactionsAdded` field contains a list of reactions added to this activity.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessageReactionAdded(handler: BotHandler): this {
        return this.on('MessageReactionAdded', handler);
    }

    /**
     * Receives MessageReaction activities when a reaction is removed
     * @remarks
     * The `context.activity.reactionsRemoved` field contains a list of reactions added from this activity.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onMessageReactionRemoved(handler: BotHandler): this {
        return this.on('MessageReactionRemoved', handler);
    }

    /**
     * Receives any Typing activities received
     * @remarks
     * Typing activities represent ongoing input from a user or a bot. This activity is often sent when keystrokes are being entered by a user.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onTyping(handler: BotHandler): this {
        return this.on('Typing', handler);
    }

    /**
     * Receives Handoff activities when a reaction is removed
     * @remarks
     * Handoff activities are used to request or signal a change in focus between elements inside a bot.
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onHandoff(handler: BotHandler): this {
        return this.on('Handoff', handler);
    }

    /**
     * Receives Event activities that indicate a new conversation has been created.
     * @remarks
     * activity.context.name will be 'createConversation'
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onCreateConversation(handler: BotHandler): this {
        return this.on('CreateConversation', handler);
    }

    /**
     * Receives Event activities that indicate an existing conversation is being continued.
     * @remarks
     * activity.context.name will be 'continueConversation'
     * @param handler BotHandler A handler function in the form async(context, next) => { ... }
     */
    public onContinueConversation(handler: BotHandler): this {
        return this.on('ContinueConversation', handler);
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
