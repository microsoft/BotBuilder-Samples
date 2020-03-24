# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json
import os

from botbuilder.adapters.slack import SlackRequestBody, SlackEvent
from botbuilder.core import ActivityHandler, MessageFactory, TurnContext
from botbuilder.schema import ChannelAccount, Attachment


class EchoBot(ActivityHandler):
    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity("Hello and welcome!")

    async def on_message_activity(self, turn_context: TurnContext):
        return await turn_context.send_activity(
            MessageFactory.text(f"Echo: {turn_context.activity.text}")
        )

    async def on_event_activity(self, turn_context: TurnContext):
        body = turn_context.activity.channel_data
        if not body:
            return

        if isinstance(body, SlackRequestBody) and body.command == "/test":
            interactive_message = MessageFactory.attachment(
                self.__create_interactive_message(
                    os.path.join(os.getcwd(), "./resources/InteractiveMessage.json")
                )
            )
            await turn_context.send_activity(interactive_message)

        if isinstance(body, SlackEvent):
            if body.subtype == "file_share":
                await turn_context.send_activity("Echo: I received and attachment")
            elif body.message and body.message.attachments:
                await turn_context.send_activity("Echo: I received a link share")

    def __create_interactive_message(self, file_path: str) -> Attachment:
        # interactive_message_json = open(file_path).read()
        # adaptive_card_attachment = json.loads(interactive_message_json, cls=Block)
        with open(file_path, "rb") as in_file:
            adaptive_card_attachment = json.load(in_file)

        return Attachment(
            content=adaptive_card_attachment,
            content_type="application/json",
            name="blocks",
        )
