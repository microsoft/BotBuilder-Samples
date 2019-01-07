// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory } = require('botbuilder');
const ssml = require('../ssml');

const resources = {
    'en': require('./en.json')
};

/**
 * Format a `message` activity using resource strings in a locale file.
 * @param {string} locale Locale of the resource file to use.
 * @param {string} resourceId ID of the resource string to lookup.
<<<<<<< HEAD
 * @param {object} args (Optional) map of named arguments to replace within the resource string. 
=======
 * @param {object} args (Optional) map of named arguments to replace within the resource string.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 */
function formatMessage(locale, resourceId, args) {
    const text = getText(locale, resourceId, args);
    const speak = getText(locale, resourceId + '_ssml');
    return MessageFactory.text(text, speak ? ssml.speak(speak) : undefined);
}
module.exports.formatMessage = formatMessage;

/**
<<<<<<< HEAD
 * Retrieves a text string from a resource file and optionally substitute ${arg_names} within the 
 * string.  
 * @param {string} locale Locale of the resource file to use.
 * @param {string} resourceId ID of the resource string to lookup.
 * @param {object} args (Optional) map of named arguments to replace within the resource string. 
=======
 * Retrieves a text string from a resource file and optionally substitute ${arg_names} within the
 * string.
 * @param {string} locale Locale of the resource file to use.
 * @param {string} resourceId ID of the resource string to lookup.
 * @param {object} args (Optional) map of named arguments to replace within the resource string.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 */
function getText(locale, resourceId, args) {
    // Lookup resource
    let output = resources[locale][resourceId];
    if (output) {
        // Choose a text string at random
        if (Array.isArray(output)) {
            const i = Math.floor(Math.random() * output.length);
            output = output[i];
        }

        // Substitute args within the text string
        if (args) {
            for (const key in args) {
<<<<<<< HEAD
                const match = new RegExp(`\\\${${key}}`, 'g');
=======
                const match = new RegExp(`\\\${${ key }}`, 'g');
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
                output = output.replace(match, args[key]);
            }
        }
    }
    return output;
}
module.exports.getText = getText;
