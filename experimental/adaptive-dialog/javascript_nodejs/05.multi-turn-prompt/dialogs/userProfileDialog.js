const { ComponentDialog } = require('botbuilder-dialogs');
const { ActivityTemplate, AdaptiveDialog, TextInput, ChoiceInput, OnUnknownIntent, SendActivity, TemplateEngineLanguageGenerator, OnBeginDialog } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const { StringExpression } = require('adaptive-expressions');

const path = require('path');

const ROOT_DIALOG = "USER_PROFILE_DIALOG";

class UserProfileDialog extends ComponentDialog {
    constructor() {
        super('userProfileDialog');
        let lgFile = Templates.parseFile(path.join(__dirname, "userProfileDialog.lg"));
        let userProfileAdaptiveDialog = new AdaptiveDialog(ROOT_DIALOG).configure({
            generator: new TemplateEngineLanguageGenerator(lgFile),
            triggers: [
                new OnBeginDialog([
                    new TextInput().configure({
                        property: new StringExpression("dialog.modeOfTransport"),
                        prompt: new ActivityTemplate("${ModeOfTransportPrompt.Text()}")
                    }),
                    new SendActivity("I have ${dialog.modeOfTransport}")
                ])
            ]
        });
        this.addDialog(userProfileAdaptiveDialog);
        this.initialDialogId = ROOT_DIALOG;
    }
}

module.exports.UserProfileDialog = UserProfileDialog;
