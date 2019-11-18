# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json
import os
import random

from botbuilder.core import ActivityHandler, TurnContext, CardFactory
from botbuilder.schema import ChannelAccount, Attachment, Activity, ActivityTypes

CARDS = [
    "resources/FlightItineraryCard.json",
    "resources/ImageGalleryCard.json",
    "resources/LargeWeatherCard.json",
    "resources/RestaurantCard.json",
    "resources/SolitaireCard.json"
]


class AdaptiveCardsBot(ActivityHandler):
    """
    This bot will respond to the user's input with an Adaptive Card. Adaptive Cards are a way for developers to
    exchange card content in a common and consistent way. A simple open card format enables an ecosystem of shared
    tooling, seamless integration between apps, and native cross-platform performance on any device. For each user
    interaction, an instance of this class is created and the OnTurnAsync method is called.  This is a Transient
    lifetime service. Transient lifetime services are created each time they're requested. For each Activity
    received, a new instance of this class is created. Objects that are expensive to construct, or have a lifetime
    beyond the single turn, should be carefully managed.
    """

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    f"Welcome to Adaptive Cards Bot  {member.name}. This bot will "
                    f"introduce you to Adaptive Cards. Type anything to see an Adaptive "
                    f"Card."
                )

    async def on_message_activity(self, turn_context: TurnContext):
        message = Activity(
            text="Here is an Adaptive Card:",
            type=ActivityTypes.message,
            attachments=[self._create_adaptive_card_attachment()],
        )

        await turn_context.send_activity(message)

    def _create_adaptive_card_attachment(self) -> Attachment:
        """
        Load a random adaptive card attachment from file.
        :return:
        """
        random_card_index = random.randint(0, len(CARDS) - 1)
        card_path = os.path.join(os.getcwd(), CARDS[random_card_index])
        with open(card_path, "rt") as in_file:
            card_data = json.load(in_file)

        return CardFactory.adaptive_card(card_data)
