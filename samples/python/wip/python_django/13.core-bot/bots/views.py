#!/usr/bin/env python
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""
This sample shows how to create a bot that demonstrates the following:
- Use [LUIS](https://www.luis.ai) to implement core AI capabilities.
- Implement a multi-turn conversation using Dialogs.
- Handle user interruptions for such things as `Help` or `Cancel`.
- Prompt for and validate requests for information from the user.
"""

import asyncio
import json
from django.http import HttpResponse
from django.apps import apps
from botbuilder.schema import Activity

# pylint: disable=line-too-long
def home():
    """Default handler."""
    return HttpResponse("Hello!")


def messages(request):
    """Main bot message handler."""
    if "application/json" in request.headers["Content-Type"]:
        body = json.loads(request.body.decode("utf-8"))
    else:
        return HttpResponse(status=415)

    activity = Activity().deserialize(body)
    auth_header = (
        request.headers["Authorization"] if "Authorization" in request.headers else ""
    )

    bot_app = apps.get_app_config("bots")
    bot = bot_app.bot
    loop = bot_app.LOOP
    adapter = bot_app.ADAPTER

    async def aux_func(turn_context):
        await bot.on_turn(turn_context)

    try:
        task = asyncio.ensure_future(
            adapter.process_activity(activity, auth_header, aux_func), loop=loop
        )
        loop.run_until_complete(task)
        return HttpResponse(status=201)
    except Exception as exception:
        raise exception
    return HttpResponse("This is message processing!")
