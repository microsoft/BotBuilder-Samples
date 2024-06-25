# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from http import HTTPStatus

from aiohttp import web
from aiohttp.web import Request, Response
from aiohttp.web_response import json_response
from botbuilder.integration.aiohttp import ConfigurationBotFrameworkAuthentication
from botbuilder.schema import Activity
from botframework.connector.auth import AuthenticationConfiguration

from bots import EchoBot
from config import DefaultConfig
from authentication import AllowedCallersClaimsValidator
from adapter_with_error_handler import AdapterWithErrorHandler

CONFIG = DefaultConfig()
CLAIMS_VALIDATOR = AllowedCallersClaimsValidator(CONFIG)
AUTH_CONFIG = AuthenticationConfiguration(
    claims_validator=CLAIMS_VALIDATOR.claims_validator
)
# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = ConfigurationBotFrameworkAuthentication(
    CONFIG,
    auth_configuration=AUTH_CONFIG,
)
ADAPTER = AdapterWithErrorHandler(SETTINGS)

# Create the Bot
BOT = EchoBot()


# Listen for incoming requests on /api/messages
async def messages(req: Request) -> Response:
    return await ADAPTER.process(req, BOT)



APP = web.Application()
APP.router.add_post("/api/messages", messages)

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
