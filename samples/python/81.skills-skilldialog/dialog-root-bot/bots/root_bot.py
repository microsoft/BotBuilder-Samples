import json
import os.path

from typing import List

from botbuilder.core import (
    ActivityHandler,
    ConversationState,
    MessageFactory,
    TurnContext,
)
from botbuilder.dialogs import Dialog, DialogExtensions
from botbuilder.schema import ActivityTypes, Attachment, ChannelAccount


class RootBot(ActivityHandler):
    def __init__(
        self, conversation_state: ConversationState, main_dialog: Dialog,
    ):
        self._conversation_state = conversation_state
        self._main_dialog = main_dialog

    async def on_turn(self, turn_context: TurnContext):
        if turn_context.activity.type != ActivityTypes.conversation_update:
            # Handle end of conversation back from the skill
            # forget skill invocation
            await DialogExtensions.run_dialog(
                self._main_dialog,
                turn_context,
                self._conversation_state.create_property("DialogState"),
            )
        else:
            # Let the base class handle the activity.
            await super().on_turn(turn_context)

        # Save any state changes that might have occurred during the turn.
        await self._conversation_state.save_changes(turn_context)

    async def on_members_added_activity(
        self, members_added: List[ChannelAccount], turn_context: TurnContext
    ):
        # Greet anyone that was not the target (recipient) of this message.
        # To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards.
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                welcome_card = self._create_adaptive_card_attachment()
                activity = MessageFactory.attachment(welcome_card)
                activity.speak = "Welcome to the Dialog Skill Prototype!"
                await turn_context.send_activity(activity)
                await DialogExtensions.run_dialog(
                    self._main_dialog,
                    turn_context,
                    self._conversation_state.create_property("DialogState"),
                )

    def _create_adaptive_card_attachment(self) -> Attachment:
        relative_path = os.path.abspath(os.path.dirname(__file__))
        path = os.path.join(relative_path, "../cards/welcomeCard.json")
        with open(path) as in_file:
            card = json.load(in_file)

        return Attachment(
            content_type="application/vnd.microsoft.card.adaptive", content=card
        )
