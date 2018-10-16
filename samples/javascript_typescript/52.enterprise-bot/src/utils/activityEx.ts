// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { Activity, ActivityTypes } from "botbuilder";

export namespace ActivityEx {
    export function createReply(source: Activity, text?: string, local?: string): Activity {
        const reply = text || "";
        return {
            channelId: source.channelId,
            conversation: source.conversation,
            from: source.recipient,
            label: source.label,
            locale: local,
            recipient: source.from,
            replyToId: source.id,
            serviceUrl: source.serviceUrl,
            text: reply,
            timestamp: new Date(),
            type: ActivityTypes.Message,
            valueType: source.valueType,
        };
    }
}
