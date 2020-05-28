# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Implements bot Activity handler."""

from botbuilder.core import (
    ActivityHandler,
    ConversationState,
    UserState,
    TurnContext,
    BotTelemetryClient,
    NullTelemetryClient,
)
from botbuilder.dialogs import Dialog, DialogExtensions
from helpers.dialog_helper import DialogHelper


class DialogBot(ActivityHandler):
    """Main activity handler for the bot."""

    def __init__(
        self,
        conversation_state: ConversationState,
        user_state: UserState,
        dialog: Dialog,
        telemetry_client: BotTelemetryClient,
    ):
        if conversation_state is None:
            raise Exception(
                "[DialogBot]: Missing parameter. conversation_state is required"
            )
        if user_state is None:
            raise Exception("[DialogBot]: Missing parameter. user_state is required")
        if dialog is None:
            raise Exception("[DialogBot]: Missing parameter. dialog is required")

        self.conversation_state = conversation_state
        self.user_state = user_state
        self.dialog = dialog
        self.telemetry_client = telemetry_client

    async def on_message_activity(self, turn_context: TurnContext):
        await DialogExtensions.run_dialog(
            self.dialog,
            turn_context,
            self.conversation_state.create_property("DialogState"),
        )

        # Save any state changes that might have occured during the turn.
        await self.conversation_state.save_changes(turn_context, False)
        await self.user_state.save_changes(turn_context, False)

    @property
    def telemetry_client(self) -> BotTelemetryClient:
        """
        Gets the telemetry client for logging events.
        """
        return self._telemetry_client

    # pylint:disable=attribute-defined-outside-init
    @telemetry_client.setter
    def telemetry_client(self, value: BotTelemetryClient) -> None:
        """
        Sets the telemetry client for logging events.
        """
        if value is None:
            self._telemetry_client = NullTelemetryClient()
        else:
            self._telemetry_client = value
