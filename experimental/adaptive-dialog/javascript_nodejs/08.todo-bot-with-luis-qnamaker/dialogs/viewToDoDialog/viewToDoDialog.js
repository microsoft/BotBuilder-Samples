// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog } = require('botbuilder-dialogs');
const { QnAMakerRecognizer, CrossTrainedRecognizerSet, TextInput, OnQnAMatch, ActivityTemplate, IfCondition, LuisAdaptiveRecognizer, AdaptiveDialog, OnBeginDialog, SendActivity, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const { IntExpression, ValueExpression, BoolExpression, StringExpression } = require('adaptive-expressions');
const path = require('path');

const DIALOG_ID = 'viewToDoDialog';

class ViewToDoDialog extends ComponentDialog {
    constructor() {
        super(DIALOG_ID);
        const lgFile = Templates.parseFile(path.join(__dirname, 'viewToDoDialog.lg'));
        const dialog = new AdaptiveDialog(DIALOG_ID).configure({
            generator: new TemplateEngineLanguageGenerator(lgFile),
            recognizer: this.createCrossTrainedRecognizer(),
            triggers: [
                new OnBeginDialog([
                    // See if any list has any items.
                    new IfCondition().configure(
                    {
                        condition: new BoolExpression("count(user.lists.todo) != 0 || count(user.lists.grocery) != 0 || count(user.lists.shopping) != 0"),
                        actions: [
                            // Get list type
                            new TextInput().configure(
                            {
                                property: new StringExpression("dialog.listType"),
                                prompt: new ActivityTemplate("${GetListType()}"),
                                value: new ValueExpression("=@listType"),
                                allowInterruptions: new BoolExpression("!@listType && turn.recognized.score >= 0.7"),
                                validations: [
                                    // Verify using expressions that the value is one of todo or shopping or grocery
                                    "contains(createArray('todo', 'shopping', 'grocery', 'all'), toLower(this.value))",
                                ],
                                outputFormat: new StringExpression("=toLower(this.value)"),
                                invalidPrompt: new ActivityTemplate("${GetListType.Invalid()}"),
                                maxTurnCount: new IntExpression(2),
                                defaultValue: new ValueExpression("todo"),
                                defaultValueResponse: new ActivityTemplate("${GetListType.DefaultValueResponse()}")
                            }),
                            new SendActivity("${ShowList()}")
                        ],
                        elseActions: [
                            new SendActivity("${NoItemsInLists()}")
                        ]
                    }),
                ]),
                // Help and chitchat is handled by qna
                new OnQnAMatch([
                    new SendActivity("${@Answer}")
                ])
            ]
        });

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        this.addDialog(dialog);

        // The initial child Dialog to run.
        this.initialDialogId = DIALOG_ID;
    }

    static dialogName = DIALOG_ID;

    createLuisRecognizer() {
        if (process.env.ViewToDoDialog_en_us_lu === "" || process.env.LuisAPIHostName === "" || process.env.LuisAPIKey === "")
            throw `Sorry, you need to configure your LUIS application and update .env file.`;
        const recognizer = new LuisAdaptiveRecognizer();
        recognizer.endpoint = new StringExpression(process.env.LuisAPIHostName);
        recognizer.endpointKey = new StringExpression(process.env.LuisAPIKey);
        recognizer.applicationId = new StringExpression(process.env.ViewToDoDialog_en_us_lu);

        // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
        recognizer.id = `LUIS_${DIALOG_ID}`;
        return recognizer;
    }

    createQnARecognizer() {
        if (process.env.TodoBotWithLuisAndQnAJS_en_us_qna === "" || process.env.QnAHostName === "" || process.env.QnAEndpointKey === "")
            throw `Sorry, you need to configure your QnA Maker KB and update .env file.`;
        let qnaRecognizer = new QnAMakerRecognizer();
        qnaRecognizer.hostname = new StringExpression(process.env.QnAHostName);
        qnaRecognizer.knowledgeBaseId = new StringExpression(process.env.TodoBotWithLuisAndQnAJS_en_us_qna);
        qnaRecognizer.endpointKey = new StringExpression(process.env.QnAEndpointKey);

        // Property path where previous qna id is set. This is required to have multi-turn QnA working.
        qnaRecognizer.qnaId = new IntExpression("turn.qnaIdFromPrompt");

        // Id needs to be QnA_<dialogName> for cross-trained recognizer to work.
        qnaRecognizer.id = `QnA_${DIALOG_ID}`;
        return qnaRecognizer;
    }

    createCrossTrainedRecognizer() {
        let recognizer = new CrossTrainedRecognizerSet();
        recognizer.recognizers = [
            this.createLuisRecognizer(),
            this.createQnARecognizer()
        ];
        return recognizer;
    }

};

module.exports.ViewToDoDialog = ViewToDoDialog;