// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
import { StatePropertyAccessor, TurnContext, UserState } from 'botbuilder';
import {
    ChoiceFactory,
    ChoicePrompt,
    ComponentDialog,
    ConfirmPrompt,
    DialogSet,
    DialogTurnStatus,
    NumberPrompt,
    PromptValidatorContext,
    TextPrompt,
    WaterfallDialog,
    WaterfallStepContext
} from 'botbuilder-dialogs';
import { UserProfile } from '../userProfile';

const CHOICE_PROMPT = 'CHOICE_PROMPT';
const CONFIRM_PROMPT = 'CONFIRM_PROMPT';
const NAME_PROMPT = 'NAME_PROMPT';
const NUMBER_PROMPT = 'NUMBER_PROMPT';
const USER_PROFILE = 'USER_PROFILE';
const WATERFALL_DIALOG = 'WATERFALL_DIALOG';

export class UserProfileDialog extends ComponentDialog {
    private userProfile: StatePropertyAccessor<UserProfile>;

    constructor(userState: UserState) {
        super('userProfileDialog');

        this.userProfile = userState.createProperty(USER_PROFILE);

        this.addDialog(new TextPrompt(NAME_PROMPT));
        this.addDialog(new ChoicePrompt(CHOICE_PROMPT));
        this.addDialog(new ConfirmPrompt(CONFIRM_PROMPT));
        this.addDialog(new NumberPrompt(NUMBER_PROMPT, this.agePromptValidator));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.transportStep.bind(this),
            this.nameStep.bind(this),
            this.nameConfirmStep.bind(this),
            this.ageStep.bind(this),
            this.confirmStep.bind(this),
            this.summaryStep.bind(this)
        ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    public async run(turnContext: TurnContext, accessor: StatePropertyAccessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    private async transportStep(stepContext: WaterfallStepContext) {
        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        // Running a prompt here means the next WaterfallStep will be run when the users response is received.
        return await stepContext.prompt(CHOICE_PROMPT, {
            choices: ChoiceFactory.toChoices(['Car', 'Bus', 'Bicycle']),
            prompt: 'Please enter your mode of transport.'
        });
    }

    private async nameStep(stepContext: WaterfallStepContext<UserProfile>) {
        stepContext.options.transport = stepContext.result.value;
        return await stepContext.prompt(NAME_PROMPT, 'What is your name, human?');
    }

    private async nameConfirmStep(stepContext: WaterfallStepContext<UserProfile>) {
        stepContext.options.name = stepContext.result;

        // We can send messages to the user at any point in the WaterfallStep.
        await stepContext.context.sendActivity(`Thanks ${stepContext.result}.`);

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        return await stepContext.prompt(CONFIRM_PROMPT, 'Do you want to give your age?', ['yes', 'no']);
    }

    private async ageStep(stepContext: WaterfallStepContext) {
        if (stepContext.result === true) {
            // User said "yes" so we will be prompting for the age.
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            const promptOptions = { prompt: 'Please enter your age.', retryPrompt: 'The value entered must be greater than 0 and less than 150.' };

            return await stepContext.prompt(NUMBER_PROMPT, promptOptions);
        } else {
            // User said "no" so we will skip the next step. Give -1 as the age.
            return await stepContext.next(-1);
        }
    }

    private async confirmStep(stepContext: WaterfallStepContext<UserProfile>) {
        stepContext.options.age = stepContext.result;

        const msg = stepContext.options.age === -1 ? 'No age given.' : `I have your age as ${stepContext.options.age}.`;

        // We can send messages to the user at any point in the WaterfallStep.
        await stepContext.context.sendActivity(msg);

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
        return await stepContext.prompt(CONFIRM_PROMPT, { prompt: 'Is this okay?' });
    }

    private async summaryStep(stepContext: WaterfallStepContext<UserProfile>) {
        if (stepContext.result) {
            // Get the current profile object from user state.
            const userProfile = await this.userProfile.get(stepContext.context, new UserProfile());
            const stepContextOptions = stepContext.options;
            userProfile.transport = stepContextOptions.transport;
            userProfile.name = stepContextOptions.name;
            userProfile.age = stepContextOptions.age;

            let msg = `I have your mode of transport as ${userProfile.transport} and your name as ${userProfile.name}.`;
            if (userProfile.age !== -1) {
                msg += ` And age as ${userProfile.age}.`;
            }

            await stepContext.context.sendActivity(msg);
        } else {
            await stepContext.context.sendActivity('Thanks. Your profile will not be kept.');
        }

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
        return await stepContext.endDialog();
    }

    private async agePromptValidator(promptContext: PromptValidatorContext<number>) {
        // This condition is our validation rule. You can also change the value at this point.
        return promptContext.recognized.succeeded && promptContext.recognized.value > 0 && promptContext.recognized.value < 150;
    }
}
