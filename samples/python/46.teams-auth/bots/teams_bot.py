# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List
from botbuilder.core import (
    ConversationState,
    UserState,
    TurnContext,
)
from botbuilder.dialogs import Dialog
from botbuilder.schema import ChannelAccount

from helpers.dialog_helper import DialogHelper
from .dialog_bot import DialogBot


class TeamsBot(DialogBot):
    def __init__(
        self,
        conversation_state: ConversationState,
        user_state: UserState,
        dialog: Dialog,
    ):
        super(TeamsBot, self).__init__(conversation_state, user_state, dialog)

    async def on_members_added_activity(
        self, members_added: List[ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            # Greet anyone that was not the target (recipient) of this message.
            # To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    "Welcome to AuthenticationBot. Type anything to get logged in. Type "
                    "'logout' to sign-out."
                )

    async def on_teams_signin_verify_state(self, turn_context: TurnContext):
        # Run the Dialog with the new Token Response Event Activity.
        # The OAuth Prompt needs to see the Invoke Activity in order to complete the login process.
        await DialogHelper.run_dialog(
            self.dialog,
            turn_context,
            self.conversation_state.create_property("DialogState"),
        )
