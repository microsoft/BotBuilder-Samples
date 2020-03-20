# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import sys
import traceback
from datetime import datetime

from aiohttp import web
from aiohttp.web import Request, Response
from botbuilder.core import (
    BotFrameworkAdapterSettings,
    TurnContext,
    BotFrameworkAdapter,
)
from botbuilder.schema import Activity, ActivityTypes
from botframework.connector.auth import AuthenticationConfiguration

from bots import EchoBot
from config import DefaultConfig
from authentication import AllowedCallersClaimsValidator

CONFIG = DefaultConfig()
CLAIMS_VALIDATOR = AllowedCallersClaimsValidator(CONFIG)
AUTH_CONFIG = AuthenticationConfiguration(
    claims_validator=CLAIMS_VALIDATOR.claims_validator
)
# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = BotFrameworkAdapterSettings(
    app_id=CONFIG.APP_ID,
    app_password=CONFIG.APP_PASSWORD,
    auth_configuration=AUTH_CONFIG,
)
ADAPTER = BotFrameworkAdapter(SETTINGS)


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

    # Send a trace activity, which will be displayed in Bot Framework Emulator
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
        await context.send_activity(trace_activity)

    # Send and EndOfConversation activity to the skill caller with the error to end the conversation and let the
    # caller decide what to do. Send a trace activity if we're talking to the Bot Framework Emulator
    end_of_conversation = Activity(
        type=ActivityTypes.end_of_conversation, code="SkillError", text=f"{error}"
    )
    await context.send_activity(end_of_conversation)


ADAPTER.on_turn_error = on_error

# Create the Bot
BOT = EchoBot()


# Listen for incoming requests on /api/messages
async def messages(req: Request) -> Response:
    # Main bot message handler.
    if "application/json" in req.headers["Content-Type"]:
        body = await req.json()
    else:
        return Response(status=415)

    activity = Activity().deserialize(body)
    auth_header = req.headers["Authorization"] if "Authorization" in req.headers else ""

    try:
        await ADAPTER.process_activity(activity, auth_header, BOT.on_turn)
        return Response(status=201)
    except Exception as exception:
        raise exception


APP = web.Application()
APP.router.add_post("/api/messages", messages)

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
