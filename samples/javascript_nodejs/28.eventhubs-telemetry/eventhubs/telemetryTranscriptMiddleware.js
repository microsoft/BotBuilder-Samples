// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

const { NullTelemetryClient } = require("botbuilder-core");
const botframework_schema = require("botframework-schema");
const { TelemetryConstants } = require("./telemetryConstants");

/**
 * Middleware for logging incoming, outgoing, updated or deleted Activity messages.
 * 
 * The primary difference between the TelemetryLoggerMiddleware and TelemetryTranscriptLoggerMiddleware
 * is Transcript adds the activity.
 * 
 */
class TelemetryTranscriptLoggerMiddleware {
    // tslint:enable:variable-name
    /**
     * Initializes a new instance of the TelemetryLoggerMiddleware class.
     * @param telemetryClient The BotTelemetryClient used for logging.
     * @param logPersonalInformation (Optional) Enable/Disable logging original message name within Application Insights.
     */
    constructor(telemetryClient, logPersonalInformation = false) {
        this.telemetryConstants = new TelemetryConstants();
        this._telemetryClient = telemetryClient || new NullTelemetryClient();
        this._logPersonalInformation = logPersonalInformation;
    }
    /**
     * Gets a value indicating whether determines whether to log personal information that came from the user.
     */
    get logPersonalInformation() { return this._logPersonalInformation; }
    /**
     * Gets the currently configured botTelemetryClient that logs the events.
     */
    get telemetryClient() { return this._telemetryClient; }
    /**
     * Logs events based on incoming and outgoing activities using the botTelemetryClient class.
     * @param context The context object for this turn.
     * @param next The delegate to call to continue the bot middleware pipeline
     */
    async onTurn(context, next) {
        if (context === null) {
            throw new Error('context is null');
        }
        // log incoming activity at beginning of turn
        if (context.activity !== null) {
            const activity = context.activity;
            // Log Bot Message Received
            await this.onReceiveActivity(activity);
        }
        // hook up onSend pipeline
        context.onSendActivities(async (ctx, activities, nextSend) => {
            // run full pipeline
            const responses = await nextSend();
            activities.forEach(async (act) => {
                await this.onSendActivity(act);
            });
            return responses;
        });
        // hook up update activity pipeline
        context.onUpdateActivity(async (ctx, activity, nextUpdate) => {
            // run full pipeline
            const response = await nextUpdate();
            await this.onUpdateActivity(activity);
            return response;
        });
        // hook up delete activity pipeline
        context.onDeleteActivity(async (ctx, reference, nextDelete) => {
            // run full pipeline
            await nextDelete();
            const deletedActivity = turnContext_1.TurnContext.applyConversationReference({
                type: botframework_schema_1.ActivityTypes.MessageDelete,
                id: reference.activityId
            }, reference, false);
            await this.onDeleteActivity(deletedActivity);
        });
        if (next !== null) {
            await next();
        }
    }
    /**
     * Invoked when a message is received from the user.
     * Performs logging of telemetry data using the IBotTelemetryClient.TrackEvent() method.
     * The event name logged is "BotMessageReceived".
     * @param activity Current activity sent from user.
     */
    async onReceiveActivity(activity) {
        var additionalProperties = {};
        additionalProperties[this.telemetryConstants.transcript] = Object.assign({}, activity);;
        this.telemetryClient.trackEvent({
            name: TelemetryTranscriptLoggerMiddleware.botMsgReceiveEvent,
            properties: await this.fillReceiveEventProperties(activity, additionalProperties)
        });
    }
    /**
     * Invoked when the bot sends a message to the user.
     * Performs logging of telemetry data using the botTelemetryClient.trackEvent() method.
     * The event name logged is "BotMessageSend".
     * @param activity Last activity sent from user.
     */
    async onSendActivity(activity) {
        var additionalProperties = {};
        additionalProperties[this.telemetryConstants.transcript] = Object.assign({}, activity);
        this.telemetryClient.trackEvent({
            name: TelemetryTranscriptLoggerMiddleware.botMsgSendEvent,
            properties: await this.fillSendEventProperties(activity, additionalProperties)
        });
    }
    /**
     * Invoked when the bot updates a message.
     * Performs logging of telemetry data using the botTelemetryClient.trackEvent() method.
     * The event name used is "BotMessageUpdate".
     * @param activity
     */
    async onUpdateActivity(activity) {
        var additionalProperties = {};
        additionalProperties[this.telemetryConstants.transcript] = Object.assign({}, activity);
        this.telemetryClient.trackEvent({
            name: TelemetryTranscriptLoggerMiddleware.botMsgUpdateEvent,
            properties: await this.fillUpdateEventProperties(activity, additionalProperties)
        });
    }
    /**
     * Invoked when the bot deletes a message.
     * Performs logging of telemetry data using the botTelemetryClient.trackEvent() method.
     * The event name used is "BotMessageDelete".
     * @param activity
     */
    async onDeleteActivity(activity) {
        var additionalProperties = {};
        additionalProperties[this.telemetryConstants.transcript] = Object.assign({}, activity);
        this.telemetryClient.trackEvent({
            name: TelemetryTranscriptLoggerMiddleware.botMsgDeleteEvent,
            properties: await this.fillDeleteEventProperties(activity, additionalProperties)
        });
    }
    /**
     * Fills the Application Insights Custom Event properties for BotMessageReceived.
     * These properties are logged in the custom event when a new message is received from the user.
     * @param activity Last activity sent from user.
     * @param telemetryProperties Additional properties to add to the event.
     * @returns A dictionary that is sent as "Properties" to botTelemetryClient.trackEvent method.
     */
    async fillReceiveEventProperties(activity, telemetryProperties) {
        const properties = {};
        properties[this.telemetryConstants.fromIdProperty] = activity.from.id || '';
        properties[this.telemetryConstants.conversationNameProperty] = activity.conversation.name || '';
        properties[this.telemetryConstants.localeProperty] = activity.locale || '';
        properties[this.telemetryConstants.recipientIdProperty] = activity.recipient.id;
        properties[this.telemetryConstants.recipientNameProperty] = activity.recipient.name;
        // Use the LogPersonalInformation flag to toggle logging PII data, text and user name are common examples
        if (this.logPersonalInformation) {
            if (activity.from.name && activity.from.name.trim()) {
                properties[this.telemetryConstants.fromNameProperty] = activity.from.name;
            }
            if (activity.text && activity.text.trim()) {
                properties[this.telemetryConstants.textProperty] = activity.text;
            }
            if (activity.speak && activity.speak.trim()) {
                properties[this.telemetryConstants.speakProperty] = activity.speak;
            }
        }
        // Additional Properties can override "stock" properties.
        if (telemetryProperties) {
            return Object.assign({}, properties, telemetryProperties);
        }
        return properties;
    }
    /**
     * Fills the Application Insights Custom Event properties for BotMessageSend.
     * These properties are logged in the custom event when a response message is sent by the Bot to the user.
     * @param activity - Last activity sent from user.
     * @param telemetryProperties Additional properties to add to the event.
     * @returns A dictionary that is sent as "Properties" to botTelemetryClient.trackEvent method.
     */
    async fillSendEventProperties(activity, telemetryProperties) {
        const properties = {};
        properties[this.telemetryConstants.replyActivityIdProperty] = activity.replyToId || '';
        properties[this.telemetryConstants.recipientIdProperty] = activity.recipient.id;
        properties[this.telemetryConstants.conversationNameProperty] = activity.conversation.name;
        properties[this.telemetryConstants.localeProperty] = activity.locale || '';
        // Use the LogPersonalInformation flag to toggle logging PII data, text and user name are common examples
        if (this.logPersonalInformation) {
            if (activity.recipient.name && activity.recipient.name.trim()) {
                properties[this.telemetryConstants.recipientNameProperty] = activity.recipient.name;
            }
            if (activity.text && activity.text.trim()) {
                properties[this.telemetryConstants.textProperty] = activity.text;
            }
            if (activity.speak && activity.speak.trim()) {
                properties[this.telemetryConstants.speakProperty] = activity.speak;
            }
        }
        // Additional Properties can override "stock" properties.
        if (telemetryProperties) {
            return Object.assign({}, properties, telemetryProperties);
        }
        return properties;
    }
    /**
     * Fills the event properties for BotMessageUpdate.
     * These properties are logged when an activity message is updated by the Bot.
     * For example, if a card is interacted with by the use, and the card needs to be updated to reflect
     * some interaction.
     * @param activity - Last activity sent from user.
     * @param telemetryProperties Additional properties to add to the event.
     * @returns A dictionary that is sent as "Properties" to botTelemetryClient.trackEvent method.
     */
    async fillUpdateEventProperties(activity, telemetryProperties) {
        const properties = {};
        properties[this.telemetryConstants.recipientIdProperty] = activity.recipient.id;
        properties[this.telemetryConstants.conversationIdProperty] = activity.conversation.id;
        properties[this.telemetryConstants.conversationNameProperty] = activity.conversation.name;
        properties[this.telemetryConstants.localeProperty] = activity.locale || '';
        // Use the LogPersonalInformation flag to toggle logging PII data, text is a common example
        if (this.logPersonalInformation && activity.text && activity.text.trim()) {
            properties[this.telemetryConstants.textProperty] = activity.text;
        }
        // Additional Properties can override "stock" properties.
        if (telemetryProperties) {
            return Object.assign({}, properties, telemetryProperties);
        }
        return properties;
    }
    /**
     * Fills the Application Insights Custom Event properties for BotMessageDelete.
     * These properties are logged in the custom event when an activity message is deleted by the Bot.  This is a relatively rare case.
     * @param activity - Last activity sent from user.
     * @param telemetryProperties Additional properties to add to the event.
     * @returns A dictionary that is sent as "Properties" to botTelemetryClient.trackEvent method.
     */
    async fillDeleteEventProperties(activity, telemetryProperties) {
        const properties = {};
        properties[this.telemetryConstants.channelIdProperty] = activity.channelId;
        properties[this.telemetryConstants.recipientIdProperty] = activity.recipient.id;
        properties[this.telemetryConstants.conversationIdProperty] = activity.conversation.id;
        properties[this.telemetryConstants.conversationNameProperty] = activity.conversation.name;
        // Additional Properties can override "stock" properties.
        if (telemetryProperties) {
            return Object.assign({}, properties, telemetryProperties);
        }
        return properties;
    }
}
/**
 * The name of the event when when new message is received from the user.
 */
TelemetryTranscriptLoggerMiddleware.botMsgReceiveEvent = 'BotMessageReceived';
/**
 * The name of the event when a message is updated by the bot.
 */
TelemetryTranscriptLoggerMiddleware.botMsgSendEvent = 'BotMessageSend';
/**
 * The name of the event when a message is updated by the bot.
 */
TelemetryTranscriptLoggerMiddleware.botMsgUpdateEvent = 'BotMessageUpdate';
/**
 * The name of the event when a message is deleted by the bot.
 */
TelemetryTranscriptLoggerMiddleware.botMsgDeleteEvent = 'BotMessageDelete';

exports.TelemetryTranscriptLoggerMiddleware = TelemetryTranscriptLoggerMiddleware;
