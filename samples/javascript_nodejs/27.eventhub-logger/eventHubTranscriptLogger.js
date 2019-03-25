// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BatchingEventHubSender } = require('./batchingEventHubSender');
/**
 * EventHubTranscriptLogger, takes in an activity and then writes it to Event Hub.
 */
class EventHubTranscriptLogger {
    /**
     *
     * @param {string} connectionString 
     * @param {string} entityPath 
     */
    constructor(connectionString, entityPath) {
        this.batchSender = new BatchingEventHubSender(connectionString, entityPath);
    }

    /**
     * Log an activity to the log file.
     * @param activity Activity being logged.
     */
    async logActivity(activity) {
        if (!activity) {
            throw new Error('Activity is required.');
        }
        this.batchSender.send(activity);
    }
}
exports.EventHubTranscriptLogger = EventHubTranscriptLogger;
