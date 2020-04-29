const { ComponentDialog } = require('botbuilder-dialogs');
const { ActivityTemplate, AdaptiveDialog, TextInput, ChoiceInput, OnUnknownIntent, SendActivity, TemplateEngineLanguageGenerator, OnBeginDialog } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const path = require('path');

const ROOT_DIALOG = "USER_PROFILE_DIALOG";

class UserProfileDialog extends ComponentDialog {
    constructor() {
        super('userProfileDialog');
        let userProfileAdaptiveDialog = new AdaptiveDialog(ROOT_DIALOG);
        console.log(path.join(__dirname, "userProfileDialog.lg"));
        let lgFile = Templates.parseFile(path.join(__dirname, "userProfileDialog.lg"));
        userProfileAdaptiveDialog.generator = new TemplateEngineLanguageGenerator(lgFile);
        userProfileAdaptiveDialog.triggers.push(
            new OnBeginDialog(
                [
                    new TextInput("dialog.modeOfTransport", new ActivityTemplate("${ModeOfTransportPrompt.Text()}")),
                    new SendActivity("I have ${dialog.modeOfTransport}")
                ]
            )
        );
        this.addDialog(userProfileAdaptiveDialog);
        this.initialDialogId = ROOT_DIALOG;
    }
}

module.exports.UserProfileDialog = UserProfileDialog;
