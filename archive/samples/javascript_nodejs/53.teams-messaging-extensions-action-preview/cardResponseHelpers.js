// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory, InputHints } = require('botbuilder');

class CardResponseHelpers {
    static toTaskModuleResponse(cardAttachment) {
        return {
            task: {
                type: 'continue',
                value: {
                    card: cardAttachment,
                    height: 450,
                    title: 'Task Module Fetch Example',
                    url: null,
                    width: 500
                }
            }
        };
    }

    static toMessagingExtensionBotMessagePreviewResponse(cardAttachment) {
        return {
            composeExtension: {
                activityPreview: MessageFactory.attachment(cardAttachment, null, null, InputHints.ExpectingInput),
                type: 'botMessagePreview'
            }
        };
    }
}

exports.CardResponseHelpers = CardResponseHelpers;
