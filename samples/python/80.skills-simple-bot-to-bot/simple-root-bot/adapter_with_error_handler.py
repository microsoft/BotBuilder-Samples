# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import sys
import traceback

from botbuilder.core import (
    BotFrameworkAdapter,
    BotFrameworkAdapterSettings,
    ConversationState,
    MessageFactory,
    TurnContext,
)
from botbuilder.integration.aiohttp.skills import SkillHttpClient
from botbuilder.schema import ActivityTypes, Activity, InputHints

from config import DefaultConfig, SkillConfiguration
from bots.root_bot import ACTIVE_SKILL_PROPERTY_NAME


class AdapterWithErrorHandler(BotFrameworkAdapter):
    def __init__(
        self,
        settings: BotFrameworkAdapterSettings,
        config: DefaultConfig,
        conversation_state: ConversationState,
        skill_client: SkillHttpClient = None,
        skill_config: SkillConfiguration = None,
    ):
        super().__init__(settings)
        self._config = config

        if not conversation_state:
            raise TypeError(
                "AdapterWithErrorHandler: `conversation_state` argument cannot be None."
            )
        self._conversation_state = conversation_state
        self._skill_client = skill_client
        self._skill_config = skill_config

        self.on_turn_error = self._handle_turn_error

    async def _handle_turn_error(self, turn_context: TurnContext, error: Exception):
        # This check writes out errors to console log
        # NOTE: In production environment, you should consider logging this to Azure
        #       application insights.
        print(f"\n [on_turn_error] unhandled error: {error}", file=sys.stderr)
        traceback.print_exc()
        await self._send_error_message(turn_context, error)
        await self._end_skill_conversation(turn_context, error)
        await self._clear_conversation_state(turn_context)

    async def _send_error_message(self, turn_context: TurnContext, error: Exception):
        if not self._skill_client or not self._skill_config:
            return
        try:
            # Send a message to the user.
            error_message_text = "The skill encountered an error or bug."
            error_message = MessageFactory.text(
                error_message_text, error_message_text, InputHints.ignoring_input
            )
            await turn_context.send_activity(error_message)

            error_message_text = (
                "To continue to run this bot, please fix the bot source code."
            )
            error_message = MessageFactory.text(
                error_message_text, error_message_text, InputHints.ignoring_input
            )
            await turn_context.send_activity(error_message)

            # Send a trace activity, which will be displayed in Bot Framework Emulator.
            await turn_context.send_trace_activity(
                label="TurnError",
                name="on_turn_error Trace",
                value=f"{error}",
                value_type="https://www.botframework.com/schemas/error",
            )
        except Exception as exception:
            print(
                f"\n Exception caught on _send_error_message : {exception}",
                file=sys.stderr,
            )
            traceback.print_exc()

    async def _end_skill_conversation(
        self, turn_context: TurnContext, error: Exception
    ):
        if (
            not self._skill_client
            or not self._skill_config
        ):
            return

        try:
            # Inform the active skill that the conversation is ended so that it has a chance to clean up.
            # Note: the root bot manages the ActiveSkillPropertyName, which has a value while the root bot
            # has an active conversation with a skill.
            active_skill = await self._conversation_state.create_property(
                ACTIVE_SKILL_PROPERTY_NAME
            ).get(turn_context)

            if active_skill:
                bot_id = self._config.APP_ID
                end_of_conversation = Activity(type=ActivityTypes.end_of_conversation)
                end_of_conversation.code = "RootSkillError"
                TurnContext.apply_conversation_reference(
                    end_of_conversation,
                    TurnContext.get_conversation_reference(turn_context.activity),
                    True,
                )

                await self._conversation_state.save_changes(turn_context, True)
                await self._skill_client.post_activity_to_skill(
                    bot_id,
                    active_skill,
                    self._skill_config.SKILL_HOST_ENDPOINT,
                    end_of_conversation,
                )
        except Exception as exception:
            print(
                f"\n Exception caught on _end_skill_conversation : {exception}",
                file=sys.stderr,
            )
            traceback.print_exc()

    async def _clear_conversation_state(self, turn_context: TurnContext):
        try:
            # Delete the conversationState for the current conversation to prevent the
            # bot from getting stuck in a error-loop caused by being in a bad state.
            # ConversationState should be thought of as similar to "cookie-state" for a Web page.
            await self._conversation_state.delete(turn_context)
        except Exception as exception:
            print(
                f"\n Exception caught on _clear_conversation_state : {exception}",
                file=sys.stderr,
            )
            traceback.print_exc()
