// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class TelemetryLoggerMiddleware {
    constructor(telemetryClient, settings) {
        // Indicates whether or not to log the user name into the BotMessageReceived event. Defaults to false.
        this.logUserName = false;

        // Indicates whether or not to log the original message into the BotMessageReceived event. Defaults to false.
        this.logOriginalMessage = false;
        this.appInsightsServiceKey = 'TelemetryLoggerMiddleware.AppInsightsContext';

        if (!settings) {
            throw new Error('The settings parameter is required.');
        }
        if (settings.logUserName) {
            this.logUserName = settings.logUserName;
        }
        if (settings.logOriginalMessage) {
            this.logOriginalMessage = settings.logOriginalMessage;
        }
        this._telemetryClient = telemetryClient;

        // Application Insights Custom Event name, logged when new message is received from the user.
        this.botMsgReceivedEvent = 'BotMessageReceived';

        // Application Insights Custom Event name, logged when a message is sent out from the bot.
        this.botMsgSendEvent = 'BotMessageSend';

        // Application Insights Custom Event name, logged when a message is updated by the bot (rare case).
        this.botMsgUpdateEvent = 'BotMessageUpdate';

        // Application Insights Custom Event name, logged when a message is deleted by the bot (rare case).
        this.botMsgDeleteEvent = 'BotMessageDelete';
    }

    async onTurn(turnContext, next) {
        if (turnContext.activity) {
            // Store the TelemetryClient on the TurnContext's turnState so MyAppInsightsQnAMaker can use it.
            turnContext.turnState.set(this.appInsightsServiceKey, this._telemetryClient);

            // Construct the EventTelemetry object.
            const msgReceivedEvent = { name: this.botMsgReceivedEvent };
            // Add activity specific information, e.g. user ID, conversation ID, to the Event's properties.
            msgReceivedEvent.properties = this.fillReceiveEventProperties(turnContext.activity);
            // Add handlers onto the context sendActivities, updateActivities, and deleteActivities methods.
            // When calling any of these methods on the current context, a custom event will be sent
            // to Application Insights.
            turnContext.onSendActivities(async (turnContext, activities, nextSend) => {
                const responses = await nextSend();
                // Create each activity's Event Telemetry and send it to Application Insights.
                activities.forEach((activity) => {
                    const msgSentEvent = { name: this.botMsgSendEvent, properties: this.fillSendEventProperties(activity) };
                    this._telemetryClient.trackEvent(msgSentEvent);
                });
                return responses;
            });

            // Register on this turn's TurnContext a handler to send Event Telemetry to Application Insights whenever an activity is deleted.
            // This is a relatively rare case.
            turnContext.onDeleteActivity(async (turnContext, reference, nextDelete) => {
                await nextDelete();
                // Create the delete activity's Event Telemetry and send it to Application Insights.
                const deleteMsgEvent = { name: this.botMsgDeleteEvent, properties: this.createBasicProperties(turnContext.activity) };
                this._telemetryClient.trackEvent(deleteMsgEvent);
            });

            // Register on this turn's TurnContext a handler to send Event Telemetry to Application Insights whenever an activity is updated.
            turnContext.onUpdateActivity(async (turnContext, activity, nextUpdate) => {
                // Finish running the request to update the activity before sending the Event Telemetry to Application Insights.
                await nextUpdate();
                // Create the update activity's Event Telemetry and send it to Application Insights.
                const msgUpdateEvent = { name: this.botMsgUpdateEvent, properties: this.fillUpdateEventProperties(turnContext.activity) };
                this._telemetryClient.trackEvent(msgUpdateEvent);
            });
            // After registering the onSendActivities, onDeleteActivity and onUpdateActivity handlers, send the msgReceivedEvent to Application Insights.
            return new Promise((resolve, reject) => {
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

    fillReceiveEventProperties(activity) {
        const properties = Object.assign({}, this.createBasicProperties(activity), { Locale: activity.locale });
        // For some customers, logging user name within Application Insights might be an issue so we have provided a config setting to enable this feature
        if (this.logUserName && activity.from.name) {
            properties.fromId = activity.from.id;
            properties.fromName = activity.from.name;
        }
        // For some customers, logging the utterances within Application Insights might be an issue so we have provided a config setting to enable this feature
        if (this.logOriginalMessage && activity.text) {
            properties.text = activity.text;
        }
        return properties;
    }

    fillSendEventProperties(activity) {
        const properties = Object.assign({}, this.createBasicProperties(activity), { Locale: activity.locale });
        // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to enable this feature.
        if (this.logUserName && !!activity.recipient.name) {
            properties.recipientName = activity.recipient.name;
        }
        // For some customers, logging the utterances within Application Insights might be an issue so have provided a config setting to enable this feature.
        if (this.logOriginalMessage && !!activity.text) {
            properties.text = activity.text;
        }
        return properties;
    }

    fillUpdateEventProperties(activity) {
        const properties = Object.assign({}, this.createBasicProperties(activity), { Locale: activity.locale });
        // For some customers, logging the utterances within Application Insights might be an issue so have provided a config setting to enable this feature.
        if (this.logOriginalMessage && !!activity.text) {
            properties.text = activity.text;
        }
        return properties;
    }

    createBasicProperties(activity) {
        const properties = {
            activityId: activity.id,
            channel: activity.channelId,
            conversationId: activity.conversation.id,
            conversationName: activity.conversation.name,
            recipientId: activity.recipient
        };
        return properties;
    }
}

module.exports.TelemetryLoggerMiddleware = TelemetryLoggerMiddleware;
