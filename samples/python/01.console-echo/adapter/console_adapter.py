# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import datetime
import asyncio
import warnings
from typing import List, Callable

from botbuilder.schema import (
    Activity,
    ActivityTypes,
    ChannelAccount,
    ConversationAccount,
    ResourceResponse,
    ConversationReference,
)
from botbuilder.core.turn_context import TurnContext
from botbuilder.core.bot_adapter import BotAdapter


class ConsoleAdapter(BotAdapter):
    """
    Lets a user communicate with a bot from a console window.

    :Example:
    import asyncio
    from botbuilder.core import ConsoleAdapter

    async def logic(context):
        await context.send_activity('Hello World!')

    adapter = ConsoleAdapter()
    loop = asyncio.get_event_loop()
    if __name__ == "__main__":
        try:
            loop.run_until_complete(adapter.process_activity(logic))
        except KeyboardInterrupt:
            pass
        finally:
            loop.stop()
            loop.close()
    """

    def __init__(self, reference: ConversationReference = None):
        super(ConsoleAdapter, self).__init__()

        self.reference = ConversationReference(
            channel_id="console",
            user=ChannelAccount(id="user", name="User1"),
            bot=ChannelAccount(id="bot", name="Bot"),
            conversation=ConversationAccount(id="convo1", name="", is_group=False),
            service_url="",
        )

        # Warn users to pass in an instance of a ConversationReference, otherwise the parameter will be ignored.
        if reference is not None and not isinstance(reference, ConversationReference):
            warnings.warn(
                "ConsoleAdapter: `reference` argument is not an instance of ConversationReference and will "
                "be ignored."
            )
        else:
            self.reference.channel_id = getattr(
                reference, "channel_id", self.reference.channel_id
            )
            self.reference.user = getattr(reference, "user", self.reference.user)
            self.reference.bot = getattr(reference, "bot", self.reference.bot)
            self.reference.conversation = getattr(
                reference, "conversation", self.reference.conversation
            )
            self.reference.service_url = getattr(
                reference, "service_url", self.reference.service_url
            )
            # The only attribute on self.reference without an initial value is activity_id, so if reference does not
            # have a value for activity_id, default self.reference.activity_id to None
            self.reference.activity_id = getattr(reference, "activity_id", None)

        self._next_id = 0

    async def process_activity(self, logic: Callable):
        """
        Begins listening to console input.
        :param logic:
        :return:
        """
        while True:
            msg = input()
            if msg is None:
                pass
            else:
                self._next_id += 1
                activity = Activity(
                    text=msg,
                    channel_id="console",
                    from_property=ChannelAccount(id="user", name="User1"),
                    recipient=ChannelAccount(id="bot", name="Bot"),
                    conversation=ConversationAccount(id="Convo1"),
                    type=ActivityTypes.message,
                    timestamp=datetime.datetime.now(),
                    id=str(self._next_id),
                )

                activity = TurnContext.apply_conversation_reference(
                    activity, self.reference, True
                )
                context = TurnContext(self, activity)
                await self.run_pipeline(context, logic)

    async def send_activities(self, context: TurnContext, activities: List[Activity]) -> List[ResourceResponse]:
        """
        Logs a series of activities to the console.
        :param context:
        :param activities:
        :return:
        """
        if context is None:
            raise TypeError(
                "ConsoleAdapter.send_activities(): `context` argument cannot be None."
            )
        if not isinstance(activities, list):
            raise TypeError(
                "ConsoleAdapter.send_activities(): `activities` argument must be a list."
            )
        if len(activities) == 0:
            raise ValueError(
                "ConsoleAdapter.send_activities(): `activities` argument cannot have a length of 0."
            )

        async def next_activity(i: int):
            responses = []

            if i < len(activities):
                responses.append(ResourceResponse())
                activity = activities[i]

                if activity.type == "delay":
                    await asyncio.sleep(activity.delay)
                    await next_activity(i + 1)
                elif activity.type == ActivityTypes.message:
                    if (
                        activity.attachments is not None
                        and len(activity.attachments) > 0
                    ):
                        append = (
                            "(1 attachment)"
                            if len(activity.attachments) == 1
                            else f"({len(activity.attachments)} attachments)"
                        )
                        print(f"{activity.text} {append}")
                    else:
                        print(activity.text)
                    await next_activity(i + 1)
                else:
                    print(f"[{activity.type}]")
                    await next_activity(i + 1)
            else:
                return responses

        await next_activity(0)

    async def delete_activity(
        self, context: TurnContext, reference: ConversationReference
    ):
        """
        Not supported for the ConsoleAdapter. Calling this method or `TurnContext.delete_activity()`
        will result an error being returned.
        :param context:
        :param reference:
        :return:
        """
        raise NotImplementedError("ConsoleAdapter.delete_activity(): not supported.")

    async def update_activity(self, context: TurnContext, activity: Activity):
        """
        Not supported for the ConsoleAdapter. Calling this method or `TurnContext.update_activity()`
        will result an error being returned.
        :param context:
        :param activity:
        :return:
        """
        raise NotImplementedError("ConsoleAdapter.update_activity(): not supported.")
