# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import sys
import traceback
from datetime import datetime
import base64

from aiohttp import web
from aiohttp.web import Request, Response, json_response

from azure.keyvault.certificates import CertificateClient
from azure.keyvault.secrets import SecretClient
from cryptography.hazmat.primitives import hashes, serialization
from cryptography.hazmat.primitives.serialization import Encoding, NoEncryption, pkcs12, PrivateFormat
from azure.identity import DefaultAzureCredential

from botbuilder.core import (
    TurnContext,
)
from botbuilder.core.integration import aiohttp_error_middleware
from botbuilder.integration.aiohttp import CloudAdapter, ConfigurationBotFrameworkAuthentication
from botbuilder.schema import Activity, ActivityTypes
from botframework.connector.auth import CertificateServiceClientCredentialsFactory

from bots import EchoBot
from config import DefaultConfig

CONFIG = DefaultConfig()

# See: https://learn.microsoft.com/en-us/python/api/overview/azure/keyvault-certificates-readme?view=azure-python
KVUri = f"https://{CONFIG.APP_KEYVAULTNAME}.vault.azure.net"
credential = DefaultAzureCredential()

secret_client = SecretClient(vault_url=KVUri, credential=credential)
certificate_secret = secret_client.get_secret(CONFIG.APP_CERTIFICATENAME)

# This needs work.  Basically get a certificate from KeyVault and translate to PEM strings for the CertificateServiceClientCredentialsFactory
# See: https://github.com/Azure/azure-sdk-for-python/blob/07d10639d7e47f4852eaeb74aef5d569db499d6e/sdk/identity/azure-identity/azure/identity/_credentials/certificate.py#L101-L123
cert_bytes = base64.b64decode(certificate_secret.value)
private_key, public_certificate, additional_certificates = pkcs12.load_key_and_certificates(
    data=cert_bytes,
    password=None
)
key_bytes = private_key.private_bytes(Encoding.PEM, PrivateFormat.PKCS8, NoEncryption())
pem_sections = [key_bytes] + [c.public_bytes(Encoding.PEM) for c in [public_certificate] + additional_certificates]
pem_bytes = b"".join(pem_sections)
#fingerprint = public_certificate.fingerprint(hashes.SHA1())  # nosec

pem_str = pem_bytes.decode("utf-8")
public_certificate_str = public_certificate.public_bytes(Encoding.PEM).decode("utf-8")

# Create adapter with CertificateServiceClientCredentialsFactory
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
CREDENTIAL_FACTORY = CertificateServiceClientCredentialsFactory(
    certificate_thumbprint=CONFIG.APP_CERTIFICATETHUMBPRINT,
    certificate_private_key=pem_str,
    app_id=CONFIG.APP_ID,
    tenant_id=CONFIG.APP_TENANTID,
    certificate_public=public_certificate_str
)
ADAPTER = CloudAdapter(ConfigurationBotFrameworkAuthentication(CONFIG, credentials_factory=CREDENTIAL_FACTORY))

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

# Create the Bot
BOT = EchoBot()


# Listen for incoming requests on /api/messages
async def messages(req: Request) -> Response:
    return await ADAPTER.process(req, BOT)


APP = web.Application(middlewares=[aiohttp_error_middleware])
APP.router.add_post("/api/messages", messages)

if __name__ == "__main__":
    try:
        web.run_app(APP, host="localhost", port=CONFIG.PORT)
    except Exception as error:
        raise error
