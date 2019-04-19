
const { CardFactory, ActionTypes } = require('botbuilder');
const { ComponentDialog, DialogSet, DialogTurnStatus, WaterfallDialog, ChoicePrompt } = require('botbuilder-dialogs');
const MAIN_WATERFALL_DIALOG = 'mainWaterfallDialog';

//These are the options provided to the user when they message the bot
const facebookPageNameOption = "Facebook Page Name";
const quickRepliesOption = "Quick Replies";
const postBackOption = "PostBack";


class MainDialog extends ComponentDialog {
    constructor(logger) {
        super('MainDialog');

        if (!logger) {
            logger = console;
            logger.log('[MainDialog]: logger not passed in, defaulting to console');
        }

        this.logger = logger;

        this.addDialog(new ChoicePrompt('ChoicePrompt'));
        this.addDialog(new WaterfallDialog(MAIN_WATERFALL_DIALOG, [
            this.choiceStep.bind(this),
            this.showResultStep.bind(this)
        ]));

        // The initial child Dialog to run.
        this.initialDialogId = MAIN_WATERFALL_DIALOG;
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


    async choiceStep(stepContext) {
        this.logger.log("MainDialog.choiceStep");
        return await this.showChoices(stepContext);
    }


    async showChoices(stepContext) {
        // Create options for the prompt
        const options = {
            prompt: "What Facebook feature would you like to try? Here are some quick replies to choose from!",
            retryPrompt: "Retry",
            choices: [
                {
                    value: quickRepliesOption
                },
                {
                    value: facebookPageNameOption
                },
                {
                    value: postBackOption
                }
            ]
        };

        return await stepContext.prompt('ChoicePrompt', options);
    }


    async showResultStep(stepContext) {
        this.logger.log("MainDialog.showResultStep");
        var turnContext = stepContext.context;

        //Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
        switch (stepContext.result.value) {

            // Here we showcase how to obtain the Facebook page name.
            // This can be useful for the Facebook multi-page support provided by the Bot Framework.
            // The Facebook page name from which the message comes from is in turnContext.Activity.Recipient.Name.
            case facebookPageNameOption:

                await turnContext.sendActivity(`This message comes from the following Facebook Page: ${turnContext.activity.recipient.name}`);
                break;

            // Here we send a HeroCard with 2 options that will trigger a Facebook PostBack.   
            case postBackOption:
                var card = CardFactory.heroCard(
                    'Is 42 the answer to the ultimate question of Life, the Universe, and Everything?',
                    null,
                    CardFactory.actions([
                        {
                            type: ActionTypes.PostBack,
                            title: 'Yes',
                            value: 'Yes'
                        },

                        {
                            type: ActionTypes.PostBack,
                            title: 'No',
                            value: 'No'
                        }
                    ])
                );

                var reply = {
                    attachments: [
                        card
                    ]
                };
                await turnContext.sendActivity(reply);
                break;
                
            // By default we offer the users different actions that the bot supports, through quick replies. 
            case quickRepliesOption:
                await turnContext.sendActivity("You have selected the Quick Replies option");
                break;
            default:
                await turnContext.sendActivity("I do not recognize the choice");
                break;

        }
        return await stepContext.endDialog();
    }
}

module.exports.MainDialog = MainDialog;