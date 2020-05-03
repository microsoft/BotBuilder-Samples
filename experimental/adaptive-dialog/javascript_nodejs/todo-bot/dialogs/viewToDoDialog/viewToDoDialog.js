const { ComponentDialog } = require('botbuilder-dialogs');
const { AdaptiveDialog, OnBeginDialog, SendActivity, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');

const path = require('path');

const DIALOG_ID = 'VIEW_TO_DO_DIALOG';

class ViewToDoDialog extends ComponentDialog {
    constructor() {
        super(DIALOG_ID);
        const lgFile = Templates.parseFile(path.join(__dirname, 'viewToDoDialog.lg'));
        const dialog = new AdaptiveDialog(DIALOG_ID).configure({
            generator: new TemplateEngineLanguageGenerator(lgFile),
            triggers: [
                new OnBeginDialog([
                    new SendActivity('${ViewToDos()}')
                ])
            ]
        });

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        this.addDialog(dialog);

        // The initial child Dialog to run.
        this.initialDialogId = DIALOG_ID;
    }
}

module.exports.ViewToDoDialog = ViewToDoDialog;
