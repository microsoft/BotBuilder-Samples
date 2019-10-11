// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    ActionTypes,
    Activity,
    CardAction,
    CardFactory,
    MessageFactory,
    TeamsActivityHandler,
    TurnContext
} from 'botbuilder';

export class CardsBot extends TeamsActivityHandler {
    // NOT SUPPORTED ON TEAMS: AnimationCard, AudioCard, VideoCard, OAuthCard
    protected cardTypes: string[];
    constructor() {
        super();
        /*
         * From the UI you can @mention the bot, from any scope, any of the strings listed below to get that card back.
         */
        const HeroCard = 'Hero';
        const ThumbnailCard = 'Thumbnail';
        const ReceiptCard = 'Receipt';
        const SigninCard = 'Signin';
        const Carousel = 'Carousel';
        const List = 'List';
        this.cardTypes = [HeroCard, ThumbnailCard, ReceiptCard, SigninCard, Carousel, List];

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            const text = context.activity.text.trim().split(' ').splice(-1)[0];
            await context.sendActivity('You said ' + text);

            const activity = context.activity;
            TurnContext.removeRecipientMention(activity);
            let reply: Partial<Activity>;
            switch (text.toLowerCase()) {
                case HeroCard.toLowerCase():
                    reply = MessageFactory.attachment(this.getHeroCard());
                    break;
                case ThumbnailCard.toLowerCase():
                    reply = MessageFactory.attachment(this.getThumbnailCard());
                    break;
                case ReceiptCard.toLowerCase():
                    reply = MessageFactory.attachment(this.getReceiptCard());
                    break;
                case SigninCard.toLowerCase():
                    reply = MessageFactory.attachment(this.getSigninCard());
                    break;
                case Carousel.toLowerCase():
                    // NOTE: if cards are NOT the same height in a carousel, Teams will instead display as AttachmentLayoutTypes.List
                    reply = MessageFactory.carousel([this.getHeroCard(), this.getHeroCard(), this.getHeroCard()]);
                    break;
                case List.toLowerCase():
                    // NOTE: MessageFactory.Attachment with multiple attachments will default to AttachmentLayoutTypes.List
                    reply = MessageFactory.list([this.getHeroCard(), this.getHeroCard(), this.getHeroCard()]);
                    break;

                default:
                    reply = MessageFactory.attachment(this.getChoices());
                    break;
            }

            await context.sendActivity(reply);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    private getHeroCard() {
        return CardFactory.heroCard('BotFramework Hero Card',
            'Build and connect intelligent bots to interact with your users naturally wherever they are,' +
            ' from text/sms to Skype, Slack, Office 365 mail and other popular services.',
            ['https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg'],
            [{ type: ActionTypes.OpenUrl, title: 'Get Started', value: 'https://docs.microsoft.com/bot-framework' }]);
    }

    private getThumbnailCard() {
        return CardFactory.thumbnailCard('BotFramework Thumbnail Card',
            'Build and connect intelligent bots to interact with your users naturally wherever they are,' +
            ' from text/sms to Skype, Slack, Office 365 mail and other popular services.',
            ['https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg'],
            [{ type: ActionTypes.OpenUrl, title: 'Get Started', value: 'https://docs.microsoft.com/bot-framework' }]);
    }

    private getReceiptCard() {
        return CardFactory.receiptCard({
            buttons: [
                {
                    image: 'https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png',
                    title: 'More information',
                    type: ActionTypes.OpenUrl,
                    value: 'https://azure.microsoft.com/en-us/pricing/'
                }
            ],
            facts: [
                { key: 'Order Number', value: '1234' },
                { key: 'Payment Method', value: 'VISA 5555-****' }
            ],
            items: [
                {
                    image: { url: 'https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png' },
                    price: '$ 38.45',
                    quantity: '368',
                    subtitle: '',
                    tap: { title: '', type: '', value: null },
                    text: '',
                    title: 'Data Transfer'
                },
                {
                    image: { url: 'https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png' },
                    price: '$ 45.00',
                    quantity: '720',
                    subtitle: '',
                    tap: { title: '', type: '', value: null },
                    text: '',
                    title: 'App Service'
                }
            ],
            tap: { title: '', type: '', value: null },
            tax: '$ 7.50',
            title: 'John Doe',
            total: '$ 90.95',
            vat: ''
        });
    }

    private getSigninCard() {
        return CardFactory.signinCard('BotFramework Sign-in Card', 'https://login.microsoftonline.com/', 'Sign-in');
    }

    private getChoices() {
        const actions = this.cardTypes.map((cardType) => ({ type: ActionTypes.MessageBack, title: cardType, text: cardType })) as CardAction[];
        return CardFactory.heroCard('Task Module Invocation from Hero Card', null, actions);
    }
}
