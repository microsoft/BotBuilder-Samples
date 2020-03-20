from typing import Awaitable, Callable, List

from botbuilder.core import Middleware, TurnContext
from botbuilder.schema import Activity, ResourceResponse


class LoggerMiddleware(Middleware):
    def __init__(self, label: str):
        self._label = label or LoggerMiddleware.__name__

    async def on_turn(
        self, context: TurnContext, logic: Callable[[TurnContext], Awaitable]
    ):
        message = f"{self._label} {context.activity.type} {context.activity.text}"
        print(message)

        # Register outgoing handler
        context.on_send_activities(self._outgoing_handler)

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
