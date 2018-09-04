const { ComponentDialog, TextPrompt, NumberPrompt, DateTimePrompt, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');

const START_DIALOG = 'start';
const HELLO_USER = 'welcome_back';

const USER_NAME_PROP = 'user_name';
const AGE_PROP = 'user_age';
const DOB_PROP = 'user_dob';
const COLOR_PROP = 'user_color';


const NAME_PROMPT = 'namePrompt';
const AGE_PROMPT = 'agePrompt';
const DOB_PROMPT = 'dobPrompt';
const COLOR_PROMPT = 'colorPrompt';

class OnboardingDialog extends ComponentDialog {

    constructor (dialogId, userState) {
        super(dialogId);

        this.userState = userState;

        this.userName = this.userState.createProperty(USER_NAME_PROP)
        this.userAge = this.userState.createProperty(AGE_PROP);
        this.userDob = this.userState.createProperty(DOB_PROP);
        this.userColor = this.userState.createProperty(COLOR_PROP);


        // Create a dialog flow that captures a series of values from a user
        this.addDialog(new WaterfallDialog(START_DIALOG,[
            this.collectName,
            this.collectAge,
            this.collectDob,
            this.collectColor,
            this.lastStep,
            this.confirmStep
        ]));

        this.addDialog(new WaterfallDialog(HELLO_USER, [
            async function(dc, step) {
                const user_name = await this.userName.get(dc.context, null);
                await dc.context.sendActivity(`You asked me to call you ${user_name}.`);
                return await step.next();
            }
        ]));


        // Add prompts
        // namePrompt will validate that the user's response is between 1 and 50 chars in length
        this.addDialog(new TextPrompt(NAME_PROMPT, async (context, step) => {
            if (!step.recognized) {
                context.sendActivity('Please tell me your name!');
            } else {
                const value = step.recognized.value;
                if (value.length < 1) {
                    context.sendActivity('Your name has to include at least one character.');
                } else if (value.length > 50) {
                    context.sendActivity(`Sorry, but I can only handle names of up to 50 characters.  Yours was ${ value.length }.`);
                } else {
                    step.end(value);
                }
            }
        }));

        // agePrompt will validate an age between 1 and 99
        this.addDialog(new NumberPrompt(AGE_PROMPT, async (context, step) =>{
            if (!step.recognized) {
                context.sendActivity('Please tell me your age!');
            } else {
                const value = step.recognized.value;
                if (value < 1 || value > 99) {
                    context.sendActivity('Please enter an age in years between 1 and 99');
                } else {
                    step.end(value);
                }
            }
        }));

        // dobPrompt will validate a date between 8/24/1918 and 8/24/2018
        const DATE_LOW_BOUNDS = new Date('8/24/1918');
        const DATE_HIGH_BOUNDS = new Date('8/24/2018');
        this.addDialog(new DateTimePrompt(DOB_PROMPT, async (context, step) => {
            console.log('DATE TIME PROMPT', step);
            const values = step.recognized.value;
            console.log('VALUES', values);
            try {
                if (!Array.isArray(values) || values.length < 0) { throw new Error('missing time') }
                if ((values[0].type !== 'datetime') && (values[0].type !== 'date')) { throw new Error('unsupported type') }
                const value = new Date(values[0].value);
                if (value.getTime() < DATE_LOW_BOUNDS.getTime()) {
                    throw new Error('too low');
                } else if (value.getTime() > DATE_HIGH_BOUNDS.getTime()) {
                    throw new Error('too high');
                }
                step.end(value);
            } catch (err) {
                await context.sendActivity(`Answer with a date like 8/8/2018 or say "cancel".`);
            }
        }));
                
        // colorPrompt provides a validation error when a valid choice is not made
        this.addDialog(new ChoicePrompt(COLOR_PROMPT
        , async (context, step) => {
            const choice = step.recognized.value;
            console.log('CHOICE ', choice);
            if (!choice) {
                // an invalid choice was received, emit an error.
                context.sendActivity(`Sorry, "${ context.activity.text }" is not on my list.`);
            }
            step.end();
            // return choice;
        }));

    }


    async collectName(dc, step) {

        // const user_name = await this.userName.get(dc, '');
        // if (user_name) {
            // return dc.begin(HELLO_USER);
        // } else {
            return await dc.prompt(NAME_PROMPT, `What is your name, human?`);
        // }
    }

    async collectAge(dc, step) {
        step.values[USER_NAME_PROP] = step.result;
        return await dc.prompt(AGE_PROMPT, `What is your age?`);
    }

    async collectDob(dc, step) {
        step.values[AGE_PROP] = step.result;
        return await dc.prompt(DOB_PROMPT, `What is your date of birth?`);
    }

    async collectColor(dc, step) {
        step.values[DOB_PROP] = step.result;
        const choices = ['red','blue','green'];
        return await dc.prompt(COLOR_PROMPT, `Finally, what is your favorite color?`, choices);
    }

    async lastStep(dc, step) {
        step.values[COLOR_PROP] = step.result;
        await step.next();
    }

    async confirmStep(dc, step) {

        // this.userName.set(dc, step.values[USER_NAME_PROP]);
        // this.userAge.set(dc, step.values[AGE_PROP]);
        // this.userDob.set(dc, step.values[DOB_PROP]);
        // this.userColor.set(dc, step.values[COLOR_PROP]);

        await dc.context.sendActivity(`Your profile is complete! Thank you.`);
        return await dc.end();
    }


}

module.exports = OnboardingDialog;