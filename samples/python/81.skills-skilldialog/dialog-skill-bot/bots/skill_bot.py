# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import (
    Bot,
    ConversationState,
    TurnContext,
)
from botbuilder.dialogs import Dialog, DialogExtensions


class SkillBot(Bot):
    def __init__(self, conversation_state: ConversationState, main_dialog: Dialog):
        self._conversation_state = conversation_state
        self._main_dialog = main_dialog

    async def on_turn(self, context: TurnContext):
        await DialogExtensions.run_dialog(
            self._main_dialog,
            context,
            self._conversation_state.create_property("DialogState"),
        )

        # Save any state changes that might have occurred during the turn.
        await self._conversation_state.save_changes(context)
