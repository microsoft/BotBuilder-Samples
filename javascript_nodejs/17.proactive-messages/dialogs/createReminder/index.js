const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
const { TurnContext } = require('botbuilder-core');
const moment = require('moment');

// Import a few specialized prompt classes.
const NamePrompt = require('../../prompts/namePrompt');
const AgePrompt = require('../../prompts/agePrompt');
const DOBPrompt = require('../../prompts/dobPrompt');
const ColorPrompt = require('../../prompts/colorPrompt');

const START_DIALOG = 'start';
const HELLO_USER = 'welcome_back';

const GET_NAME_PROMPT = 'namePrompt';
const GET_AGE_PROMPT = 'agePrompt';
const GET_DOB_PROMPT = 'dobPrompt';
const GET_COLOR_PROMPT = 'colorPrompt';

const USER_NAME_PROPERTY = 'user_name';
const AGE_PROPERTY = 'user_age';
const DOB_PROPERTY = 'user_dob';
const COLOR_PROPERTY = 'user_color';

class CreateReminderDialog extends ComponentDialog {

    /**
     * 
     * @param {string} dialogId A unique identifier for this dialog.
     * @param {BotStatePropertyAccessor} userName A property used to store the user's name.
     * @param {BotStatePropertyAccessor} userAge A property used to store the user's age.
     * @param {BotStatePropertyAccessor} userDob A property used to store the user's date of birth.
     * @param {BotStatePropertyAccessor} userColor A property used to store the user's favorite color.
     */
    constructor (dialogId) {
        super(dialogId);

        this.addDialog(new WaterfallDialog(START_DIALOG, [
            await (dc, step) => {
                return await dc.context.sendActivity(`Let's set up a simple reminder!`)
            },
            await (dc, step) => {
                const reference = TurnContext.getConversationReference(dc.context.activity);
                return await dc.context.sendActivity(`The conversation reference we will use is ${ reference }`);
            },
        ]));

    }
}

module.exports = CreateReminderDialog;