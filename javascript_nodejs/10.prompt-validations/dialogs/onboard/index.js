const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
const moment = require('moment');

const NamePrompt = require('../../prompts/namePrompt');
const AgePrompt = require('../../prompts/agePrompt');
const DOBPrompt = require('../../prompts/dobPrompt');
const ColorPrompt = require('../../prompts/colorPrompt');

const START_DIALOG = 'start';
const HELLO_USER = 'welcome_back';


const NAME_PROMPT = 'namePrompt';
const AGE_PROMPT = 'agePrompt';
const DOB_PROMPT = 'dobPrompt';
const COLOR_PROMPT = 'colorPrompt';


const USER_NAME_PROP = 'user_name';
const AGE_PROP = 'user_age';
const DOB_PROP = 'user_dob';
const COLOR_PROP = 'user_color';

class OnboardingDialog extends ComponentDialog {

    constructor (dialogId, userName, userAge, userDob, userColor) {
        super(dialogId);

        // Create a dialog flow that captures a series of values from a user
        this.addDialog(new WaterfallDialog(START_DIALOG,[
            async (dc, step) => {
                const user_name = await userName.get(dc.context, '');
                if (user_name) {
                    return dc.replace(HELLO_USER);
                } else {
                    return await dc.prompt(NAME_PROMPT, `What is your name, human?`);
                }
            },
            async (dc, step) => {
                step.values[USER_NAME_PROP] = step.result;
                return await dc.prompt(AGE_PROMPT, `What is your age?`);
            },
            async (dc, step) => {
                step.values[AGE_PROP] = step.result;
                return await dc.prompt(DOB_PROMPT, `What is your date of birth?`);
            },
            async (dc, step) => {
                step.values[DOB_PROP] = step.result;
                const choices = ['red','blue','green'];
                return await dc.prompt(COLOR_PROMPT, `Finally, what is your favorite color?`, choices);
            },
            async (dc, step) => {
                step.values[COLOR_PROP] = step.result;
                return await step.next();
            },
            async (dc, step) => {
                await userName.set(dc.context, step.values[USER_NAME_PROP]);
                await userAge.set(dc.context, step.values[AGE_PROP]);
                await userDob.set(dc.context, step.values[DOB_PROP]);
                await userColor.set(dc.context, step.values[COLOR_PROP]);
        
                await dc.context.sendActivity(`Your profile is complete! Thank you.`);

                // Transition to the display of the profile data.
                return await dc.begin(HELLO_USER);
            }
        ]));

        this.addDialog(new WaterfallDialog(HELLO_USER, [
            async (dc, step) => {
                const user_name = await userName.get(dc.context, null);
                const user_dob = await userDob.get(dc.context, null);
                const user_age = await userAge.get(dc.context, null);
                const user_color = await userColor.get(dc.context, null);

                await dc.context.sendActivity(`You asked me to call you ${user_name}. You were born on ${ moment( user_dob ).format("MMM Do, YYYY") } and claim to be ${ user_age }. Your favorite color is ${ user_color }.`);
                return await step.next();
            }
        ]));

        // Add prompts
        // namePrompt will validate that the user's response is between 1 and 50 chars in length
        this.addDialog(new NamePrompt(NAME_PROMPT));

        // agePrompt will validate an age between 1 and 99
        this.addDialog(new AgePrompt(AGE_PROMPT));
        
        // dobPrompt will validate a date between 8/24/1918 and 8/24/2018
        this.addDialog(new DOBPrompt(DOB_PROMPT));
                
        // colorPrompt provides a validation error when a valid choice is not made
        this.addDialog(new ColorPrompt(COLOR_PROMPT));
    }
}

module.exports = OnboardingDialog;