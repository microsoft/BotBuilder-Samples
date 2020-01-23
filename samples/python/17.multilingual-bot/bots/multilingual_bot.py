# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json
import os

from botbuilder.core import (
    ActivityHandler,
    TurnContext,
    UserState,
    CardFactory,
    MessageFactory,
)
from botbuilder.schema import (
    ChannelAccount,
    Attachment,
    SuggestedActions,
    CardAction,
    ActionTypes,
)

from translation.translation_settings import TranslationSettings


class MultiLingualBot(ActivityHandler):
    """
    This bot demonstrates how to use Microsoft Translator.
    More information can be found at:
    https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-info-overview"
    """

    def __init__(self, user_state: UserState):
        if user_state is None:
            raise TypeError(
                "[MultiLingualBot]: Missing parameter. user_state is required but None was given"
            )

        self.user_state = user_state

        self.language_preference_accessor = self.user_state.create_property(
            "LanguagePreference"
        )

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        # Greet anyone that was not the target (recipient) of this message.
        # To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    MessageFactory.attachment(self._create_adaptive_card_attachment())
                )
                await turn_context.send_activity(
                    "This bot will introduce you to translation middleware. Say 'hi' to get started."
                )

    async def on_message_activity(self, turn_context: TurnContext):
        if self._is_language_change_requested(turn_context.activity.text):
            # If the user requested a language change through the suggested actions with values "es" or "en",
            # simply change the user's language preference in the user state.
            # The translation middleware will catch this setting and translate both ways to the user's
            # selected language.
            # If Spanish was selected by the user, the reply below will actually be shown in Spanish to the user.
            current_language = turn_context.activity.text.lower()
            if current_language in (
                    TranslationSettings.english_english.value, TranslationSettings.spanish_english.value
            ):
                lang = TranslationSettings.english_english.value
            else:
                lang = TranslationSettings.english_spanish.value

            await self.language_preference_accessor.set(turn_context, lang)

            await turn_context.send_activity(f"Your current language code is: {lang}")

            # Save the user profile updates into the user state.
            await self.user_state.save_changes(turn_context)
        else:
            # Show the user the possible options for language. If the user chooses a different language
            # than the default, then the translation middleware will pick it up from the user state and
            # translate messages both ways, i.e. user to bot and bot to user.
            reply = MessageFactory.text("Choose your language:")
            reply.suggested_actions = SuggestedActions(
                actions=[
                    CardAction(
                        title="Español",
                        type=ActionTypes.post_back,
                        value=TranslationSettings.english_spanish.value,
                    ),
                    CardAction(
                        title="English",
                        type=ActionTypes.post_back,
                        value=TranslationSettings.english_english.value,
                    ),
                ]
            )

            await turn_context.send_activity(reply)

    def _create_adaptive_card_attachment(self) -> Attachment:
        """
        Load attachment from file.
        :return:
        """
        card_path = os.path.join(os.getcwd(), "cards/welcomeCard.json")
        with open(card_path, "rt") as in_file:
            card_data = json.load(in_file)

        return CardFactory.adaptive_card(card_data)

    def _is_language_change_requested(self, utterance: str) -> bool:
        if not utterance:
            return False

        utterance = utterance.lower()
        return utterance in (
            TranslationSettings.english_spanish.value,
            TranslationSettings.english_english.value,
            TranslationSettings.spanish_spanish.value,
            TranslationSettings.spanish_english.value
        )
