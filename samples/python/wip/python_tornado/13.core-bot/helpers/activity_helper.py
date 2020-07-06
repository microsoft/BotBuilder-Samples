# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from datetime import datetime
from botbuilder.schema import (
    Activity,
    ActivityTypes,
    ChannelAccount,
    ConversationAccount,
)


def create_activity_reply(activity: Activity, text: str = None, locale: str = None):

    return Activity(
        type=ActivityTypes.message,
        timestamp=datetime.utcnow(),
        from_property=ChannelAccount(
            id=getattr(activity.recipient, "id", None),
            name=getattr(activity.recipient, "name", None),
        ),
        recipient=ChannelAccount(
            id=activity.from_property.id, name=activity.from_property.name
        ),
        reply_to_id=activity.id,
        service_url=activity.service_url,
        channel_id=activity.channel_id,
        conversation=ConversationAccount(
            is_group=activity.conversation.is_group,
            id=activity.conversation.id,
            name=activity.conversation.name,
        ),
        text=text or "",
        locale=locale or "",
        attachments=[],
        entities=[],
    )
