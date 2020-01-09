# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import ActivityHandler, MessageFactory, TurnContext
from botbuilder.schema import Activity, ActivityTypes, EndOfConversationCodes


class EchoBot(ActivityHandler):
    async def on_message_activity(self, turn_context: TurnContext):
        if "end" in turn_context.activity.text or "stop" in turn_context.activity.text:
            # Send End of conversation at the end.
            await turn_context.send_activity(
                MessageFactory.text("Ending conversation from the skill...")
            )

            end_of_conversation = Activity(type=ActivityTypes.end_of_conversation)
            end_of_conversation.code = EndOfConversationCodes.completed_successfully
            await turn_context.send_activity(end_of_conversation)
        else:
            await turn_context.send_activity(
                MessageFactory.text(f"Echo (python): {turn_context.activity.text}")
            )
            await turn_context.send_activity(
                MessageFactory.text(
                    f'Say "end" or "stop" and I\'ll end the conversation and back to the parent.'
                )
            )
