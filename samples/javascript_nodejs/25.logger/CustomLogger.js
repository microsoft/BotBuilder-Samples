// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const util = require('util');
/**
 * CustomLogger, takes in an activity and then writes it to a log file in the transcriptsPath folder.
 */
class CustomLogger {
    /**
     * Log an activity to the log file.
     * @param activity Activity being logged.
     */
    logActivity(activity) {
        if (!activity) {
            throw new Error('Activity is required.');
        }
        var logText = util.format('\n Activity Received: %s \n', util.inspect(activity));
        console.log(logText);

        if (activity.conversation) {
            var id = activity.conversation.id;
            if (id.indexOf('|') !== -1) {
                id = activity.conversation.id.replace(/\|.*/, '');
            }
            var transcriptfileName = util.format('%s/log_%s.log', process.env.transcriptsPath, id);
            fs.appendFile(transcriptfileName, logText, function(err) {
                if (err) throw err;
            });
        }
    }
}
exports.CustomLogger = CustomLogger;
