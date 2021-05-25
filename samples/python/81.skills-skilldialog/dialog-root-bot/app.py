# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
from http import HTTPStatus

from aiohttp import web
from aiohttp.web import Request, Response
from aiohttp.web_response import json_response
from botbuilder.core import (
    BotFrameworkAdapterSettings,
    ConversationState,
    MemoryStorage,
    UserState,
)
from botbuilder.core.integration import (
    aiohttp_channel_service_routes,
    aiohttp_error_middleware,
)
from botbuilder.core.skills import SkillHandler
from botbuilder.schema import Activity
from botbuilder.integration.aiohttp.skills import SkillHttpClient
from botframework.connector.auth import (
    AuthenticationConfiguration,
    SimpleCredentialProvider,
)

from authentication import AllowedSkillsClaimsValidator
from bots import RootBot
from dialogs import MainDialog
from config import DefaultConfig, SkillConfiguration
from adapter_with_error_handler import AdapterWithErrorHandler
from skill_conversation_id_factory import SkillConversationIdFactory

CONFIG = DefaultConfig()
SKILL_CONFIG = SkillConfiguration()

# Create MemoryStorage, UserState and ConversationState
MEMORY = MemoryStorage()
USER_STATE = UserState(MEMORY)
CONVERSATION_STATE = ConversationState(MEMORY)
ID_FACTORY = SkillConversationIdFactory(MEMORY)

CREDENTIAL_PROVIDER = SimpleCredentialProvider(CONFIG.APP_ID, CONFIG.APP_PASSWORD)
CLIENT = SkillHttpClient(CREDENTIAL_PROVIDER, ID_FACTORY)


# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = BotFrameworkAdapterSettings(CONFIG.APP_ID, CONFIG.APP_PASSWORD)
ADAPTER = AdapterWithErrorHandler(
    SETTINGS, CONFIG, CONVERSATION_STATE, CLIENT, SKILL_CONFIG
)

DIALOG = MainDialog(CONVERSATION_STATE, ID_FACTORY, CLIENT, SKILL_CONFIG, CONFIG)

# Create the Bot
BOT = RootBot(CONVERSATION_STATE, DIALOG)  # , SKILL_CONFIG, ID_FACTORY, CLIENT, CONFIG)

AUTH_CONFIG = AuthenticationConfiguration(
    claims_validator=AllowedSkillsClaimsValidator(CONFIG).claims_validator
)

SKILL_HANDLER = SkillHandler(ADAPTER, BOT, ID_FACTORY, CREDENTIAL_PROVIDER, AUTH_CONFIG)


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
APP.router.add_get("/api/messages", messages)
APP.router.add_routes(aiohttp_channel_service_routes(SKILL_HANDLER, "/api/skills"))

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
