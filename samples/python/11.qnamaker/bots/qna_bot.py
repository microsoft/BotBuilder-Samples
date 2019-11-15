# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from flask import Config

from botbuilder.ai.qna import QnAMaker, QnAMakerEndpoint
from botbuilder.core import ActivityHandler, MessageFactory, TurnContext
from botbuilder.schema import ChannelAccount


class QnABot(ActivityHandler):
    def __init__(self, config: Config):
        self.qna_maker = QnAMaker(
            QnAMakerEndpoint(
                knowledge_base_id=config["QNA_KNOWLEDGEBASE_ID"],
                endpoint_key=config["QNA_ENDPOINT_KEY"],
                host=config["QNA_ENDPOINT_HOST"],
            )
        )

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    "Welcome to the QnA Maker sample! Ask me a question and I will try "
                    "to answer it."
                )

    async def on_message_activity(self, turn_context: TurnContext):
        # The actual call to the QnA Maker service.
        response = await self.qna_maker.get_answers(turn_context)
        if response and len(response) > 0:
            await turn_context.send_activity(MessageFactory.text(response[0].answer))
        else:
            await turn_context.send_activity("No QnA Maker answers were found.")
