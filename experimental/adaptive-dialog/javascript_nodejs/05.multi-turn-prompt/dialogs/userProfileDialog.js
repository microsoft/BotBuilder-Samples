const { ComponentDialog } = require('botbuilder-dialogs');
const { AdaptiveDialog, OnUnknownIntent, SendActivity, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const ROOT_DIALOG = "USER_PROFILE_DIALOG";

class UserProfileDialog extends ComponentDialog {
    constructor() {
        super('userProfileDialog');
        let userProfileAdaptiveDialog = new AdaptiveDialog(ROOT_DIALOG);
        userProfileAdaptiveDialog.generator = new TemplateEngineLanguageGenerator()
        userProfileAdaptiveDialog.triggers.push(new OnUnknownIntent([
            new SendActivity("You said ${turn.activity.text}")
        ]));
        this.addDialog(userProfileAdaptiveDialog);
        this.initialDialogId = ROOT_DIALOG;
    }
}

module.exports.UserProfileDialog = UserProfileDialog;
