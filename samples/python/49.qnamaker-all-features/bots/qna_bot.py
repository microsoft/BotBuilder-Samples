# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
from typing import List

from botbuilder.core import (
    ActivityHandler,
    TurnContext,
    ConversationState,
    UserState,
)
from botbuilder.dialogs import Dialog
from botbuilder.schema import ChannelAccount

from helpers.dialog_helper import DialogHelper


class QnABot(ActivityHandler):
    def __init__(
        self,
        conversation_state: ConversationState,
        user_state: UserState,
        dialog: Dialog,
    ):
        self.conversation_state = conversation_state
        self.user_state = user_state
        self.dialog = dialog

    async def on_turn(self, turn_context: TurnContext):
        await super().on_turn(turn_context)
        await self.conversation_state.save_changes(turn_context)
        await self.user_state.save_changes(turn_context)

    async def on_members_added_activity(
        self, members_added: List[ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity("Hello and welcome!")

    async def on_message_activity(self, turn_context: TurnContext):
        await DialogHelper.run_dialog(
            self.dialog,
            turn_context,
            self.conversation_state.create_property("DialogState"),
        )
