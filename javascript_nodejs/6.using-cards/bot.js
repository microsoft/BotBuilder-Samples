// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory } = require('botbuilder');
const { ChoicePrompt, DialogSet, DialogTurnResult, DialogTurnStatus, ListStyle } = require('botbuilder-dialogs');

/**
 * RichCardsBot prompts a user to select a Rich Card and then returns the card that matches the user's selection.
 */
class RichCardsBot {
    
    /**
     * Constructs the three pieces necessary by this bot to operate:
     * 1. StatePropertyAccessor
     * 2. DialogSet
     * 3. ChoicePrompt
     * 
     * The only argument taken by this constructor is a ConversationState instance, although any BotState instance would suffice for this bot. `conversationState` is used to create a StatePropertyAccessor, which is needed to create a DialogSet. All botbuilder-dialogs `Prompts` need a DialogSet to operate.
     * @param {ConversationState} conversationState 
     */
    constructor(conversationState) {
        // DialogState property accessor. Used to keep persist DialogState when using DialogSet.
        this.dialogState = conversationState.createProperty('dialogState');

        // Create a DialogSet that will be contain the ChoicePrompt.
        this.dialogs = new DialogSet(this.dialogState);

        // Create the ChoicePrompt with a unique id of 'cardPrompt' which will be used to call the dialog in
        // the bot's onTurn logic. 
        // Set the choice rendering to list and then add it to the bot's DialogSet.
        const prompt = new ChoicePrompt('cardPrompt');
        prompt.style = ListStyle.list;
        this.dialogs.add(prompt);
    }

    /**
     * Prompts the user if the user is not in the middle of a dialog, reprompts a user when invalid
     * input is provided, or sends back to the user a Rich Card response after a valid prompt reply.
     * @param {TurnContext} turnContext 
     */
    async onTurn(turnContext) {
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Construct a DialogContext instance which will be used to continue during a DialogStack
            // and prompt users.
            const dc = await this.dialogs.createContext(turnContext);

            const results = await dc.continue();
            if (!turnContext.responded && results.status === DialogTurnStatus.empty) {
                await turnContext.sendActivity('Welcome to the Rich Cards Bot!');
                // Create the PromptOptions which contain the prompt and reprompt messages.
                // PromptOptions also contains the valid list of choices available to the user
                // for selecting.
                const promptOptions = {
                    prompt: 'Please select a card:',
                    reprompt: 'That was not a valid choice, please select a card or number from 1 to 7.',
                    choices: this.getChoices()
                };

                // Prompt the user with the configured PromptOptions.
                await dc.prompt('cardPrompt', promptOptions);
            
            // The bot parsed a valid response from user's prompt response and so it must respond.
            } else if (results.status === DialogTurnStatus.complete) {
                await this.sendCardResponse(turnContext, results);
            }
        }
    }

    /**
     * Handles the user's valid prompt responses and sends back to the user either a Rich Card, or many Rich Cards.
     * 
     * This method is only ever called when a valid prompt response is parsed from the user's response to the ChoicePrompt.
     * @param {TurnContext} turnContext
     * @param {DialogTurnResult} dialogTurnResult 
     */
    async sendCardResponse(turnContext, dialogTurnResult) {
        switch (dialogTurnResult.result.value) {
            case 'Animation card':
                await turnContext.sendActivity({ attachments: [this.createAnimationCard()] });
                break;
            case 'Audio card':
                await turnContext.sendActivity({ attachments: [this.createAudioCard()] });
                break;
            case 'Hero card':
                await turnContext.sendActivity({ attachments: [this.createHeroCard()] });
                break;
            case 'Receipt card':
                await turnContext.sendActivity({ attachments: [this.createReceiptCard()] });
                break;
            case 'Signin card':
                await turnContext.sendActivity({ attachments: [this.createSignInCard()] });
                break;
            case 'Thumbnail card':
                await turnContext.sendActivity({ attachments: [this.createThumbnailCard()] });
                break;
            case 'Video card':
                await turnContext.sendActivity({ attachments: [this.createVideoCard()] });
                break;
            case 'All cards':
                await turnContext.sendActivities([
                    { attachments: [this.createAnimationCard()] },
                    { attachments: [this.createAudioCard()] },
                    { attachments: [this.createHeroCard()] },
                    { attachments: [this.createReceiptCard()] },
                    { attachments: [this.createSignInCard()] },
                    { attachments: [this.createThumbnailCard()] },
                    { attachments: [this.createVideoCard()] }
                ]);
                break;
            default:
                await turnContext.sendActivity('Sorry! An invalid selection was parsed. No corresponding Rich Cards were found.');
        }
    }

    /**
     * Create the choices with synonyms that will be rendered for the user during the ChoicePrompt.
     */
    getChoices() {
        const cardOptions = [
            {
                value: 'Animation card',
                synonyms: ['1', 'animation', 'animation card']
            },
            {
                value: 'Audio card',
                synonyms: ['2', 'audio', 'audio card']
            },
            {
                value: 'Hero card',
                synonyms: ['3', 'hero', 'hero card']
            },
            {
                value: 'Receipt card',
                synonyms: ['4', 'receipt', 'receipt card']
            },
            {
                value: 'Signin card',
                synonyms: ['5', 'signin', 'signin card']
            },
            {
                value: 'Thumbnail card',
                synonyms: ['6', 'thumbnail', 'thumbnail card']
            },
            {
                value: 'Video card',
                synonyms: ['7', 'video', 'video card']
            },
            {
                value: 'All cards',
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
                { url: 'http://i.giphy.com/Ki55RUbOV5njy.gif' }
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
            ['http://www.wavlist.com/movies/004/father.wav'],
            CardFactory.actions([
                {
                    type: 'openUrl',
                    title: 'Read More',
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
                    title: 'Get Started',
                    value: 'https://docs.microsoft.com/en-us/azure/bot-service/'
                }
            ])
        );
    }
    
    createReceiptCard() {
        return CardFactory.receiptCard({
            title: "John Doe",
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
                    title: 'More Information',
                    value: 'https://azure.microsoft.com/en-us/pricing/details/bot-service/'
                }
            ])
        })
    }

    createSignInCard() {
        return CardFactory.signinCard(
            'BotFramework Sign-in Card',
            'https://login.microsoftonline.com',
            'Sign-in'
        );
    }
    
    createThumbnailCard() {
        return CardFactory.thumbnailCard(
            'BotFramework Thumbnail Card',
            [{ url: 'https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg' }],
            [{
                type: 'openUrl',
                title: 'Get Started',
                value: 'https://docs.microsoft.com/en-us/azure/bot-service/'
            }],
            {
                subtitle: 'Your bots — wherever your users are talking',
                text: 'Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.'
            }
        )
    }
    
    createVideoCard() {
        return CardFactory.videoCard(
            'Big Buck Bunny',
            [{ url: 'http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4' }],
            [{
                type: 'openUrl',
                title: 'Lean More',
                value: 'https://peach.blender.org/'
            }],
            {
                subtitle: 'by the Blender Institute',
                text: 'Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute, part of the Blender Foundation. Like the foundation\'s previous film Elephants Dream, the film was made using Blender, a free software application for animation made by the same foundation. It was released as an open-source film under Creative Commons License Attribution 3.0.'
            }
        )
    }
}

module.exports = RichCardsBot;
