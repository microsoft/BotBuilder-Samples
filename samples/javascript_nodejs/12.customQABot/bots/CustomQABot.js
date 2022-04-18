// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ActivityTypes } = require('botbuilder');
const { CustomQuestionAnswering } = require('botbuilder-ai');

class CustomQABot extends ActivityHandler {
    constructor() {
        super();

        try {
            this.qnaMaker = new CustomQuestionAnswering({
                knowledgeBaseId: process.env.ProjectName,
                endpointKey: process.env.LanguageEndpointKey,
                host: process.env.LanguageEndpointHostName
            });
        } catch (err) {
            console.warn(`QnAMaker Exception: ${ err } Check your QnAMaker configuration in .env`);
        }

        // If a new user is added to the conversation, send them a greeting message
        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    const DefaultWelcomeMessageFromConfig = process.env.DefaultWelcomeMessage;
                    await context.sendActivity(DefaultWelcomeMessageFromConfig?.length > 0 ? DefaultWelcomeMessageFromConfig : 'Hello and Welcome');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        // When a user sends a message, perform a call to the QnA Maker service to retrieve matching Question and Answer pairs.
        this.onMessage(async (context, next) => {
            if (!process.env.ProjectName || !process.env.LanguageEndpointKey || !process.env.LanguageEndpointHostName) {
                const unconfiguredQnaMessage = 'NOTE: \r\n' +
                    'Custom Question Answering is not configured. To enable all capabilities, add `ProjectName`, `LanguageEndpointKey` and `LanguageEndpointHostName` to the .env file. \r\n' +
                    'You may visit https://language.cognitive.azure.com/ to create a Custom Question Answering Project.';

                await context.sendActivity(unconfiguredQnaMessage);
            } else {
                console.log('Calling CQA');

                const enablePreciseAnswer = process.env.EnablePreciseAnswer === 'true';
                const displayPreciseAnswerOnly = process.env.DisplayPreciseAnswerOnly === 'true';
                const response = await this.qnaMaker.getAnswers(context, { enablePreciseAnswer: enablePreciseAnswer });

                // If an answer was received from CQA, send the answer back to the user.
                if (response.length > 0) {
                    var activities = [];

                    var answerText = response[0].answer;

                    // Answer span text has precise answer.
                    var preciseAnswerText = response[0].answerSpan?.text;
                    if (!preciseAnswerText) {
                        activities.push({ type: ActivityTypes.Message, text: answerText });
                    } else {
                        activities.push({ type: ActivityTypes.Message, text: preciseAnswerText });

                        if (!displayPreciseAnswerOnly) {
                            // Add answer to the reply when it is configured.
                            activities.push({ type: ActivityTypes.Message, text: answerText });
                        }
                    }
                    await context.sendActivities(activities);
                    // If no answers were returned from QnA Maker, reply with help.
                } else {
                    await context.sendActivity('No answers were found.');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.CustomQABot = CustomQABot;
