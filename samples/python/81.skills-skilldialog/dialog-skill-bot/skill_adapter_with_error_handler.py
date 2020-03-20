# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import sys
import traceback
from datetime import datetime

from botbuilder.core import (
    BotFrameworkAdapter,
    BotFrameworkAdapterSettings,
    ConversationState,
    MessageFactory,
    TurnContext,
)
from botbuilder.schema import ActivityTypes, Activity, InputHints


class SkillAdapterWithErrorHandler(BotFrameworkAdapter):
    def __init__(
        self,
        settings: BotFrameworkAdapterSettings,
        conversation_state: ConversationState = None,
    ):
        super().__init__(settings)
        self._conversation_state = conversation_state

        # Catch-all for errors.
        async def on_error(context: TurnContext, error: Exception):
            # This check writes out errors to console log
            # NOTE: In production environment, you should consider logging this to Azure
            #       application insights.
            print(f"\n [on_turn_error] unhandled error: {error}", file=sys.stderr)
            traceback.print_exc()

            # Send a message to the user
            error_message_text = "The skill encountered an error or bug."
            error_message = MessageFactory.text(
                error_message_text, error_message_text, InputHints.ignoring_input
            )
            await context.send_activity(error_message)

            error_message_text = (
                "To continue to run this bot, please fix the bot source code."
            )
            error_message = MessageFactory.text(
                error_message_text, error_message_text, InputHints.ignoring_input
            )
            await context.send_activity(error_message)

            # Send a trace activity if we're talking to the Bot Framework Emulator
            if context.activity.channel_id == "emulator":
                # Create a trace activity that contains the error object
                trace_activity = Activity(
                    label="TurnError",
                    name="on_turn_error Trace",
                    timestamp=datetime.utcnow(),
                    type=ActivityTypes.trace,
                    value=f"{error}",
                    value_type="https://www.botframework.com/schemas/error",
                )
                # Send a trace activity, which will be displayed in Bot Framework Emulator
                await context.send_activity(trace_activity)

            # Clear out state
            nonlocal self
            if self._conversation_state:
                try:
                    await self._conversation_state.delete(context)
                except Exception as exception:
                    print(
                        f"\n Exception caught on attempting to Delete ConversationState : {exception}",
                        file=sys.stderr,
                    )
                    traceback.print_exc()

            # Send and EndOfConversation activity to the skill caller with the error to end the conversation
            # and let the caller decide what to do.
            end_of_conversation = Activity(type=ActivityTypes.end_of_conversation)
            end_of_conversation.code = "SkillError"
            end_of_conversation.text = str(error)
            await context.send_activity(end_of_conversation)

            # Send a trace activity, which will be displayed in the Bot Framework Emulator
            # Note: we return the entire exception in the value property to help the developer,
            # this should not be done in prod.
            await context.send_trace_activity(
                "OnTurnError Trace",
                str(error),
                "https://www.botframework.com/schemas/error",
                "TurnError",
            )

        self.on_turn_error = on_error
