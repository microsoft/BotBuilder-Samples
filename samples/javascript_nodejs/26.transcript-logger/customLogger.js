// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const util = require('util');
/**
 * CustomLogger, takes in an activity and saves it for the duration of the conversation, writing to an emulator compatible transcript file in the transcriptsPath folder.
 */
class CustomLogger {
    /**
     * Log an activity to the transcript file.
     * @param activity Activity being logged.
     */
    constructor() {
        this.conversations = {};
    }

    logActivity(activity) {
        if (!activity) {
            throw new Error('Activity is required.');
        }
        if (activity.conversation) {
            var id = activity.conversation.id;
            if (id.indexOf('|' !== -1)) {
                id = activity.conversation.id.replace(/\|.*/, '');
            }
        }

        if (activity.type === 'conversationUpdate' && !(id in this.conversations)) {
            console.log('new conversation');
            this.conversations[id] = [];
            this.conversations[id].push(activity);
        } else if (id in this.conversations) {
            this.conversations[id].push(activity);
        }

        if (activity.value === 'endOfInput') {
            console.log(this.conversations[id]);
            var transcriptfileName = util.format('%s/log_%s.transcript', process.env.transcriptsPath, id);
            fs.writeFile(transcriptfileName, JSON.stringify(this.conversations[id], null, 3), function(err) {
                if (err) throw err;
            });
            delete this.conversations[id];
        }
    }
}

exports.CustomLogger = CustomLogger;
