// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TelemetryClient } from 'applicationinsights';
import { EventTelemetry } from 'applicationinsights/out/Declarations/Contracts';
import { Activity, ConversationReference, Middleware, ResourceResponse, TurnContext, ActivityTypes } from 'botbuilder';

export interface MyAppInsightsMiddlewareSettings {
    /* The Application Insights instrumentation key.  See Application Insights for more information. */
    instrumentationKey: string;

    /* (Optional) Enable/Disable logging user name within Application Insights. */
    logUserName?: boolean;

    /* (Optional) Enable/Disable logging original message name within Application Insights.*/
    logOriginalMessage?: boolean;
}

/**
 * Middleware for logging incoming activitites into Application Insights.
 * In addition, registers a service so other components can log telemetry.
 * If this component is not registered, visibility within the Bot is not logged.
 */
export class MyAppInsightsMiddleware implements Middleware {

    // Indicates whether or not to log the user name into the BotMessageReceived event. Defaults to false.
    public logUserName = false;

    // Indicates whether or not to log the original message into the BotMessageReceived event. Defaults to false.
    public logOriginalMessage = false;

    public readonly appInsightsServiceKey = 'AppInsightsLoggerMiddleware.AppInsightsContext';

    // Application Insights Custom Event name, logged when new message is received from the user.
    public readonly botMsgReceivedEvent: 'BotMessageReceived';
    // Application Insights Custom Event name, logged when a message is sent out from the bot.
    public readonly botMsgSendEvent: 'BotMessageSend';
    // Application Insights Custom Event name, logged when a message is updated by the bot (rare case).
    public readonly botMsgUpdateEvent: 'BotMessageUpdate';
    // Application Insights Custom Event name, logged when a message is deleted by the bot (rare case).
    public readonly botMsgDeleteEvent: 'BotMessageDelete';

    // TelemetryClient used to send the Custom Application Insights Events.
    private _telemetryClient: TelemetryClient;
    
    constructor(settings: MyAppInsightsMiddlewareSettings) {
        if (!settings) {
            throw new Error('The settings parameter is required.');
        }

        if (!settings.instrumentationKey) {
            throw new Error('The settings.instrumentationKey parameter is required.');
        }

        if (settings.logUserName) {
            this.logUserName = settings.logUserName;
        }
        if (settings.logOriginalMessage) {
            this.logOriginalMessage = settings.logOriginalMessage;
        }

        this._telemetryClient = new TelemetryClient(settings.instrumentationKey);
    }

    /**
     * Records incoming and outgoing activities to the Application Insights store.
     * @param context Context for the current turn of conversation with the user.
     * @param next Function to invoke at the end of the middleware chain.
     */
    public async onTurn(turnContext: TurnContext, next: () => Promise<void>): Promise<void> {
        if (turnContext.activity) {
            const activity = turnContext.activity;

            // Set userId and sessionId tag values for the Application Insights Context object.
            if (activity.from && activity.from.id) {
                this._telemetryClient.context.keys.userId = activity.from.id;
            }
            
            if (activity.conversation && activity.conversation.id) {
                this._telemetryClient.context.keys.sessionId = activity.conversation.id;
            }

            // Construct the EventTelemetry object.
            const msgReceivedEvent: EventTelemetry = { name: this.botMsgReceivedEvent };

            // Add activity specific information, e.g. user ID, conversation ID, to the Event's properties.
            msgReceivedEvent.properties = this.fillReceiveEventProperties(turnContext.activity);

            // Add handlers onto the context sendActivities, updateActivities, and deleteActivities methods.
            // When calling any of these methods on the current context, a custom event will be sent 
            // to Application Insights.
            turnContext.onSendActivities(async (turnContext: TurnContext,
                                                activities: Partial<Activity>[],
                                                nextSend: () => Promise<ResourceResponse[]>): Promise<ResourceResponse[]> => {
                const responses: ResourceResponse[] = await nextSend();
                // Create each activity's Event Telemetry and send it to Application Insights.
                activities.forEach((activity) => {
                    const msgSentEvent: EventTelemetry = { name: this.botMsgSendEvent, properties: this.fillSendEventProperties(activity) };
                    this._telemetryClient.trackEvent(msgSentEvent);
                });
                return responses;
            });

            // Register on this turn's TurnContext a handler to send Event Telemetry to Application Insights whenever an activity is deleted.
            // This is a relatively rare case.
            turnContext.onDeleteActivity(async (turnContext: TurnContext, reference: Partial<ConversationReference>, nextDelete: () => Promise<void>): Promise<void> => {

                await nextDelete();
                // Create the delete activity's Event Telemetry and send it to Application Insights.
                const deleteMsgEvent: EventTelemetry = { name: this.botMsgDeleteEvent, properties: this.createBasicProperties(turnContext.activity) };
                this._telemetryClient.trackEvent(deleteMsgEvent);
            });

            // Register on this turn's TurnContext a handler to send Event Telemetry to Application Insights whenever an activity is updated.
            turnContext.onUpdateActivity(async (turnContext: TurnContext, activity: Partial<Activity>, nextUpdate: () => Promise<void>): Promise<void> => {
                
                // Finish running the request to update the activity before sending the Event Telemetry to Application Insights.
                await nextUpdate();
                
                // Create the update activity's Event Telemetry and send it to Application Insights.
                const msgUpdateEvent: EventTelemetry = { name: this.botMsgUpdateEvent, properties: this.fillUpdateEventProperties(turnContext.activity) };
                this._telemetryClient.trackEvent(msgUpdateEvent);
            })

            // After registering the onSendActivities, onDeleteActivity and onUpdateActivity handlers, send the msgReceivedEvent to Application Insights. 
            return new Promise<void>((resolve, reject) => {
                this._telemetryClient.trackEvent(msgReceivedEvent);
                resolve();
            }).then(() => {
                return next();
            });
        }

        // All middleware must call next to continue the middleware pipeline and reach the bot's central logic.
        // Otherwise the bot will stop processing activities after reaching a middleware that does not call `next()`.
        await next();
    }

    /**
     * Fills the Application Insights Custom Event properties for BotMessageReceived.
     * These properties are logged in the custom event when a new message is received from the user.
     * @param activity The Receive activity whose properties are placed into the Application Insights custom event.
     * @returns An object that is sent as "Properties" to Application Insights via the trackEvent method for the BotMessageReceived Message.
     */
    private fillReceiveEventProperties(activity: Partial<Activity>): { [key: string]: string } {
        const properties = { ...this.createBasicProperties(activity), Locale: activity.locale } as any;

        // For some customers, logging user name within Application Insights might be an issue so we have provided a config setting to enable this feature
        if (this.logUserName && activity.from.name) {
            properties.FromName = activity.from.name;
        }

        // For some customers, logging the utterances within Application Insights might be an issue so we have provided a config setting to enable this feature
        if (this.logOriginalMessage && activity.text) {
            properties.TextProperty = activity.text;
        }

        return properties;
    }

    /**
     * Fills the Application Insights Custom Event properties for BotMessageSend. 
     * These properties are logged in the custom event when a response message is sent by the Bot to the user.
     * @param activity The Send activity whose properties are placed into the Application Insights custom event.
     * @returns An object that is sent as "Properties" to Applications Insights via the trackEvent method for the BotMessageSend Message.
     */
    private fillSendEventProperties(activity: Partial<Activity>): { [key: string]: string } {
        const properties = { ...this.createBasicProperties(activity), Locale: activity.locale } as any;

        // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to enable this feature.
        if (this.logUserName && !!activity.recipient.name) {
            properties.RecipientName = activity.recipient.name;
        }

        // For some customers, logging the utterances within Application Insights might be an issue so have provided a config setting to enable this feature.
        if (this.logOriginalMessage && !!activity.text) {
            properties.Text = activity.text;
        }
        
        return properties;
    }

    /**
     * Fills the Application Insights Custom Event properties for BotMessageUpdate.
     * These properties are logged in the custom event when an activity message is updated by the Bot.
     * For example, if a card is interacted with by the use, and the card needs to be updated to reflect
     * some interaction.
     * @param activity The Update activity whose properties are placed into the Application Insights custom event.
     * @returns An object that is sent as "Properties" to Application Insights via the trackEvent method for the BotMessageUpdate Message.
     */
    private fillUpdateEventProperties(activity: Partial<Activity>): { [key: string]: string } {
        const properties = { ...this.createBasicProperties(activity), Locale: activity.locale } as any;

        // For some customers, logging the utterances within Application Insights might be an issue so have provided a config setting to enable this feature.
        if (this.logOriginalMessage && !!activity.text) {
            properties.Text = activity.text;
        }

        return properties;
    }

    /**
     * Returns a basic property bag that contains the following data:
     * - ActivityId: The incoming activity's id.
     * - Channel: The id of the channel, e.g. 'directline', 'facebook', 'msteams'.
     * - ConversationId: The unique identifier for a conversation.
     * - ConversationName: The name of a conversation.
     * - RecipientId: The unique id of the recipient.
     * 
     * @param activity The activity whose properties are placed into the Application Insights custom event.
     */
    private createBasicProperties(activity: Partial<Activity>): { [key: string]: string } {
        const properties = {
            ActivityId: activity.id,
            Channel: activity.channelId,
            ConversationId: activity.conversation.id,
            ConversationName: activity.conversation.name,
            RecipientId: activity.recipient
        } as any;

        return properties;
    }
};
