// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog } = require('botbuilder-dialogs');
const { CancelAllDialogs, ActivityTemplate, ConfirmInput, IfCondition, ForEach, OnConversationUpdateActivity, LuisAdaptiveRecognizer, BeginDialog, OnIntent, AdaptiveDialog, OnBeginDialog, SendActivity, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const { GetUserProfileDialog } = require('../getUserProfileDialog/getUserProfileDialog');
const { BoolExpression, StringExpression } = require('adaptive-expressions');
const path = require('path');

const DIALOG_ID = 'ROOT_DIALOG';

class RootDialog extends ComponentDialog {
    constructor() {
        super(DIALOG_ID);
        const lgFile = Templates.parseFile(path.join(__dirname, 'rootDialog.lg'));
        const dialog = new AdaptiveDialog(DIALOG_ID).configure({
            generator: new TemplateEngineLanguageGenerator(lgFile),
            recognizer: this.createLuisRecognizer(),
            triggers: [
                new OnConversationUpdateActivity(this.welcomeUserSteps()),
                new OnIntent("GetUserProfile", [], [
                    new BeginDialog('GET_USER_PROFILE_DIALOG')
                ]),
                new OnIntent("Help", [], [
                    new SendActivity("${RootHelp()}")
                ]),
                new OnIntent("Cancel", [], [
                    new ConfirmInput().configure({
                        prompt: new ActivityTemplate('${RootCancelConfirm()}'),
                        property: new StringExpression('turn.confirm'),
                        allowInterruptions: new BoolExpression("false")
                    }),
                    new IfCondition().configure(
                    {
                        condition: new BoolExpression("turn.confirm == true"),
                        actions: [
                            new SendActivity("${CancelReadBack()}"),
                            new CancelAllDialogs()
                        ],
                        elseActions: [
                            new SendActivity("${Cancelcancelled()}")
                        ]
                    })
                ])
            ]
        });

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        this.addDialog(dialog);
        this.addDialog(new GetUserProfileDialog())

        // The initial child Dialog to run.
        this.initialDialogId = DIALOG_ID;
    }

    createLuisRecognizer() {
        if (process.env.rootDialog_en_us_lu === "" || process.env.LuisAPIHostName === "" || process.env.LuisAPIKey === "")
            throw `Sorry, you need to configure your LUIS application and update .env file.`;
        const recognizer = new LuisAdaptiveRecognizer();
        recognizer.endpoint = new StringExpression(process.env.LuisAPIHostName);
        recognizer.endpointKey = new StringExpression(process.env.LuisAPIKey);
        recognizer.applicationId = new StringExpression(process.env.getUserProfileDialog_en_us_lu);
        return recognizer;
    }

    welcomeUserSteps() {
        return [
            // Iterate through membersAdded list and greet user added to the conversation.
            new ForEach().configure({
                itemsProperty: new StringExpression('turn.activity.membersAdded'),
                actions: [
                    // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                    // Filter cases where the bot itself is the recipient of the message.
                    new IfCondition().configure({
                        condition: new BoolExpression('$foreach.value.name != turn.activity.recipient.name'),
                        actions: [
                            new SendActivity('${WelcomeUser()}')
                        ]
                    })
                ]
            })
        ];
    }
}

module.exports.RootDialog = RootDialog;
