// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Activity,
    Attachment,
    MessageFactory,
    MessagingExtensionAction,
    MessagingExtensionActionResponse,
    TeamDetails,
    TeamsActivityHandler,
    teamsCreateConversation,
    TeamsInfo,
    TurnContext
} from 'botbuilder';

import { AdaptiveCardHelper } from './adaptiveCardHelper';
import { CardResponseHelpers } from './cardResponseHelpers';
import { SubmitExampleData } from './submitExampleData';

export class ActionBasedMessagingExtensionFetchTaskBot extends TeamsActivityHandler {
    /*
     * After installing this bot you will need to click on the 3 dots to pull up the extension menu to select the bot. Once you do you do
     * see the extension pop a task module.
     */
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(`You said '${context.activity.text}'`);
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    protected async onTeamsMessagingExtensionFetchTask(context, query): Promise<MessagingExtensionActionResponse> {
        const response = AdaptiveCardHelper.createTaskModuleAdaptiveCardResponse();
        return response;
    }

    protected async onTeamsMessagingExtensionSubmitAction(context: TurnContext, action: MessagingExtensionAction): Promise<MessagingExtensionActionResponse> {
        const submittedData = action.data as SubmitExampleData;
        const adaptiveCard = AdaptiveCardHelper.toAdaptiveCardAttachment(submittedData);
        const response = CardResponseHelpers.toMessagingExtensionBotMessagePreviewResponse(adaptiveCard);
        return response;
    }

    protected async onTeamsMessagingExtensionBotMessagePreviewEdit(context: TurnContext, action: MessagingExtensionAction): Promise<MessagingExtensionActionResponse> {
        const submitData = AdaptiveCardHelper.toSubmitExampleData(action);
        const response = AdaptiveCardHelper.createTaskModuleAdaptiveCardResponse(
                                                    submitData.Question,
                                                    (submitData.MultiSelect.toLowerCase() === 'true'),
                                                    submitData.Option1,
                                                    submitData.Option2,
                                                    submitData.Option3);
        return response;
    }

    protected async onTeamsMessagingExtensionBotMessagePreviewSend(context: TurnContext, action: MessagingExtensionAction): Promise<MessagingExtensionActionResponse> {
        const submitData: SubmitExampleData = AdaptiveCardHelper.toSubmitExampleData(action);
        const adaptiveCard: Attachment = AdaptiveCardHelper.toAdaptiveCardAttachment(submitData);

        const responseActivity = {type: 'message', attachments: [adaptiveCard] } as Activity;

        try {
            // Send to channel where messaging extension invoked.
            let results = await teamsCreateConversation(context, context.activity.channelData.channel.id, responseActivity);

            // Send card to "General" channel.
            const teamDetails: TeamDetails = await TeamsInfo.getTeamDetails(context);
            results = await teamsCreateConversation(context, teamDetails.id, responseActivity);
        } catch {
            console.error('In group chat or personal scope.');
        }

        // Send card to compose box for the current user.
        const response = CardResponseHelpers.toComposeExtensionResultResponse(adaptiveCard);
        return response;
    }

    protected async onTeamsMessagingExtensionCardButtonClicked(context: TurnContext, obj) {
        const reply = MessageFactory.text('onTeamsMessagingExtensionCardButtonClicked Value: ' + JSON.stringify(context.activity.value));
        await context.sendActivity(reply);
    }
}
