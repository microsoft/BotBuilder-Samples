// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { Activity, ActivityTypes } from 'botbuilder';

export module ActivityEx {
    export function createReply(source: Activity, text?: string, locale?: string): Activity {
        const reply = text || '';
        return {
            type: ActivityTypes.Message,
            timestamp: new Date(),
            from: source.recipient,
            recipient: source.from,
            replyToId: source.id,
            serviceUrl: source.serviceUrl,
            channelId: source.channelId,
            conversation: source.conversation,
            text: reply,
            locale: locale,
            label: source.label,
            valueType: source.valueType
        };
    }
}