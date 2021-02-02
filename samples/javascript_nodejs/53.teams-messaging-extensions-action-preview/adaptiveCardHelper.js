// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');

class AdaptiveCardHelper {
    static toSubmitExampleData(action) {
        const activityPreview = action.botActivityPreview[0];
        const attachmentContent = activityPreview.attachments[0].content;
        const userText = attachmentContent.body[1].text;
        const choiceSet = attachmentContent.body[3];
        const attributionFlag = attachmentContent.body[4].text.split(':')[1];
        return {
            MultiSelect: choiceSet.isMultiSelect ? 'true' : 'false',
            UserAttributionSelect: attributionFlag,
            Option1: choiceSet.choices[0].title,
            Option2: choiceSet.choices[1].title,
            Option3: choiceSet.choices[2].title,
            Question: userText
        };
    }

    static createAdaptiveCardEditor(userText = null, isMultiSelect = true, option1 = null, option2 = null, option3 = null) {
        return CardFactory.adaptiveCard({
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
                    choices: [{ title: 'True', value: 'true' }, { title: 'False', value: 'false' }],
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
                },
                { type: 'TextBlock', text: 'Do you want to send this card on behalf of the User?' },
                {
                    choices: [{ title: 'Yes', value: 'true' }, { title: 'No', value: 'false' }],
                    id: 'UserAttributionSelect',
                    isMultiSelect: false,
                    style: 'expanded',
                    type: 'Input.ChoiceSet',
                    value: isMultiSelect ? 'true' : 'false'
                }
            ],
            type: 'AdaptiveCard',
            version: '1.0'
        });
    }

    static createAdaptiveCardAttachment(data) {
        return CardFactory.adaptiveCard({
            actions: [
                { type: 'Action.Submit', title: 'Submit', data: { submitLocation: 'messagingExtensionSubmit' } }
            ],
            body: [
                { text: 'Adaptive Card from Task Module', type: 'TextBlock', weight: 'bolder' },
                { text: `${ data.Question }`, type: 'TextBlock', id: 'Question' },
                { id: 'Answer', placeholder: 'Answer here...', type: 'Input.Text' },
                {
                    choices: [
                        { title: data.Option1, value: data.Option1 },
                        { title: data.Option2, value: data.Option2 },
                        { title: data.Option3, value: data.Option3 }
                    ],
                    id: 'Choices',
                    isMultiSelect: data.MultiSelect,
                    style: 'expanded',
                    type: 'Input.ChoiceSet'
                },
                { text: 'Sending card on behalf of user is set to:'+`${ data.UserAttributionSelect }`, type: 'TextBlock', id: 'AttributionChoice' }
            ],
            type: 'AdaptiveCard',
            version: '1.0'
        });
    }
}
exports.AdaptiveCardHelper = AdaptiveCardHelper;
