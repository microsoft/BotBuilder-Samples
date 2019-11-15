# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import MessageFactory, TurnContext
from botbuilder.schema import ChannelAccount
from .dialog_bot import DialogBot


class RichCardsBot(DialogBot):
    """
    RichCardsBot prompts a user to select a Rich Card and then returns the card
    that matches the user's selection.
    """

    def __init__(self, conversation_state, user_state, dialog):
        super().__init__(conversation_state, user_state, dialog)

    async def on_members_added_activity(
        self, members_added: ChannelAccount, turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                reply = MessageFactory.text(
                    "Welcome to CardBot. "
                    + "This bot will show you different types of Rich Cards. "
                    + "Please type anything to get started."
                )
                await turn_context.send_activity(reply)
