const { ComponentDialog } = require('botbuilder-dialogs');
const { NumberInput, AttachmentInput, ConfirmInput, IfCondition, ActivityTemplate, AdaptiveDialog, TextInput, ChoiceInput, OnUnknownIntent, SendActivity, TemplateEngineLanguageGenerator, OnBeginDialog } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const { StringExpression, BoolExpression } = require('adaptive-expressions');

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
                    // Ask for user's age and set it in user.userProfile scope.
                    new TextInput().configure(
                    {
                        // Set the output of the text input to this property in memory.
                        property: new StringExpression("user.userProfile.Transport"),
                        prompt: new ActivityTemplate("${ModeOfTransportPrompt()}")
                    }),
                    new TextInput().configure(
                    {
                        property: new StringExpression("user.userProfile.Name"),
                        prompt: new ActivityTemplate("${AskForName()}")
                    }),
                    // SendActivity supports full language generation resolution.
                    // See here to learn more about language generation
                    // https://aka.ms/language-generation
                    new SendActivity("${AckName()}"),
                    new ConfirmInput().configure(
                    {
                        property: new StringExpression("turn.ageConfirmation"),
                        prompt: new ActivityTemplate("${AgeConfirmPrompt()}")
                    }),
                    new IfCondition().configure(
                    {
                        // All conditions are expressed using the common expression language.
                        // See https://aka.ms/adaptive-expressions to learn more
                        condition: new BoolExpression("turn.ageConfirmation == true"),
                        actions: [
                            new NumberInput().configure(
                            {
                                prompt: new ActivityTemplate("${AskForAge()}"),
                                property: new StringExpression("user.userProfile.Age"),
                                // Add validations
                                validations: [
                                    // Age must be greater than or equal 1
                                    "int(this.value) >= 1",
                                    // Age must be less than 150
                                    "int(this.value) < 150"
                                ],
                                invalidPrompt: new ActivityTemplate("${AskForAge.invalid()}"),
                                unrecognizedPrompt: new ActivityTemplate("${AskForAge.unRecognized()}")
                            }),
                            new SendActivity("${UserAgeReadBack()}")
                        ],
                        elseActions: [
                            new SendActivity("${NoAge()}") 
                        ]
                    }),
                    new IfCondition().configure(
                    {
                        condition: new BoolExpression("turn.activity.channelId == 'msteams'"),
                        actions: [
                            // This attachment prompt example is not designed to work for Teams attachments, so skip it in this case
                            new SendActivity('Skipping attachment prompt in Teams channel...')
                        ],
                        elseActions: [
                            new AttachmentInput().configure(
                            {
                                prompt: new ActivityTemplate("${AskForImage()}"),
                                property: new StringExpression("user.userProfile.picture"),
                                validations: [
                                    "this.value.contentType == 'image/jpeg' || this.value.contentType == 'image/png'"
                                ],
                                invalidPrompt: new ActivityTemplate("${AskForImage.Invalid()}")
                            })
                        ]
                    }),
                    new ConfirmInput().configure(
                    {
                        prompt: new ActivityTemplate("${ConfirmPrompt()}"),
                        property: new StringExpression("turn.finalConfirmation")
                    }),
                    // Use LG template to come back with the final read out.
                    // This LG template is a great example of what logic can be wrapped up in LG sub-system.
                    new SendActivity("${FinalUserProfileReadOut()}"), // examines turn.finalConfirmation
                ])
            ]
        });
        this.addDialog(userProfileAdaptiveDialog);
        this.initialDialogId = ROOT_DIALOG;
    }
}

module.exports.UserProfileDialog = UserProfileDialog;
