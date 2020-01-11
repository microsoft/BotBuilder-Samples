# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

""" Bot initialization """
# pylint: disable=line-too-long
import sys
import asyncio
from django.apps import AppConfig
from botbuilder.core import (
    BotFrameworkAdapter,
    BotFrameworkAdapterSettings,
    TurnContext,
    ConversationState,
    MemoryStorage,
    UserState,
)
from dialogs import MainDialog
from bots import DialogAndWelcomeBot
import config


class BotConfig(AppConfig):
    """ Bot initialization """

    name = "bots"
    appConfig = config.DefaultConfig

    SETTINGS = BotFrameworkAdapterSettings(appConfig.APP_ID, appConfig.APP_PASSWORD)
    ADAPTER = BotFrameworkAdapter(SETTINGS)
    LOOP = asyncio.get_event_loop()

    # Create MemoryStorage, UserState and ConversationState
    memory = MemoryStorage()
    user_state = UserState(memory)
    conversation_state = ConversationState(memory)

    dialog = MainDialog(appConfig)
    bot = DialogAndWelcomeBot(conversation_state, user_state, dialog)

    async def on_error(self, context: TurnContext, error: Exception):
        """
        Catch-all for errors.
        This check writes out errors to console log
        NOTE: In production environment, you should consider logging this to Azure
        application insights.
        """
        print(f"\n [on_turn_error]: { error }", file=sys.stderr)
        # Send a message to the user
        await context.send_activity("Oops. Something went wrong!")
        # Clear out state
        await self.conversation_state.delete(context)

    def ready(self):
        self.ADAPTER.on_turn_error = self.on_error
