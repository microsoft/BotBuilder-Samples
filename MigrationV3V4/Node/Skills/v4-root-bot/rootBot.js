// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ActivityTypes } = require('botbuilder');
const path = require('path');
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

class RootBot extends ActivityHandler {
    constructor(conversationState, skillsConfig, skillClient) {
        super();
        if (!conversationState) throw new Error('[RootBot]: Missing parameter. conversationState is required');
        if (!skillsConfig) throw new Error('[RootBot]: Missing parameter. skillsConfig is required');
        if (!skillClient) throw new Error('[RootBot]: Missing parameter. skillClient is required');

        this.conversationState = conversationState;
        this.skillsConfig = skillsConfig;
        this.skillClient = skillClient;

        this.botId = process.env.MicrosoftAppId;
        if (!this.botId) {
            throw new Error('[RootBot] MicrosoftAppId is not set in configuration');
        }

        // We use two skills in this example.
        const targetSimpleSkillId = process.env.SkillSimpleId;
        const targetBookingSkillId = process.env.SkillBookingId;
        this.targetSimpleSkill = skillsConfig.skills[targetSimpleSkillId];
        this.targetBookingSkill = skillsConfig.skills[targetBookingSkillId];
        if (!this.targetSimpleSkill) {
            throw new Error(`[RootBot] Skill with ID "${ targetSimpleSkillId }" not found in configuration`);
        }

        if (!this.targetBookingSkill) {
            throw new Error(`[RootBot] Skill with ID "${ targetBookingSkillId }" not found in configuration`);
        }

        // Create state property to track the active skill
        this.activeSkillProperty = this.conversationState.createProperty(RootBot.ActiveSkillPropertyName);

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            // Try to get the active skill
            const activeSkill = await this.activeSkillProperty.get(context);

            if (activeSkill) {
                // Send the activity to the skill
                await this.sendToSkill(context, activeSkill);
            } else {
                if (context.activity.text.toLowerCase() === 'yes') {
                    await context.sendActivity('Got it, connecting you to the booking skill...');

                    // Set active skill
                    await this.activeSkillProperty.set(context, this.targetBookingSkill);

                    // Send the activity to the skill
                    await this.sendToSkill(context, this.targetBookingSkill);
                } else if (context.activity.text.toLowerCase() === 'echo') {
                    await context.sendActivity('Got it, connecting you to the simple echo skill...');

                    // Set active skill
                    await this.activeSkillProperty.set(context, this.targetSimpleSkill);

                    // Send the activity to the skill
                    await this.sendToSkill(context, this.targetSimpleSkill);
                } else {
                    await context.sendActivity("If you'd like to book a hotel say 'yes'. Otherwise say 'echo' the echo skill will display your message.");
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onUnrecognizedActivityType(async (context, next) => {
            // Handle EndOfConversation returned by the skill.
            if (context.activity.type === ActivityTypes.EndOfConversation) {
                // Stop forwarding activities to Skill.
                await this.activeSkillProperty.set(context, undefined);

                // Show status message, text and value returned by the skill
                let eocActivityMessage = `Received ${ ActivityTypes.EndOfConversation }.\n\nCode: ${ context.activity.code }`;
                if (context.activity.text) {
                    eocActivityMessage += `\n\nText: ${ context.activity.text }`;
                }

                if (context.activity.value) {
                    eocActivityMessage += `\n\nValue: ${ context.activity.value }`;
                }

                await context.sendActivity(eocActivityMessage);

                // We are back at the root
                await context.sendActivity("Back in the root bot. If you'd like to book a hotel say 'yes'. Otherwise say 'echo' the echo skill will display your message.");

                // Save conversation state
                await this.conversationState.saveChanges(context, true);
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; ++cnt) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello and welcome!');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onDialog(async (context, next) => {
            // Save any state changes. The load happened during the execution of the Dialog.
            await this.conversationState.saveChanges(context);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    async sendToSkill(context, targetSkill) {
        // NOTE: Always SaveChanges() before calling a skill so that any activity generated by the skill
        // will have access to current accurate state.
        await this.conversationState.saveChanges(context, true);

        // route the activity to the skill
        const response = await this.skillClient.postToSkill(this.botId, targetSkill, this.skillsConfig.skillHostEndpoint, context.activity);

        // Check response status
        if (!(response.status >= 200 && response.status <= 299)) {
            throw new Error(`[RootBot]: Error invoking the skill id: "${ targetSkill.id }" at "${ targetSkill.skillEndpoint }" (status is ${ response.status }). \r\n ${ response.body }`);
        }
    }
}

module.exports.RootBot = RootBot;
RootBot.ActiveSkillPropertyName = 'activeSkillProperty';
