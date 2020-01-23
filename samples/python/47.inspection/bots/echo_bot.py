# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import (
    ActivityHandler,
    ConversationState,
    TurnContext,
    UserState,
    MessageFactory,
)
from botbuilder.schema import ChannelAccount

from data_models import CustomState


class EchoBot(ActivityHandler):
    def __init__(self, conversation_state: ConversationState, user_state: UserState):
        if conversation_state is None:
            raise TypeError(
                "[EchoBot]: Missing parameter. conversation_state is required but None was given"
            )
        if user_state is None:
            raise TypeError(
                "[EchoBot]: Missing parameter. user_state is required but None was given"
            )

        self.conversation_state = conversation_state
        self.user_state = user_state

        self.conversation_state_accessor = self.conversation_state.create_property(
            "CustomState"
        )
        self.user_state_accessor = self.user_state.create_property("CustomState")

    async def on_turn(self, turn_context: TurnContext):
        await super().on_turn(turn_context)

        await self.conversation_state.save_changes(turn_context)
        await self.user_state.save_changes(turn_context)

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity("Hello and welcome!")

    async def on_message_activity(self, turn_context: TurnContext):
        # Get the state properties from the turn context.
        user_data = await self.user_state_accessor.get(turn_context, CustomState)
        conversation_data = await self.conversation_state_accessor.get(
            turn_context, CustomState
        )

        await turn_context.send_activity(
            MessageFactory.text(
                f"Echo: {turn_context.activity.text}, "
                f"conversation state: {conversation_data.value}, "
                f"user state: {user_data.value}"
            )
        )

        user_data.value = user_data.value + 1
        conversation_data.value = conversation_data.value + 1
