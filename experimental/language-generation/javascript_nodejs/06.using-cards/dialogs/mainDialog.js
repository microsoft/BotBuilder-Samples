// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { AttachmentLayoutTypes, CardFactory } = require('botbuilder');
const { ChoicePrompt, ComponentDialog, DialogSet, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const {
    ActivityFactory,
    TemplateEngine
} = require('botbuilder-lg');

const MAIN_WATERFALL_DIALOG = 'mainWaterfallDialog';

class MainDialog extends ComponentDialog {
    constructor() {
        super('MainDialog');

        // Define the main dialog and its related components.
        this.addDialog(new ChoicePrompt('cardPrompt'));
        this.addDialog(new WaterfallDialog(MAIN_WATERFALL_DIALOG, [
            this.choiceCardStep.bind(this),
            this.showCardStep.bind(this)
        ]));

        // The initial child Dialog to run.
        this.initialDialogId = MAIN_WATERFALL_DIALOG;

        this.templateEngine = new TemplateEngine().addFiles([
            "./resources/MainDialog.LG",
            "./resources/Cards.lg"
        ]);
    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    async run(turnContext, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    /**
     * 1. Prompts the user if the user is not in the middle of a dialog.
     * 2. Re-prompts the user when an invalid input is received.
     *
     * @param {WaterfallStepContext} stepContext
     */
    async choiceCardStep(stepContext) {
        console.log('MainDialog.choiceCardStep');

        // Create the PromptOptions which contain the prompt and re-prompt messages.
        // PromptOptions also contains the list of choices available to the user.
        const options = {
            prompt: ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('CardChoice')),
            retryPrompt: ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('CardChoice.Invalid.Prompt')),
            choices: this.getChoices()
        };

        // Prompt the user with the configured PromptOptions.
        return await stepContext.prompt('cardPrompt', options);
    }

    /**
     * Send a Rich Card response to the user based on their choice.
     * This method is only called when a valid prompt response is parsed from the user's response to the ChoicePrompt.
     * @param {WaterfallStepContext} stepContext
     */
    async showCardStep(stepContext) {
        console.log('MainDialog.showCardStep');

        switch (stepContext.result.value) {
        case 'Adaptive Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('AdaptiveCard')));
            break;
        case 'Animation Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('AnimationCard')));
            break;
        case 'Audio Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('AudioCard')));
            break;
        case 'Hero Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('HeroCard')));
            break;
        case 'Receipt Card':
            var data = {
                receiptItems : [
                    {
                        title: 'Data Transfer',
                        price: '$38.45',
                        quantity: 368,
                        image: { url: 'https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png' }
                    },
                    {
                        title: 'App Service',
                        price: '$45.00',
                        quantity: 720,
                        image: { url: 'https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png' }
                    }
                ]
            };
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate("ReceiptCard", data)));
            break;
        case 'Signin Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('SigninCard')));
            break;
        case 'Thumbnail Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('ThumbnailCard')));
            break;
        case 'Video Card':
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('VideoCard')));
            break;
        default:
            await stepContext.context.sendActivity(ActivityFactory.createActivity(this.templateEngine.evaluateTemplate('AllCards')));
            break;
        }

        // Give the user instructions about what to do next
        await stepContext.context.sendActivity('Type anything to see another card.');

        return await stepContext.endDialog();
    }

    /**
     * Create the choices with synonyms to render for the user during the ChoicePrompt.
     * (Indexes and upper/lower-case variants do not need to be added as synonyms)
     */
    getChoices() {
        const cardOptions = [
            {
                value: 'Adaptive Card',
                synonyms: ['adaptive']
            },
            {
                value: 'Animation Card',
                synonyms: ['animation']
            },
            {
                value: 'Audio Card',
                synonyms: ['audio']
            },
            {
                value: 'Hero Card',
                synonyms: ['hero']
            },
            {
                value: 'Receipt Card',
                synonyms: ['receipt']
            },
            {
                value: 'Signin Card',
                synonyms: ['signin']
            },
            {
                value: 'Thumbnail Card',
                synonyms: ['thumbnail', 'thumb']
            },
            {
                value: 'Video Card',
                synonyms: ['video']
            },
            {
                value: 'All Cards',
                synonyms: ['all']
            }
        ];

        return cardOptions;
    }
}

module.exports.MainDialog = MainDialog;
