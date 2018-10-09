// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class SlotDetails {
    /**
     * SlotDetails is a small class that defines a "slot" to be filled in a SlotFillingDialog.
     * @param {string} name The field name used to store user's response.
     * @param {string} promptId A unique identifier of a Dialog or Prompt registered on the DialogSet.
     * @param {string} prompt The text of the prompt presented to the user.
     * @param {string} reprompt (optional) The text to present if the user responds with an invalid value.
     */
    constructor(name, promptId, prompt, reprompt) {
        this.name = name;
        this.promptId = promptId;
        if (prompt && reprompt) {
            this.options = {
                prompt: prompt,
                retryPrompt: reprompt
            };
        } else {
            this.options = prompt;
        }
    }
}

module.exports.SlotDetails = SlotDetails;
