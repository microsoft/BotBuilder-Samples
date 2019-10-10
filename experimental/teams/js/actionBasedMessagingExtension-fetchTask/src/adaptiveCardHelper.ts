// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Attachment,
    CardFactory,
    MessagingExtensionAction,
    MessagingExtensionActionResponse,
    TaskModuleContinueResponse,
    TaskModuleTaskInfo
} from 'botbuilder';

import { SubmitExampleData } from './submitExampleData';

export class AdaptiveCardHelper {
    public static toSubmitExampleData(action: MessagingExtensionAction): SubmitExampleData {
        const activityPreview = action.botActivityPreview[0];
        const attachmentContent = activityPreview.attachments[0].content;

        const userText = attachmentContent.body[1].text as string;
        const choiceSet = attachmentContent.body[3];

        return {
            MultiSelect: choiceSet.isMultiSelect ? 'true' : 'false',
            Option1: choiceSet.choices[0].title,
            Option2: choiceSet.choices[1].title,
            Option3: choiceSet.choices[2].title,
            Question: userText
        } as SubmitExampleData;
    }

    public static createTaskModuleAdaptiveCardResponse(
        userText: string = null,
        isMultiSelect: boolean = true,
        option1: string = null,
        option2: string = null,
        option3: string = null): MessagingExtensionActionResponse {

        const responseCard = CardFactory.adaptiveCard({
            actions: [
                {
                    data: {
                         submitLocation: 'messagingExtensionFetchTask'
                    },
                    title: 'Submit',
                    type: 'Action.Submit'
                }
            ],
            body: [
                {
                    text: 'This is an Adaptive Card within a Task Module',
                    type: 'TextBlock',
                    weight: 'bolder'
                },
                { type: 'TextBlock', text: 'Enter text for Question:' },
                {
                    id: 'Question',
                    placeholder: 'Question text here',
                    type: 'Input.Text',
                    value: userText
                },
                { type: 'TextBlock', text: 'Options for Question:' },
                { type: 'TextBlock', text: 'Is Multi-Select:' },
                {
                    choices: [{title: 'True', value: 'true'}, {title: 'False', value: 'false'}],
                    id: 'MultiSelect',
                    isMultiSelect: false,
                    style: 'expanded',
                    type: 'Input.ChoiceSet',
                    value: isMultiSelect ? 'true' : 'false'
                },
                {
                    id: 'Option1',
                    placeholder: 'Option 1 here',
                    type: 'Input.Text',
                    value: option1
                },
                {
                    id: 'Option2',
                    placeholder: 'Option 2 here',
                    type: 'Input.Text',
                    value: option2
                },
                {
                    id: 'Option3',
                    placeholder: 'Option 3 here',
                    type: 'Input.Text',
                    value: option3
                }
            ],
            type: 'AdaptiveCard',
            version : '1.0'
        });

        return {
            task: {
                type: 'continue',
                value: {
                    card: responseCard as Attachment,
                    height: 450,
                    title: 'Task Module Fetch Example',
                    url: null,
                    width: 500
                } as TaskModuleTaskInfo
            } as TaskModuleContinueResponse
        } as MessagingExtensionActionResponse;
    }

    public static toAdaptiveCardAttachment(data: SubmitExampleData): Attachment {
        return CardFactory.adaptiveCard({
            actions: [
                { type: 'Action.Submit', title: 'Submit', data: { submitLocation: 'messagingExtensionSubmit'} }
            ],
            body: [
                { text: 'Adaptive Card from Task Module', type: 'TextBlock', weight: 'bolder' },
                { text: `${ data.Question }`, type: 'TextBlock', id: 'Question' },
                { id: 'Answer', placeholder: 'Answer here...', type: 'Input.Text' },
                {
                    choices: [
                        {title: data.Option1, value: data.Option1},
                        {title: data.Option2, value: data.Option2},
                        {title: data.Option3, value: data.Option3}
                    ],
                    id: 'Choices',
                    isMultiSelect: Boolean(data.MultiSelect),
                    style: 'expanded',
                    type: 'Input.ChoiceSet'
                }
            ],
            type: 'AdaptiveCard',
            version: '1.0'
        });
    }
}
