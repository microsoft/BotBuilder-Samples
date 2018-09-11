// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TextPrompt } = require('botbuilder-dialogs');

// Minimum lengh requirements for city and name
const CITY_LENGTH_MIN = 5;
const NAME_LENGTH_MIN = 3;

/**
 * CityPrompt show how to extend the TextPrompt class
 * to perform input validation
 */
class CityPrompt extends TextPrompt {
  constructor(dialogId) {
    super(dialogId, async (context, prompt) => {
      // Validate that the user entered a minimum lenght for their name
      const value = (prompt.recognized.value || '').trim();
      if (value.length > CITY_LENGTH_MIN) {
        prompt.end(value);
      } else {
        await context.sendActivity(`City names needs to be at least ${CITY_LENGTH_MIN} characters long.`);
      }
    });
  }
}

/**
 * NamePrompt show how to extend the TextPrompt class
 * to perform input validation
 */
class NamePrompt extends TextPrompt {
  constructor(dialogId) {
    super(dialogId, async (context, prompt) => {
      // Validate that the user entered a minimum lenght for their name
      const value = (prompt.recognized.value || '').trim();
      if (value.length > NAME_LENGTH_MIN) {
        prompt.end(value);
      } else {
        await context.sendActivity(`Names need to be at least ${NAME_LENGTH_MIN} characters long.`);
      }
    });
  }
}

module.exports.CityPrompt = CityPrompt;
module.exports.NamePrompt = NamePrompt;
