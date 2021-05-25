# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""
This sample shows how to create a bot that demonstrates the following:
- Use [LUIS](https://www.luis.ai) to implement core AI capabilities.
- Implement a multi-turn conversation using Dialogs.
- Handle user interruptions for such things as `Help` or `Cancel`.
- Prompt for and validate requests for information from the user.
"""
from http import HTTPStatus

from aiohttp import web
from aiohttp.web import Request, Response, json_response
from botbuilder.core import (
    BotFrameworkAdapterSettings,
    ConversationState,
    MemoryStorage,
    UserState,
)
from botbuilder.core.integration import aiohttp_error_middleware
from botbuilder.schema import Activity
from botbuilder.applicationinsights import ApplicationInsightsTelemetryClient
from botbuilder.integration.applicationinsights.aiohttp import (
    AiohttpTelemetryProcessor,
    bot_telemetry_middleware,
)

from config import DefaultConfig
from dialogs import MainDialog, BookingDialog
from bots import DialogAndWelcomeBot

from adapter_with_error_handler import AdapterWithErrorHandler
from flight_booking_recognizer import FlightBookingRecognizer

CONFIG = DefaultConfig()

# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = BotFrameworkAdapterSettings(CONFIG.APP_ID, CONFIG.APP_PASSWORD)

# Create MemoryStorage, UserState and ConversationState
MEMORY = MemoryStorage()
USER_STATE = UserState(MEMORY)
CONVERSATION_STATE = ConversationState(MEMORY)

# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
ADAPTER = AdapterWithErrorHandler(SETTINGS, CONVERSATION_STATE)

# Create telemetry client.
# Note the small 'client_queue_size'.  This is for demonstration purposes.  Larger queue sizes
# result in fewer calls to ApplicationInsights, improving bot performance at the expense of
# less frequent updates.
INSTRUMENTATION_KEY = CONFIG.APPINSIGHTS_INSTRUMENTATION_KEY
TELEMETRY_CLIENT = ApplicationInsightsTelemetryClient(
    INSTRUMENTATION_KEY, telemetry_processor=AiohttpTelemetryProcessor(), client_queue_size=10
)

# Create dialogs and Bot
RECOGNIZER = FlightBookingRecognizer(CONFIG)
BOOKING_DIALOG = BookingDialog()
DIALOG = MainDialog(RECOGNIZER, BOOKING_DIALOG, telemetry_client=TELEMETRY_CLIENT)
BOT = DialogAndWelcomeBot(CONVERSATION_STATE, USER_STATE, DIALOG, TELEMETRY_CLIENT)


# Listen for incoming requests on /api/messages.
async def messages(req: Request) -> Response:
    # Main bot message handler.
    if "application/json" in req.headers["Content-Type"]:
        body = await req.json()
    else:
        return Response(status=HTTPStatus.UNSUPPORTED_MEDIA_TYPE)

    activity = Activity().deserialize(body)
    auth_header = req.headers["Authorization"] if "Authorization" in req.headers else ""

    response = await ADAPTER.process_activity(activity, auth_header, BOT.on_turn)
    if response:
        return json_response(data=response.body, status=response.status)
    return Response(status=HTTPStatus.OK)


APP = web.Application(middlewares=[bot_telemetry_middleware, aiohttp_error_middleware])
APP.router.add_post("/api/messages", messages)

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
