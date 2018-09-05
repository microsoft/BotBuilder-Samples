// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BotState, StatePropertyAccessor, TurnContext, ActivityTypes } from 'botbuilder';
import { QnAMaker, QnAMakerEndpoint, QnAMakerOptions, QnAMakerResult } from 'botbuilder-ai';

/**
 * A simple bot that responds to users' utterances with answers from QnA Maker when possible.
 * If an answer is not found for a user's utterance, the bot will respond with a message that
 * provides examples of questions that the bot should be able to answer with QnA Maker.
 */
export class QnAMakerBot {
    private qnaMaker: QnAMaker;
    
    /**
     * The QnAMakerBot constructor requires one argument (`endpoint`) which is used to create an instance of `QnAMaker`.
     * @param endpoint The basic configuration needed to call QnA Maker. In this sample this configuration is retrieved from the .bot file.
     * @param config An optional parameter that contains additional settings for configuring a `QnAMaker` when calling the service.
     */
    constructor(endpoint: QnAMakerEndpoint, qnaOptions?: QnAMakerOptions) {
        this.qnaMaker = new QnAMaker(endpoint, qnaOptions);
    }

    /**
     * Every conversation turn for our QnA Bot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param turnContext A TurnContext instance, containing all the data needed for processing this conversation turn.
     */
    public async onTurn(turnContext: TurnContext) {
        // By checking the incoming activity's type, the bot only calls QnA Maker in appropriate cases.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Perform a call to the QnA Maker service to retrieve any potentially matching Question and Answer pairs.
            const qnaResults: QnAMakerResult[] = await this.qnaMaker.generateAnswer(turnContext.activity.text);
            // If an answer was received from QnA Maker, send the answer back to the user.
            if (qnaResults[0]) {
                await turnContext.sendActivity(qnaResults[0].answer);
            
            // If no answers were returned from QnA Maker, reply to the user with examples of valid questions.
            } else {
                await turnContext.sendActivity('No QnA Maker answers were found. This example uses a QnA Maker Knowledge Base that focuses on smart light bulbs. To see QnA Maker in action, ask the bot questions like "Why won\'t it turn on?" or say something like "I need help."');
            }

        // If the Activity is a ConversationUpdate, send a greeting message to the user.
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate &&
                   turnContext.activity.recipient.id !== turnContext.activity.membersAdded[0].id) {
            await turnContext.sendActivity(`Welcome to the QnA Maker sample! Ask me a question and I will try to answer it.`);
        
        // Respond to all other Activity types.
        } else if (turnContext.activity.type !== ActivityTypes.ConversationUpdate) {
            await turnContext.sendActivity(`[${turnContext.activity.type}]-type activity detected.`);
        }
    }
}
