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
 * @param {object} args (Optional) map of named arguments to replace within the resource string.
 */
function formatMessage(locale, resourceId, args) {
    const text = getText(locale, resourceId, args);
    const speak = getText(locale, resourceId + '_ssml');
    return MessageFactory.text(text, speak ? ssml.speak(speak) : undefined);
}
module.exports.formatMessage = formatMessage;

/**
 * Retrieves a text string from a resource file and optionally substitute ${arg_names} within the
 * string.
 * @param {string} locale Locale of the resource file to use.
 * @param {string} resourceId ID of the resource string to lookup.
 * @param {object} args (Optional) map of named arguments to replace within the resource string.
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
                const match = new RegExp(`\\\${${ key }}`, 'g');
                output = output.replace(match, args[key]);
            }
        }
    }
    return output;
}
module.exports.getText = getText;
