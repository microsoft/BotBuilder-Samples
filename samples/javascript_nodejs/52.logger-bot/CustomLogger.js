/**
 * Licensed under the MIT License.
 */
const fs = require('fs');
const util = require('util');
/**
 * CustomLogger, takes in an activity and then writes it to a transcript file in the logs folder. 
 */
class CustomLogger {
    /**
     * Log an activity to the transcript file.
     * @param activity Activity being logged.
     */
    logActivity(activity) {
        if (!activity) {
            throw new Error('Activity is required.');
        }
        if (activity.conversation) {
            var logText = util.format('\n Activity Recieved: %s \n', util.inspect(activity));
            var logfileName = util.format('%s/log_%s.txt', process.env.logFilePath, activity.conversation.id);
            // tslint:disable-next-line:no-console
            console.log(logText);
            fs.appendFile(logfileName, logText, function(err) {
                if (err) throw err;
            });
        }
    }
}
exports.CustomLogger = CustomLogger;
