# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import (
    ActivityHandler,
    TurnContext,
    UserState,
    CardFactory,
    MessageFactory,
)
from botbuilder.schema import (
    ChannelAccount,
    HeroCard,
    CardImage,
    CardAction,
    ActionTypes,
)

from data_models import WelcomeUserState


class WelcomeUserBot(ActivityHandler):
    def __init__(self, user_state: UserState):
        if user_state is None:
            raise TypeError(
                "[WelcomeUserBot]: Missing parameter. user_state is required but None was given"
            )

        self._user_state = user_state

        self.user_state_accessor = self._user_state.create_property("WelcomeUserState")

        self.WELCOME_MESSAGE = """This is a simple Welcome Bot sample. This bot will introduce you
                        to welcoming and greeting users. You can say 'intro' to see the
                        introduction card. If you are running this bot in the Bot Framework
                        Emulator, press the 'Restart Conversation' button to simulate user joining
                        a bot or a channel"""

        self.INFO_MESSAGE = """You are seeing this message because the bot received at least one
                        'ConversationUpdate' event, indicating you (and possibly others)
                        joined the conversation. If you are using the emulator, pressing
                        the 'Start Over' button to trigger this event again. The specifics
                        of the 'ConversationUpdate' event depends on the channel. You can
                        read more information at: https://aka.ms/about-botframework-welcome-user"""

        self.LOCALE_MESSAGE = """"You can use the 'activity.locale' property to welcome the
                        user using the locale received from the channel. If you are using the 
                        Emulator, you can set this value in Settings."""

        self.PATTERN_MESSAGE = """It is a good pattern to use this event to send general greeting
                        to user, explaining what your bot can do. In this example, the bot
                        handles 'hello', 'hi', 'help' and 'intro'. Try it now, type 'hi'"""

    async def on_turn(self, turn_context: TurnContext):
        await super().on_turn(turn_context)

        # save changes to WelcomeUserState after each turn
        await self._user_state.save_changes(turn_context)

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        """
        Greet when users are added to the conversation.
        Note that all channels do not send the conversation update activity.
        If you find that this bot works in the emulator, but does not in
        another channel the reason is most likely that the channel does not
        send this activity.
        """
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    f"Hi there { member.name }. " + self.WELCOME_MESSAGE
                )

                await turn_context.send_activity(self.INFO_MESSAGE)

                await turn_context.send_activity(
                    f"{ self.LOCALE_MESSAGE } Current locale is { turn_context.activity.locale }"
                )

                await turn_context.send_activity(self.PATTERN_MESSAGE)

    async def on_message_activity(self, turn_context: TurnContext):
        """
        Respond to messages sent from the user.
        """
        # Get the state properties from the turn context.
        welcome_user_state = await self.user_state_accessor.get(
            turn_context, WelcomeUserState
        )

        if not welcome_user_state.did_welcome_user:
            welcome_user_state.did_welcome_user = True

            await turn_context.send_activity(
                "You are seeing this message because this was your first message ever to this bot."
            )

            name = turn_context.activity.from_property.name
            await turn_context.send_activity(
                f"It is a good practice to welcome the user and provide personal greeting. For example: Welcome {name}"
            )

        else:
            # This example hardcodes specific utterances. You should use LUIS or QnA for more advance language
            # understanding.
            text = turn_context.activity.text.lower()
            if text in ("hello", "hi"):
                await turn_context.send_activity(f"You said { text }")
            elif text in ("intro", "help"):
                await self.__send_intro_card(turn_context)
            else:
                await turn_context.send_activity(self.WELCOME_MESSAGE)

    async def __send_intro_card(self, turn_context: TurnContext):
        card = HeroCard(
            title="Welcome to Bot Framework!",
            text="Welcome to Welcome Users bot sample! This Introduction card "
            "is a great way to introduce your Bot to the user and suggest "
            "some things to get them started. We use this opportunity to "
            "recommend a few next steps for learning more creating and deploying bots.",
            images=[CardImage(url="https://aka.ms/bf-welcome-card-image")],
            buttons=[
                CardAction(
                    type=ActionTypes.open_url,
                    title="Get an overview",
                    text="Get an overview",
                    display_text="Get an overview",
                    value="https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0",
                ),
                CardAction(
                    type=ActionTypes.open_url,
                    title="Ask a question",
                    text="Ask a question",
                    display_text="Ask a question",
                    value="https://stackoverflow.com/questions/tagged/botframework",
                ),
                CardAction(
                    type=ActionTypes.open_url,
                    title="Learn how to deploy",
                    text="Learn how to deploy",
                    display_text="Learn how to deploy",
                    value="https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0",
                ),
            ],
        )

        return await turn_context.send_activity(
            MessageFactory.attachment(CardFactory.hero_card(card))
        )
