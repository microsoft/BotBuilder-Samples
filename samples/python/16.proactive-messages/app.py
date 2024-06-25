# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import sys
import traceback
import uuid
from datetime import datetime
from http import HTTPStatus
from typing import Dict

from aiohttp import web
from aiohttp.web import Request, Response, json_response
from botbuilder.core import (
    TurnContext,
)
from botbuilder.core.integration import aiohttp_error_middleware
from botbuilder.integration.aiohttp import CloudAdapter, ConfigurationBotFrameworkAuthentication
from botbuilder.schema import Activity, ActivityTypes, ConversationReference

from bots import ProactiveBot
from config import DefaultConfig

CONFIG = DefaultConfig()

# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
ADAPTER = CloudAdapter(ConfigurationBotFrameworkAuthentication(CONFIG))

# Catch-all for errors.
async def on_error(context: TurnContext, error: Exception):
    # This check writes out errors to console log .vs. app insights.
    # NOTE: In production environment, you should consider logging this to Azure
    #       application insights.
    print(f"\n [on_turn_error] unhandled error: {error}", file=sys.stderr)
    traceback.print_exc()

    # Send a message to the user
    await context.send_activity("The bot encountered an error or bug.")
    await context.send_activity(
        "To continue to run this bot, please fix the bot source code."
    )
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


ADAPTER.on_turn_error = on_error

# Create a shared dictionary.  The Bot will add conversation references when users
# join the conversation and send messages.
CONVERSATION_REFERENCES: Dict[str, ConversationReference] = dict()

# If the channel is the Emulator, and authentication is not in use, the AppId will be null.
# We generate a random AppId for this case only. This is not required for production, since
# the AppId will have a value.
APP_ID = CONFIG.APP_ID if CONFIG.APP_ID else uuid.uuid4()

# Create the Bot
BOT = ProactiveBot(CONVERSATION_REFERENCES)


# Listen for incoming requests on /api/messages.
async def messages(req: Request) -> Response:
    return await ADAPTER.process(req, BOT)


# Listen for requests on /api/notify, and send a messages to all conversation members.
async def notify(req: Request) -> Response:  # pylint: disable=unused-argument
    await _send_proactive_message()
    return Response(status=HTTPStatus.OK, text="Proactive messages have been sent")


# Send a message to all conversation members.
# This uses the shared Dictionary that the Bot adds conversation references to.
async def _send_proactive_message():
    for conversation_reference in CONVERSATION_REFERENCES.values():
        await ADAPTER.continue_conversation(
            conversation_reference,
            lambda turn_context: turn_context.send_activity("proactive hello"),
            APP_ID,
        )


APP = web.Application(middlewares=[aiohttp_error_middleware])
APP.router.add_post("/api/messages", messages)
APP.router.add_get("/api/notify", notify)

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
