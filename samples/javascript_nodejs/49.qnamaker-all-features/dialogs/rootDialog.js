// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    ComponentDialog,
    DialogSet,
    DialogTurnStatus,
    WaterfallDialog
} = require('botbuilder-dialogs');

const {
    QnAMakerBaseDialog,
    DefaultCardNoMatchResponse,
    DefaultCardNoMatchText,
    DefaultCardTitle,
    DefaultNoAnswer,
    DefaultThreshold,
    DefaultTopN,
    QnAOptions,
    QnADialogResponseOptions
} = require('./qnamakerBaseDialog');

const INITIAL_DIALOG = 'initial-dialog';
const ROOT_DIALOG = 'root-dialog';
const QNAMAKER_BASE_DIALOG = 'qnamaker-base-dailog';

class RootDialog extends ComponentDialog {
    /**
     * Root dialog for this bot.
     * @param {QnAMaker} qnaService A QnAMaker service object.
     */
    constructor(knowledgebaseId, authkey, host) {
        super(ROOT_DIALOG);
        console.log('RootDialog constructor')
        // Initial waterfall dialog.
        this.addDialog(new WaterfallDialog(INITIAL_DIALOG, [
            this.startInitialDialog.bind(this)
        ]));
        this.addDialog(new QnAMakerBaseDialog(knowledgebaseId, authkey, host));
        this.initialDialogId = INITIAL_DIALOG;

    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    async run(context, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(context);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    // This is the first step of the WaterfallDialog.
    // It kicks off the dialog with the QnA Maker with provided options.
    async startInitialDialog(step) {
        console.log('startInitialDialog')
        return await step.beginDialog(QNAMAKER_BASE_DIALOG);
    }
}

module.exports.RootDialog = RootDialog;
