// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Attachment,
    CardFactory,
    MessagingExtensionActionResponse,
    MessagingExtensionAction,
    MessagingExtensionQuery,
    TaskModuleContinueResponse,
    TaskModuleTaskInfo,
    TaskModuleRequest,
    TeamsActivityHandler,
    TurnContext,
    BotFrameworkAdapter,
} from 'botbuilder';

import {
    IUserTokenProvider,
} from 'botbuilder-core';

/*
* This Bot requires an Azure Bot Service OAuth connection name in appsettings.json
* see: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication
*
* Clicking this bot's Task Menu will retrieve the login dialog, if the user is not already signed in.
*/
export class MessagingExtensionAuthBot extends TeamsActivityHandler {
    connectionName: string;
    constructor(authConnectionName: string) {
        super();
        
        this.connectionName = authConnectionName;

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {

            // Hack around weird behavior of RemoveRecipientMention (it alters the activity.Text)
            const originalText = context.activity.text;
            TurnContext.removeRecipientMention(context.activity);
            const text = context.activity.text.replace(' ', '').toUpperCase();
            context.activity.text = originalText;

            if (text === 'LOGOUT' || text === 'SIGNOUT')
            {
                const adapter: IUserTokenProvider = context.adapter as BotFrameworkAdapter;

                await adapter.signOutUser(context, this.connectionName);
                await context.sendActivity(`Signed Out: ${context.activity.from.name}`);
                
                return;
            }

            await context.sendActivity(`You said '${context.activity.text}'`);
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (const member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello and welcome!');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    protected async onTeamsMessagingExtensionFetchTask(context: TurnContext, query: MessagingExtensionQuery): Promise<MessagingExtensionActionResponse> {
        const adapter: IUserTokenProvider = context.adapter as BotFrameworkAdapter;
        const userToken = await adapter.getUserToken(context, this.connectionName);
        if (!userToken)
        {
            // There is no token, so the user has not signed in yet.

            // Retrieve the OAuth Sign in Link to use in the MessagingExtensionResult Suggested Actions
            const signInLink = await adapter.getSignInLink(context, this.connectionName);

            const response : MessagingExtensionActionResponse = { 
                composeExtension: { 
                    type: 'auth', 
                    suggestedActions: { 
                        actions: [{ 
                            type: 'openUrl', 
                            value: signInLink, 
                            title: 'Bot Service OAuth' 
                        }]
                    }
                }
            };
            return response;
        };

        // User is already signed in.
        const continueResponse : TaskModuleContinueResponse = {
            type: 'continue',
            value: this.CreateSignedInTaskModuleTaskInfo(),
        };

        const response : MessagingExtensionActionResponse = { 
            task: continueResponse 
        }; 

        return response;
    }

    protected async onTeamsTaskModuleFetch(context: TurnContext, taskModuleRequest: TaskModuleRequest): Promise<TaskModuleTaskInfo> {
        var data = context.activity.value;
        if (data && data.state)
        {
            const adapter: IUserTokenProvider = context.adapter as BotFrameworkAdapter;
            const tokenResponse = await adapter.getUserToken(context, this.connectionName, data.state);
            return this.CreateSignedInTaskModuleTaskInfo(tokenResponse.token);
        }
        else
        {
            await context.sendActivity("OnTeamsTaskModuleFetchAsync called without 'state' in Activity.Value");
            return null;
        }
    }

    protected async onTeamsMessagingExtensionSubmitAction(context, action: MessagingExtensionAction): Promise<MessagingExtensionActionResponse> {
        if (action.data != null && action.data.key && action.data.key == "signout")
        {
            // User clicked the Sign Out button from a Task Module
            await (context.adapter as IUserTokenProvider).signOutUser(context, this.connectionName);
            await context.sendActivity(`Signed Out: ${context.activity.from.name}`);
        }

        return null;
    }

    private CreateSignedInTaskModuleTaskInfo(token?: string): TaskModuleTaskInfo {
        const attachment = this.GetTaskModuleAdaptiveCard(); 
        let width = 350;
        let height = 160;
        if(token){

            const subCard = CardFactory.adaptiveCard({
                version: '1.0.0',
                type: 'AdaptiveCard',
                body: [
                    {
                        type: 'TextBlock',
                        text: `Your token is ` + token,
                        wrap: true,
                    }
                ]
            });

            const card = attachment.content;
            card.actions.push(
                {
                    type: 'Action.ShowCard',
                    title: 'Show Token',
                    card: subCard.content,
                }
            );
            width = 500;
            height = 300;
        }
        return { 
            card: attachment,
            height: height,
            width: width,
            title: 'Compose Extension Auth Example',
        };
    }

    private GetTaskModuleAdaptiveCard(): Attachment {
        return CardFactory.adaptiveCard({
            version: '1.0.0',
            type: 'AdaptiveCard',
            body: [
                {
                    type: 'TextBlock',
                    text: `You are signed in!`,
                },
                {
                    type: 'TextBlock',
                    text: `Send 'Log out' or 'Sign out' to start over.`,
                },
                {
                    type: 'TextBlock',
                    text: `(Or click the Sign Out button below.)`,
                },
            ],
            actions: [
                {
                    type: 'Action.Submit',
                    title: 'Close',
                    data: {
                        key: 'close',
                    }
                },
                {
                    type: 'Action.Submit',
                    title: 'Sign Out',
                    data: {
                        key: 'signout',
                    }
                }
            ]
        });
    }
}
