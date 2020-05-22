// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog } = require('botbuilder-dialogs');
const { OnDialogEvent, ArrayChangeType, EditArray, OnBeginDialog, QnAMakerRecognizer, CrossTrainedRecognizerSet, TextInput, CodeAction, SetProperty, OnQnAMatch, ActivityTemplate, LuisAdaptiveRecognizer, OnIntent, AdaptiveDialog, SendActivity, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const { EnumExpression, IntExpression, Expression, ValueExpression, BoolExpression, StringExpression } = require('adaptive-expressions');
const { AdaptiveEvents } = require('botbuilder-dialogs-adaptive/lib/adaptiveEvents');
const { ActivityFactory } = require('botbuilder');

const path = require('path');

// Note: Dialog name needs to match with the file name for cross-train CLI commands and the recognizer to work E2E.
const DIALOG_ID = 'addToDoDialog';

class AddToDoDialog extends ComponentDialog {
    constructor() {
        super(DIALOG_ID);
        const lgFile = Templates.parseFile(path.join(__dirname, `${DIALOG_ID}.lg`));
        this.lgGenerator = new TemplateEngineLanguageGenerator(lgFile);
        const dialog = new AdaptiveDialog(DIALOG_ID).configure({
            generator: this.lgGenerator,
            recognizer: this.createCrossTrainedRecognizer(),
            triggers: [
                new OnBeginDialog([
                    // Take todo title if we already have it from root dialog's LUIS model.
                    // This is the title entity defined in ../RootDialog/RootDialog.lu.
                    // There is one LUIS application for this bot. So any entity captured by the rootDialog
                    // will be automatically available to child dialog.
                    // @EntityName is a short-hand for turn.recognized.entities.<EntityName>. Other useful short-hands are 
                    //     #IntentName is a short-hand for turn.intents.<IntentName>
                    //     $PropertyName is a short-hand for dialog.<PropertyName>
                    new SetProperty().configure(
                    {
                        property: new StringExpression("dialog.itemTitle"),
                        value: new ValueExpression("=@itemTitle")
                    }),

                    new SetProperty().configure(
                    {
                        property: new StringExpression("dialog.listType"),
                        value: new ValueExpression("=@listType")
                    }),

                    // TextInput by default will skip the prompt if the property has value.
                    new TextInput().configure(
                    {
                        property: new StringExpression("dialog.itemTitle"),
                        prompt: new ActivityTemplate("${GetItemTitle()}"),
                        // This entity is coming from the local AddToDoDialog's own LUIS recognizer.
                        // This dialog's .lu file is under \AddToDoDialog\AddToDoDialog.lu
                        value: new ValueExpression("=@itemTitle"),
                        // Allow interruption if we do not have an item title and have a super high confidence classification of an intent.
                        allowInterruptions: new BoolExpression("!@itemTitle && turn.recognized.score >= 0.7")
                    }),
                    // Get list type
                    new TextInput().configure(
                    {
                        property: new StringExpression("dialog.listType"),
                        prompt: new ActivityTemplate("${GetListType()}"),
                        value: new ValueExpression("=@listType"),
                        allowInterruptions: new BoolExpression("!@listType && turn.recognized.score >= 0.7"),
                        validations: [
                            // Verify using expressions that the value is one of todo or shopping or grocery
                            "contains(createArray('todo', 'shopping', 'grocery'), toLower(this.value))",
                        ],
                        outputFormat: new StringExpression("=toLower(this.value)"),
                        invalidPrompt: new ActivityTemplate("${GetListType.Invalid()}"),
                        maxTurnCount: new IntExpression(2),
                        defaultValue: new ValueExpression("todo"),
                        defaultValueResponse: new ActivityTemplate("${GetListType.DefaultValueResponse()}")
                    }),
                    // Add the new todo title to the list of todos. Keep the list of todos in the user scope.
                    new EditArray().configure(
                    {
                        itemsProperty: new StringExpression("user.lists[dialog.listType]"),
                        changeType: new EnumExpression(ArrayChangeType.push),
                        value: new ValueExpression("=dialog.itemTitle")
                    }),
                    new SendActivity("${AddItemReadBack()}")
                    // All child dialogs will automatically end if there are no additional steps to execute. 
                    // If you wish for a child dialog to not end automatically, you can set 
                    // AutoEndDialog property on the Adaptive Dialog to 'false'
                ]),
                // Although root dialog can handle this, this will match loacally because this dialog's .lu has local definition for this intent. 
                new OnIntent("Help", [], [
                    new SendActivity("${HelpAddItem()}")
                ]),
                // Shows how to use dialog event to capture intent recognition event for more than one intent.
                // Alternate to this would be to add two separate OnIntent events.
                // This ensures we set any entities recognized by these two intents.
                new OnDialogEvent(AdaptiveEvents.recognizedIntent, [
                    new SetProperty().configure(
                    {
                        property: new StringExpression("dialog.itemTitle"),
                        value: new ValueExpression("=@itemTitle")
                    }),
                    new SetProperty().configure(
                    {
                        property: new StringExpression("dialog.itemTitle"),
                        value: new ValueExpression("=@itemTitle")
                    }),
                ], "#GetItemTitle || #GetListType"),
                                   
                // Help and chitchat is handled by qna
                new OnQnAMatch([
                    // Use code action to render QnA response. This is also a demonstration of how to use code actions to light up custom functionality.
                    new CodeAction(this.resolveAndSendQnAAnswer.bind(this))
                ]),
            ]
        });

        this.addDialog(dialog);

        this.initialDialogId = DIALOG_ID;
    }

    static dialogName = DIALOG_ID;

    createLuisRecognizer() {
        if (process.env.AddToDoDialog_en_us_lu === "" || process.env.LuisAPIHostName === "" || process.env.LuisAPIKey === "")
            throw `Sorry, you need to configure your LUIS application and update .env file.`;
        return new LuisAdaptiveRecognizer().configure(
            {
                endpoint: new StringExpression(process.env.LuisAPIHostName),
                endpointKey: new StringExpression(process.env.LuisAPIKey),
                applicationId: new StringExpression(process.env.AddToDoDialog_en_us_lu),

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                id: `LUIS_${DIALOG_ID}`
            }
        );
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

    // Code action to process response from QnA maker using the generator for this dialog.
    // You can use code action to perform any operation including memory mutations. 
    resolveAndSendQnAAnswer = async function(dc, options)
    {
        let exp1 = Expression.parse("@answer").tryEvaluate(dc.state).value;
        let resVal = await this.lgGenerator.generate(dc, exp1, dc.state);
        await dc.context.sendActivity(ActivityFactory.fromObject(resVal));
        return await dc.endDialog(options);
    }

};

module.exports.AddToDoDialog = AddToDoDialog;
