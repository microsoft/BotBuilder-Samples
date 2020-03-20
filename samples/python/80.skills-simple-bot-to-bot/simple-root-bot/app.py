# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import sys
import traceback
from datetime import datetime

from aiohttp import web
from aiohttp.web import Request, Response
from botbuilder.core import (
    BotFrameworkAdapterSettings,
    ConversationState,
    MemoryStorage,
    TurnContext,
    BotFrameworkAdapter,
)
from botbuilder.core.integration import (
    aiohttp_channel_service_routes,
    aiohttp_error_middleware,
)
from botbuilder.core.skills import SkillHandler
from botbuilder.schema import Activity, ActivityTypes
from botframework.connector.auth import (
    AuthenticationConfiguration,
    SimpleCredentialProvider,
)

from bots.root_bot import ACTIVE_SKILL_PROPERTY_NAME
from skill_http_client import SkillHttpClient
from skill_conversation_id_factory import SkillConversationIdFactory
from authentication import AllowedSkillsClaimsValidator
from bots import RootBot
from config import DefaultConfig, SkillConfiguration
from adapter_with_error_handler import AdapterWithErrorHandler

CONFIG = DefaultConfig()
SKILL_CONFIG = SkillConfiguration()

# Whitelist skills from SKILL_CONFIG
AUTH_CONFIG = AuthenticationConfiguration(
    claims_validator=AllowedSkillsClaimsValidator(CONFIG).validate_claims
)
# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = BotFrameworkAdapterSettings(
    app_id=CONFIG.APP_ID,
    app_password=CONFIG.APP_PASSWORD,
    auth_configuration=AUTH_CONFIG,
)

STORAGE = MemoryStorage()
CONVERSATION_STATE = ConversationState(STORAGE)

ID_FACTORY = SkillConversationIdFactory(STORAGE)
CREDENTIAL_PROVIDER = SimpleCredentialProvider(CONFIG.APP_ID, CONFIG.APP_PASSWORD)
CLIENT = SkillHttpClient(CREDENTIAL_PROVIDER, ID_FACTORY)

ADAPTER = AdapterWithErrorHandler(SETTINGS, CONFIG, CONVERSATION_STATE, CLIENT, SKILL_CONFIG)

# Create the Bot
BOT = RootBot(CONVERSATION_STATE, SKILL_CONFIG, CLIENT, CONFIG)

SKILL_HANDLER = SkillHandler(
    ADAPTER, BOT, ID_FACTORY, CREDENTIAL_PROVIDER, AuthenticationConfiguration()
)

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


APP = web.Application(middlewares=[aiohttp_error_middleware])
APP.router.add_post("/api/messages", messages)
APP.router.add_routes(aiohttp_channel_service_routes(SKILL_HANDLER, "/api/skills"))

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
