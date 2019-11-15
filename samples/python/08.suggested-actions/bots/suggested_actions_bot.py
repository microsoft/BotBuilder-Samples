# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import ActivityHandler, MessageFactory, TurnContext
from botbuilder.schema import ChannelAccount, CardAction, ActionTypes, SuggestedActions


class SuggestActionsBot(ActivityHandler):
    """
    This bot will respond to the user's input with suggested actions.
    Suggested actions enable your bot to present buttons that the user
    can tap to provide input.
    """

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        """
        Send a welcome message to the user and tell them what actions they may perform to use this bot
        """

        return await self._send_welcome_message(turn_context)

    async def on_message_activity(self, turn_context: TurnContext):
        """
        Respond to the users choice and display the suggested actions again.
        """

        text = turn_context.activity.text.lower()
        response_text = self._process_input(text)

        await turn_context.send_activity(MessageFactory.text(response_text))

        return await self._send_suggested_actions(turn_context)

    async def _send_welcome_message(self, turn_context: TurnContext):
        for member in turn_context.activity.members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    MessageFactory.text(
                        f"Welcome to SuggestedActionsBot {member.name}."
                        f" This bot will introduce you to suggestedActions."
                        f" Please answer the question: "
                    )
                )

                await self._send_suggested_actions(turn_context)

    def _process_input(self, text: str):
        color_text = "is the best color, I agree."

        if text == "red":
            return f"Red {color_text}"

        if text == "yellow":
            return f"Yellow {color_text}"

        if text == "blue":
            return f"Blue {color_text}"

        return "Please select a color from the suggested action choices"

    async def _send_suggested_actions(self, turn_context: TurnContext):
        """
        Creates and sends an activity with suggested actions to the user. When the user
        clicks one of the buttons the text value from the "CardAction" will be displayed
        in the channel just as if the user entered the text. There are multiple
        "ActionTypes" that may be used for different situations.
        """

        reply = MessageFactory.text("What is your favorite color?")

        reply.suggested_actions = SuggestedActions(
            actions=[
                CardAction(title="Red", type=ActionTypes.im_back, value="Red"),
                CardAction(title="Yellow", type=ActionTypes.im_back, value="Yellow"),
                CardAction(title="Blue", type=ActionTypes.im_back, value="Blue"),
            ]
        )

        return await turn_context.send_activity(reply)
