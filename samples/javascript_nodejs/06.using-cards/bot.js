// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { AttachmentLayoutTypes, ActivityTypes, CardFactory } = require('botbuilder');
const { ChoicePrompt, DialogSet, DialogTurnStatus, ListStyle } = require('botbuilder-dialogs');

/**
 * RichCardsBot prompts a user to select a Rich Card and then returns the card
 * that matches the user's selection.
 */
class RichCardsBot {
    /**
     * Constructs the three pieces necessary for this bot to operate:
     * 1. StatePropertyAccessor
     * 2. DialogSet
     * 3. ChoicePrompt
     *
     * The only argument taken (and required!) by this constructor is a
     * ConversationState instance.
     * The ConversationState is used to create a BotStatePropertyAccessor
     * which is needed to create a DialogSet that houses the ChoicePrompt.
     * @param {ConversationState} conversationState The state that will contain the DialogState BotStatePropertyAccessor.
     */
    constructor(conversationState) {
        // Store the conversationState to be able to save state changes.
        this.conversationState = conversationState;
        // Create a DialogState StatePropertyAccessor which is used to
        // persist state using dialogs.
        this.dialogState = conversationState.createProperty('dialogState');

        // Create a DialogSet that contains the ChoicePrompt.
        this.dialogs = new DialogSet(this.dialogState);

        // Create the ChoicePrompt with a unique id of 'cardPrompt' which is
        // used to call the dialog in the bot's onTurn logic.
        const prompt = new ChoicePrompt('cardPrompt');

        // Set the choice rendering to list and then add it to the bot's DialogSet.
        prompt.style = ListStyle.list;
        this.dialogs.add(prompt);
    }

    /**
     * Driver code that does one of the following:
     * 1. Prompts the user if the user is not in the middle of a dialog.
     * 2. Re-prompts a user when an invalid input is received.
     * 3. Sends back to the user a Rich Card response after a valid prompt reply.
     *
     * These three scenarios are preceded by an Activity type check.
     * This check ensures that the bot only responds to Activities that
     * are of the "Message" type.
     *
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async onTurn(turnContext) {
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Construct a DialogContext instance which is used to resume any
            // existing Dialogs and prompt users.
            const dc = await this.dialogs.createContext(turnContext);

            const results = await dc.continueDialog();
            if (!turnContext.responded && results.status === DialogTurnStatus.empty) {
                await turnContext.sendActivity('Welcome to the Rich Cards Bot!');
                // Create the PromptOptions which contain the prompt and re-prompt messages.
                // PromptOptions also contains the list of choices available to the user.
                const promptOptions = {
                    prompt: 'Please select a card:',
                    retryPrompt: 'That was not a valid choice, please select a card or number from 1 to 8.',
                    choices: this.getChoices()
                };

                // Prompt the user with the configured PromptOptions.
                await dc.prompt('cardPrompt', promptOptions);

            // The bot parsed a valid response from user's prompt response and so it must respond.
            } else if (results.status === DialogTurnStatus.complete) {
                await this.sendCardResponse(turnContext, results);
            }
            await this.conversationState.saveChanges(turnContext);
        }
    }

    /**
     * Send a Rich Card response to the user based on their choice.
     *
     * This method is only called when a valid prompt response is parsed from the user's response to the ChoicePrompt.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     * @param {DialogTurnResult} dialogTurnResult Contains the result from any called Dialogs and indicates the status of the DialogStack.
     */
    async sendCardResponse(turnContext, dialogTurnResult) {
        switch (dialogTurnResult.result.value) {
        case 'Animation Card':
            await turnContext.sendActivity({ attachments: [this.createAnimationCard()] });
            break;
        case 'Audio Card':
            await turnContext.sendActivity({ attachments: [this.createAudioCard()] });
            break;
        case 'Hero Card':
            await turnContext.sendActivity({ attachments: [this.createHeroCard()] });
            break;
        case 'Receipt Card':
            await turnContext.sendActivity({ attachments: [this.createReceiptCard()] });
            break;
        case 'Signin Card':
            await turnContext.sendActivity({ attachments: [this.createSignInCard()] });
            break;
        case 'Thumbnail Card':
            await turnContext.sendActivity({ attachments: [this.createThumbnailCard()] });
            break;
        case 'Video Card':
            await turnContext.sendActivity({ attachments: [this.createVideoCard()] });
            break;
        case 'All Cards':
            await turnContext.sendActivity({
                attachments: [this.createVideoCard(),
                    this.createAnimationCard(),
                    this.createAudioCard(),
                    this.createHeroCard(),
                    this.createReceiptCard(),
                    this.createSignInCard(),
                    this.createThumbnailCard(),
                    this.createVideoCard()
                ],
                attachmentLayout: AttachmentLayoutTypes.Carousel
            });
            break;
        default:
            await turnContext.sendActivity('An invalid selection was parsed. No corresponding Rich Cards were found.');
        }
    }

    /**
     * Create the choices with synonyms to render for the user during the ChoicePrompt.
     */
    getChoices() {
        const cardOptions = [
            {
                value: 'Animation Card',
                synonyms: ['1', 'animation', 'animation card']
            },
            {
                value: 'Audio Card',
                synonyms: ['2', 'audio', 'audio card']
            },
            {
                value: 'Hero Card',
                synonyms: ['3', 'hero', 'hero card']
            },
            {
                value: 'Receipt Card',
                synonyms: ['4', 'receipt', 'receipt card']
            },
            {
                value: 'Signin Card',
                synonyms: ['5', 'signin', 'signin card']
            },
            {
                value: 'Thumbnail Card',
                synonyms: ['6', 'thumbnail', 'thumbnail card']
            },
            {
                value: 'Video Card',
                synonyms: ['7', 'video', 'video card']
            },
            {
                value: 'All Cards',
                synonyms: ['8', 'all', 'all cards']
            }
        ];

        return cardOptions;
    }

    // ======================================
    // Helper functions used to create cards.
    // ======================================

    createAnimationCard() {
        return CardFactory.animationCard(
            'Microsoft Bot Framework',
            [
                { url: 'https://i.giphy.com/Ki55RUbOV5njy.gif' }
            ],
            [],
            {
                subtitle: 'Animation Card'
            }
        );
    }

    createAudioCard() {
        return CardFactory.audioCard(
            'I am your father',
            ['https://www.mediacollege.com/downloads/sound-effects/star-wars/darthvader/darthvader_yourfather.wav'],
            CardFactory.actions([
                {
                    type: 'openUrl',
                    title: 'Read more',
                    value: 'https://en.wikipedia.org/wiki/The_Empire_Strikes_Back'
                }
            ]),
            {
                subtitle: 'Star Wars: Episode V - The Empire Strikes Back',
                text: 'The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back) is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and Lawrence Kasdan wrote the screenplay, with George Lucas writing the film\'s story and serving as executive producer. The second installment in the original Star Wars trilogy, it was produced by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams, Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.',
                image: 'https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg'
            }
        );
    }

    createHeroCard() {
        return CardFactory.heroCard(
            'BotFramework Hero Card',
            CardFactory.images(['https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg']),
            CardFactory.actions([
                {
                    type: 'openUrl',
                    title: 'Get started',
                    value: 'https://docs.microsoft.com/en-us/azure/bot-service/'
                }
            ])
        );
    }

    createReceiptCard() {
        return CardFactory.receiptCard({
            title: 'John Doe',
            facts: [
                {
                    key: 'Order Number',
                    value: '1234'
                },
                {
                    key: 'Payment Method',
                    value: 'VISA 5555-****'
                }
            ],
            items: [
                {
                    title: 'Data Transfer',
                    price: '$38.45',
                    quantity: 368,
                    image: { url: 'https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png' }
                },
                {
                    title: 'App Service',
                    price: '$45.00',
                    quantity: 720,
                    image: { url: 'https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png' }
                }
            ],
            tax: '$7.50',
            total: '$90.95',
            buttons: CardFactory.actions([
                {
                    type: 'openUrl',
                    title: 'More information',
                    value: 'https://azure.microsoft.com/en-us/pricing/details/bot-service/'
                }
            ])
        });
    }

    createSignInCard() {
        return CardFactory.signinCard(
            'BotFramework Sign in Card',
            'https://login.microsoftonline.com',
            'Sign in'
        );
    }

    createThumbnailCard() {
        return CardFactory.thumbnailCard(
            'BotFramework Thumbnail Card',
            [{ url: 'https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg' }],
            [{
                type: 'openUrl',
                title: 'Get started',
                value: 'https://docs.microsoft.com/en-us/azure/bot-service/'
            }],
            {
                subtitle: 'Your bots — wherever your users are talking.',
                text: 'Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.'
            }
        );
    }

    createVideoCard() {
        return CardFactory.videoCard(
            '2018 Imagine Cup World Championship Intro',
            [{ url: 'https://sec.ch9.ms/ch9/783d/d57287a5-185f-4df9-aa08-fcab699a783d/IC18WorldChampionshipIntro2.mp4' }],
            [{
                type: 'openUrl',
                title: 'Lean More',
                value: 'https://channel9.msdn.com/Events/Imagine-Cup/World-Finals-2018/2018-Imagine-Cup-World-Championship-Intro'
            }],
            {
                subtitle: 'by Microsoft',
                text: 'Microsoft\'s Imagine Cup has empowered student developers around the world to create and innovate on the world stage for the past 16 years. These innovations will shape how we live, work and play.'
            }
        );
    }
}

module.exports.RichCardsBot = RichCardsBot;
