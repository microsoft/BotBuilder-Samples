# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List

from botbuilder.core import BotAdapter, TurnContext
from botbuilder.schema import Activity, ConversationReference


class DialogHostAdapter(BotAdapter):
    """
    This custom BotAdapter supports scenarios that only Send Activities. Update and Delete Activity
    are not supported.
    Rather than sending the outbound Activities directly as the BotFrameworkAdapter does this class
    buffers them in a list. The list is exposed as a public property.
    """

    def __init__(self):
        super(DialogHostAdapter, self).__init__()
        self.activities = []

    async def send_activities(self, context: TurnContext, activities: List[Activity]):
        self.activities.extend(activities)
        return []

    async def update_activity(self, context: TurnContext, activity: Activity):
        raise NotImplementedError

    async def delete_activity(
        self, context: TurnContext, reference: ConversationReference
    ):
        raise NotImplementedError
