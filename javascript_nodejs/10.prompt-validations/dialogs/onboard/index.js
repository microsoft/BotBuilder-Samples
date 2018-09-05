const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
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

class OnboardingDialog extends ComponentDialog {

    /**
     * 
     * @param {string} dialogId A unique identifier for this dialog.
     * @param {BotStatePropertyAccessor} userName A property used to store the user's name.
     * @param {BotStatePropertyAccessor} userAge A property used to store the user's age.
     * @param {BotStatePropertyAccessor} userDob A property used to store the user's date of birth.
     * @param {BotStatePropertyAccessor} userColor A property used to store the user's favorite color.
     */
    constructor (dialogId, userName, userAge, userDob, userColor) {
        super(dialogId);

        // Create a dialog flow that captures a series of values from a user
        this.addDialog(new WaterfallDialog(START_DIALOG,[
            // If a user name is already set, switch to the HELLO_USER dialog.
            // Otherwise, continue with collecting values from the user.
            async (dc, step) => {
                const user_name = await userName.get(dc.context, '');
                if (user_name) {
                    return dc.replace(HELLO_USER);
                } else {
                    return await dc.prompt(GET_NAME_PROMPT, `What is your name, human?`);
                }
            },
            // Collect user name, then prompt for age. 
            async (dc, step) => {
                step.values[USER_NAME_PROPERTY] = step.result;
                return await dc.prompt(GET_AGE_PROMPT, `What is your age?`);
            },
            // Collect age, then prompt for date of birth.
            async (dc, step) => {
                step.values[AGE_PROPERTY] = step.result;
                return await dc.prompt(GET_DOB_PROMPT, `What is your date of birth?`);
            },
            // Collect date of birth, then prompt for favorite color.
            async (dc, step) => {
                step.values[DOB_PROPERTY] = step.result;
                const choices = ['red','blue','green'];
                return await dc.prompt(GET_COLOR_PROMPT, `Finally, what is your favorite color?`, choices);
            },
            // Collect favorite color and continue.
            async (dc, step) => {
                step.values[COLOR_PROPERTY] = step.result;
                return await step.next();
            },
            // With all values in hand, we can now store them in our model and complete.
            async (dc, step) => {
                await userName.set(dc.context, step.values[USER_NAME_PROPERTY]);
                await userAge.set(dc.context, step.values[AGE_PROPERTY]);
                await userDob.set(dc.context, step.values[DOB_PROPERTY]);
                await userColor.set(dc.context, step.values[COLOR_PROPERTY]);
        
                await dc.context.sendActivity(`Your profile is complete! Thank you.`);

                // Transition to the display of the profile data.
                return await dc.begin(HELLO_USER);
            }
        ]));

        // This dialog loads and displays the information previously provided by the user.
        this.addDialog(new WaterfallDialog(HELLO_USER, [
            async (dc, step) => {
                const user_name = await userName.get(dc.context, null);
                const user_dob = await userDob.get(dc.context, null);
                const user_age = await userAge.get(dc.context, null);
                const user_color = await userColor.get(dc.context, null);

                const text = [
                    `You asked me to call you ${user_name}.`,
                    `You were born on ${ moment( user_dob ).format("MMM Do, YYYY") } and claim to be ${ user_age }.`,
                    `Your favorite color is ${ user_color }.`
                ];

                await dc.context.sendActivity(text.join(' '));
                return await dc.end();
            }
        ]));

        // Add prompts:
        // NAME_PROMPT will validate that the user's response is between 1 and 50 chars in length.
        this.addDialog(new NamePrompt(NAME_PROMPT));

        // AGE_PROMPT will validate an age between 1 and 99.
        this.addDialog(new AgePrompt(AGE_PROMPT));
        
        // DOB_PROMPT will validate a date between 8/24/1918 and 8/24/2018.
        this.addDialog(new DOBPrompt(DOB_PROMPT));
                
        // COLOR_PROMPT provides a validation error when a valid choice is not made.
        this.addDialog(new ColorPrompt(COLOR_PROMPT));
    }
}

module.exports = OnboardingDialog;