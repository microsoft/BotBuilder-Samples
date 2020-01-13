# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os
import sys
from datetime import datetime

import tornado.ioloop
import tornado.web
import tornado.escape
from tornado.options import define, parse_command_line, options
from botbuilder.core import BotFrameworkAdapterSettings, TurnContext, BotFrameworkAdapter
from botbuilder.schema import Activity, ActivityTypes


from bots import EchoBot

define("port", default=3978, help="Application port")
define("app_id", default=os.environ.get("MicrosoftAppId", ""), help="Application id from Azure")
define(
    "app_password", default=os.environ.get("MicrosoftAppPassword", ""),
    help="Application password from Azure"
)
define("debug", default=True, help="run in debug mode")

class MessageHandler(tornado.web.RequestHandler):
    def initialize(self, adapter: BotFrameworkAdapter, bot: EchoBot):
        self.adapter = adapter
        self.bot = bot

    async def post(self):
        # Main bot message handler.
        if "application/json" in self.request.headers["Content-Type"]:
            body = tornado.escape.json_decode(self.request.body)
        else:
            self.set_status(415)
            return

        activity = Activity().from_dict(body)
        auth_header = (
            self.request.headers["Authorization"] if "Authorization" in self.request.headers else ""
        )

        try:
            await self.adapter.process_activity(activity, auth_header, self.bot.on_turn)
            self.set_status(201)
            return
        except Exception as exception:
            raise exception

parse_command_line()

# Create adapter.
# See https://aka.ms/about-bot-adapter to learn more about how bots work.
SETTINGS = BotFrameworkAdapterSettings(options.app_id, options.app_password)
ADAPTER = BotFrameworkAdapter(SETTINGS)

# Catch-all for errors.
async def on_error(context: TurnContext, error: Exception):
    # This check writes out errors to console log .vs. app insights.
    # NOTE: In production environment, you should consider logging this to Azure
    #       application insights.
    print(f"\n [on_turn_error] unhandled error: {error}", file=sys.stderr)

    # Send a message to the user
    await context.send_activity("The bot encountered an error or bug.")
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

ADAPTER.on_turn_error = on_error
BOT = EchoBot()

APP = tornado.web.Application(
    [
        (r"/api/messages", MessageHandler, dict(adapter=ADAPTER, bot=BOT)),
    ],
    debug=options.debug,
)
APP.listen(options.port)
tornado.ioloop.IOLoop.current().start()
