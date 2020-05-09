# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import Awaitable, Callable, List

from botbuilder.core import Middleware, TurnContext
from botbuilder.schema import Activity, ResourceResponse, ActivityTypes


class LoggerMiddleware(Middleware):
    """
    Uses an ILogger instance to log user and bot messages. It filters out ContinueConversation
    events coming from skill responses.
    """

    def __init__(self, label: str = None):
        self._label = label or LoggerMiddleware.__name__

    async def on_turn(
        self, context: TurnContext, logic: Callable[[TurnContext], Awaitable]
    ):
        # Note: skill responses will show as ContinueConversation events; we don't log those.
        # We only log incoming messages from users.
        if (
            context.activity.type != ActivityTypes.event
            and context.activity.name != "ContinueConversation"
        ):
            message = f"{self._label} {context.activity.type} {context.activity.text}"
            print(message)

        # Register outgoing handler.
        context.on_send_activities(self._outgoing_handler)

        # Continue processing messages.
        await logic()

    async def _outgoing_handler(
        self,
        context: TurnContext,  # pylint: disable=unused-argument
        activities: List[Activity],
        logic: Callable[[TurnContext], Awaitable[List[ResourceResponse]]],
    ):
        for activity in activities:
            message = f"{self._label} {activity.type} {activity.text}"
            print(message)

        return await logic()
