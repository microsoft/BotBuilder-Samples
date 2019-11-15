# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.dialogs.choices import Choice, ChoiceFactory
from botbuilder.core import ActivityHandler, MessageFactory, TurnContext, CardFactory
from botbuilder.schema import ChannelAccount, CardAction, ActionTypes, HeroCard

FACEBOOK_PAGEID_OPTION = "Facebook Id"
QUICK_REPLIES_OPTION = "Quick Replies"
POSTBACK_OPTION = "PostBack"


class FacebookBot(ActivityHandler):
    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity("Hello and welcome!")

    async def on_message_activity(self, turn_context: TurnContext):
        if not await self._process_facebook_payload(
            turn_context, turn_context.activity.channel_data
        ):
            await self._show_choices(turn_context)

    async def on_event_activity(self, turn_context: TurnContext):
        await self._process_facebook_payload(turn_context, turn_context.activity.value)

    async def _show_choices(self, turn_context: TurnContext):
        choices = [
            Choice(
                value=QUICK_REPLIES_OPTION,
                action=CardAction(
                    title=QUICK_REPLIES_OPTION,
                    type=ActionTypes.post_back,
                    value=QUICK_REPLIES_OPTION,
                ),
            ),
            Choice(
                value=FACEBOOK_PAGEID_OPTION,
                action=CardAction(
                    title=FACEBOOK_PAGEID_OPTION,
                    type=ActionTypes.post_back,
                    value=FACEBOOK_PAGEID_OPTION,
                ),
            ),
            Choice(
                value=POSTBACK_OPTION,
                action=CardAction(
                    title=POSTBACK_OPTION,
                    type=ActionTypes.post_back,
                    value=POSTBACK_OPTION,
                ),
            ),
        ]

        message = ChoiceFactory.for_channel(
            turn_context.activity.channel_id,
            choices,
            "What Facebook feature would you like to try? Here are some quick replies to choose from!",
        )
        await turn_context.send_activity(message)

    async def _process_facebook_payload(self, turn_context: TurnContext, data) -> bool:
        if "postback" in data:
            await self._on_facebook_postback(turn_context, data["postback"])
            return True

        if "optin" in data:
            await self._on_facebook_optin(turn_context, data["optin"])
            return True

        if "message" in data and "quick_reply" in data["message"]:
            await self._on_facebook_quick_reply(
                turn_context, data["message"]["quick_reply"]
            )
            return True

        if "message" in data and data["message"]["is_echo"]:
            await self._on_facebook_echo(turn_context, data["message"])
            return True

    async def _on_facebook_postback(
        self, turn_context: TurnContext, facebook_postback: dict
    ):
        # TODO: Your PostBack handling logic here...

        reply = MessageFactory.text(f"Postback: {facebook_postback}")
        await turn_context.send_activity(reply)
        await self._show_choices(turn_context)

    async def _on_facebook_quick_reply(
        self, turn_context: TurnContext, facebook_quick_reply: dict
    ):
        # TODO: Your quick reply event handling logic here...

        if turn_context.activity.text == FACEBOOK_PAGEID_OPTION:
            reply = MessageFactory.text(
                f"This message comes from the following Facebook Page: {turn_context.activity.recipient.id}"
            )
            await turn_context.send_activity(reply)
            await self._show_choices(turn_context)
        elif turn_context.activity.text == POSTBACK_OPTION:
            card = HeroCard(
                text="Is 42 the answer to the ultimate question of Life, the Universe, and Everything?",
                buttons=[
                    CardAction(title="Yes", type=ActionTypes.post_back, value="Yes"),
                    CardAction(title="No", type=ActionTypes.post_back, value="No"),
                ],
            )
            reply = MessageFactory.attachment(CardFactory.hero_card(card))
            await turn_context.send_activity(reply)
        else:
            print(facebook_quick_reply)
            await turn_context.send_activity("Quick Reply")
            await self._show_choices(turn_context)

    async def _on_facebook_optin(self, turn_context: TurnContext, facebook_optin: dict):
        # TODO: Your optin event handling logic here...
        print(facebook_optin)
        await turn_context.send_activity("Opt In")

    async def _on_facebook_echo(
        self, turn_context: TurnContext, facebook_message: dict
    ):
        # TODO: Your echo event handling logic here...
        print(facebook_message)
        await turn_context.send_activity("Echo")
