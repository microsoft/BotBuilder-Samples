// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const appInsights = require("applicationinsights");
const { EventHubClient } = require('@azure/event-hubs');


/**
 * EventHubsBatchingSender, takes in an activity and then writes it to the Azure EventHub in batches.
  */
class EventHubsBatchingSender {
    /**
     *
     * @param {string} connectionString 
     * @param {string} entityPath 
     */
    constructor(connectionString, entityPath, batchSize = null, batchIntervalMs=null) {
        this._client = EventHubClient.createFromConnectionString(connectionString, entityPath);
        this._cache = [];
        this._batchSize = batchSize || process.env.EventHubsBatchSize || 5;
        this._partition_index = 0;
        this._partitionIds = null;
        this._batchIntervalMs = batchIntervalMs || process.env.EventHubsBatchIntervalMs || 60*1000;
    }


    async send(activity) {
        if (this._partitionIds === null) {
            this._partitionIds = await this._client.getPartitionIds();
        }

        activity = JSON.parse(activity);
        
        console.warn("Pushing Event");
        if (!('channelId' in activity && 'type' in activity)) {
            console.warn('Activity missing channelId and/or type properties');
            console.warn(activity);
        }
        
        var newEvent = { body: activity };
        this._cache.push(newEvent);
    
        if (this._cache.length >= this._batchSize)
        {
            await this.triggerSend();
            return;
        }

        // ensure an invocation timeout is set if anything is in the buffer
        if (!this._timeoutHandle && this._cache.length > 0) {
            this._timeoutHandle = setTimeout(async () => {
                this._timeoutHandle = null;
                await this.triggerSend();
            }, this._batchIntervalMs);
        }            
    }

    /**
     * Immediately send buffered data
     */
    async triggerSend(callback) {
        let bufferIsEmpty = this._cache.length < 1;
        if (!bufferIsEmpty) {
            // invoke send
            const partition = this._partition_index;
            const batch = Array.from(this._cache);
            this._partition_index = (this._partition_index + 1) % this._partitionIds.length;
            await this._client.sendBatch(batch, this._partitionIds[partition]).then(() => {
                console.warn('Batch sent');
            })
        }

        // update lastSend time to enable throttling
        this._lastSend = +new Date;

        // clear buffer
        this._cache.length = 0
        clearTimeout(this._timeoutHandle);
        this._timeoutHandle = null;
        if (bufferIsEmpty && typeof callback === "function") {
            callback("no data to send");
        }
    }    

}

let batchSender = new EventHubsBatchingSender(process.env.EventHubsConnectionString, process.env.EventHubsName);
module.exports = {
    getBatchSender: () => batchSender,
}

exports.EventHubsBatchingSender = EventHubsBatchingSender;
