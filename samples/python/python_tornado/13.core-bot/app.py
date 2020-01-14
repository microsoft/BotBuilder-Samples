# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""
This sample shows how to create a bot that demonstrates the following:
- Use [LUIS](https://www.luis.ai) to implement core AI capabilities.
- Implement a multi-turn conversation using Dialogs.
- Handle user interruptions for such things as `Help` or `Cancel`.
- Prompt for and validate requests for information from the user.
"""
import os

import tornado.ioloop
import tornado.web
import tornado.escape
from tornado.options import define, parse_command_line, options

from botbuilder.core import (
    BotFrameworkAdapterSettings,
    ConversationState,
    MemoryStorage,
    UserState,
)
from botbuilder.schema import Activity

from dialogs import MainDialog, BookingDialog
from bots import DialogAndWelcomeBot

from adapter_with_error_handler import AdapterWithErrorHandler
from flight_booking_recognizer import FlightBookingRecognizer

define("port", default=3978, help="Application port")
define("app_id", default=os.environ.get("MicrosoftAppId", ""), help="Application id from Azure")
define(
    "app_password",
    default=os.environ.get("MicrosoftAppPassword", ""),
    help="Application password from Azure"
)
define("luis_app_id", default=os.environ.get("LuisAppId", ""), help="Application id for Luis")
define("luis_api_key", default=os.environ.get("LuisApiKey", ""), help="Api key for Luis")
define(
    "luis_api_host_name",
    default=os.environ.get("LuisAppId", ""),
    help="LUIS endpoint host name, ie 'westus.api.cognitive.microsoft.com')"
)
define("debug", default=True, help="run in debug mode")

class MessageHandler(tornado.web.RequestHandler):
    def initialize(self, adapter: AdapterWithErrorHandler, bot: DialogAndWelcomeBot):
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


def main():
    parse_command_line()

    # Create adapter.
    # See https://aka.ms/about-bot-adapter to learn more about how bots work.
    settings = BotFrameworkAdapterSettings(options.app_id, options.app_password)

    # Create MemoryStorage, UserState and ConversationState
    memory = MemoryStorage()
    user_state = UserState(memory)
    conversation_state = ConversationState(memory)

    # Create adapter.
    # See https://aka.ms/about-bot-adapter to learn more about how bots work.
    adapter = AdapterWithErrorHandler(settings, conversation_state)

    # Create dialogs and Bot
    recognizer = FlightBookingRecognizer(options)
    booking_dialog = BookingDialog()
    dialog = MainDialog(recognizer, booking_dialog)
    bot = DialogAndWelcomeBot(conversation_state, user_state, dialog)


    app = tornado.web.Application(
        [
            (r"/api/messages", MessageHandler, dict(adapter=adapter, bot=bot)),
        ],
        debug=options.debug,
    )
    app.listen(options.port)
    tornado.ioloop.IOLoop.current().start()

if __name__ == "__main__":
    main()
