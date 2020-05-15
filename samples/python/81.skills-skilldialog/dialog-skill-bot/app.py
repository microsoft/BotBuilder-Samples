# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
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
from botframework.connector.auth import AuthenticationConfiguration

from authentication import AllowedCallersClaimsValidator
from bots import SkillBot
from config import DefaultConfig
from dialogs import ActivityRouterDialog, DialogSkillBotRecognizer
from skill_adapter_with_error_handler import AdapterWithErrorHandler

CONFIG = DefaultConfig()

# Create MemoryStorage, UserState and ConversationState
MEMORY = MemoryStorage()
USER_STATE = UserState(MEMORY)
CONVERSATION_STATE = ConversationState(MEMORY)

# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
VALIDATOR = AllowedCallersClaimsValidator(CONFIG).claims_validator
SETTINGS = BotFrameworkAdapterSettings(
    CONFIG.APP_ID,
    CONFIG.APP_PASSWORD,
    auth_configuration=AuthenticationConfiguration(claims_validator=VALIDATOR),
)
ADAPTER = AdapterWithErrorHandler(SETTINGS, CONVERSATION_STATE)

# Create the Bot
RECOGNIZER = DialogSkillBotRecognizer(CONFIG)
ROUTER = ActivityRouterDialog(RECOGNIZER)
BOT = SkillBot(CONVERSATION_STATE, ROUTER)


# Listen for incoming requests on /api/messages
async def messages(req: Request) -> Response:
    # Main bot message handler.
    if "application/json" in req.headers["Content-Type"]:
        body = await req.json()
    else:
        return Response(status=HTTPStatus.UNSUPPORTED_MEDIA_TYPE)

    activity = Activity().deserialize(body)
    auth_header = req.headers["Authorization"] if "Authorization" in req.headers else ""

    invoke_response = await ADAPTER.process_activity(activity, auth_header, BOT.on_turn)
    if invoke_response:
        return json_response(data=invoke_response.body, status=invoke_response.status)
    return Response(status=HTTPStatus.OK)


APP = web.Application(middlewares=[aiohttp_error_middleware])
APP.router.add_post("/api/messages", messages)

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
