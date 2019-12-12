# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
# pylint: disable=import-error

"""
This sample shows how to create a bot that demonstrates the following:
- Use [LUIS](https://www.luis.ai) to implement core AI capabilities.
- Implement a multi-turn conversation using Dialogs.
- Handle user interruptions for such things as `Help` or `Cancel`.
- Prompt for and validate requests for information from the user.
"""

import asyncio
import sys
from datetime import datetime
from types import MethodType

from flask import Flask, request, Response
from botbuilder.core import (
    BotFrameworkAdapter,
    BotFrameworkAdapterSettings,
    ConversationState,
    MemoryStorage,
    UserState,
    TurnContext
)
from botbuilder.schema import Activity, ActivityTypes

from bots import DialogAndWelcomeBot
from dialogs import MainDialog, BookingDialog
from flight_booking_recognizer import FlightBookingRecognizer

LOOP = asyncio.get_event_loop()
APP = Flask(__name__, instance_relative_config=True)
APP.config.from_object("config.DefaultConfig")

SETTINGS = BotFrameworkAdapterSettings(APP.config["APP_ID"], APP.config["APP_PASSWORD"])
ADAPTER = BotFrameworkAdapter(SETTINGS)

# Create MemoryStorage, UserState and ConversationState
MEMORY = MemoryStorage()
USER_STATE = UserState(MEMORY)
CONVERSATION_STATE = ConversationState(MEMORY)
RECOGNIZER = FlightBookingRecognizer(APP.config)
BOOKING_DIALOG = BookingDialog()

# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = BotFrameworkAdapterSettings(APP.config["APP_ID"], APP.config["APP_PASSWORD"])
ADAPTER = BotFrameworkAdapter(SETTINGS)

# Catch-all for errors.
# pylint: disable=unused-argument
async def on_error(self, context: TurnContext, error: Exception):
    # This check writes out errors to console log .vs. app insights.
    # NOTE: In production environment, you should consider logging this to Azure
    #       application insights.
    print(f"\n [on_turn_error] unhandled error: {error}", file=sys.stderr)

    # Send a message to the user
    await context.send_activity("The bot encounted an error or bug.")
    await context.send_activity("To continue to run this bot, please fix the bot source code.")
    # Send a trace activity if we're talking to the Bot Framework Emulator
    if context.activity.channel_id == 'emulator':
        # Create a trace activity that contains the error object
        trace_activity = Activity(
            label="TurnError",
            name="on_turn_error Trace",
            timestamp=datetime.utcnow(),
            type=ActivityTypes.trace,
            value=f"{error}",
            value_type="https://www.botframework.com/schemas/error"
        )
        # Send a trace activity, which will be displayed in Bot Framework Emulator
        await context.send_activity(trace_activity)

ADAPTER.on_turn_error = MethodType(on_error, ADAPTER)

DIALOG = MainDialog(RECOGNIZER, BOOKING_DIALOG)
BOT = DialogAndWelcomeBot(CONVERSATION_STATE, USER_STATE, DIALOG)


@APP.route("/api/messages", methods=["POST"])
def messages():
    """Main bot message handler."""
    if "application/json" in request.headers["Content-Type"]:
        body = request.json
    else:
        return Response(status=415)

    activity = Activity().deserialize(body)
    auth_header = (
        request.headers["Authorization"] if "Authorization" in request.headers else ""
    )

    try:
        task = LOOP.create_task(
            ADAPTER.process_activity(activity, auth_header, BOT.on_turn)
        )
        LOOP.run_until_complete(task)
        return Response(status=201)
    except Exception as exception:
        raise exception


if __name__ == "__main__":
    try:
        APP.run(debug=False, port=APP.config["PORT"])  # nosec debug
    except Exception as exception:
        raise exception
