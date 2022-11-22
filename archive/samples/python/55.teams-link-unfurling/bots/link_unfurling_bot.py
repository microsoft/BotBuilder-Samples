# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import TurnContext, CardFactory
from botbuilder.core.card_factory import ContentTypes
from botbuilder.core.teams import TeamsActivityHandler
from botbuilder.schema import ThumbnailCard, CardImage, HeroCard
from botbuilder.schema.teams import (
    AppBasedLinkQuery,
    MessagingExtensionQuery,
    MessagingExtensionAttachment,
    MessagingExtensionResult,
    MessagingExtensionResponse,
)


class LinkUnfurlingBot(TeamsActivityHandler):
    async def on_teams_app_based_link_query(
        self, turn_context: TurnContext, query: AppBasedLinkQuery
    ):
        hero_card = ThumbnailCard(
            title="Thumbnail Card",
            text=query.url,
            images=[
                CardImage(
                    url="https://raw.githubusercontent.com/microsoft/botframework-sdk/master/icon.png"
                )
            ],
        )

        attachments = MessagingExtensionAttachment(
            content_type=ContentTypes.hero_card, content=hero_card
        )

        result = MessagingExtensionResult(
            attachment_layout="list", type="result", attachments=[attachments]
        )

        return MessagingExtensionResponse(compose_extension=result)

    async def on_teams_messaging_extension_query(
        self, turn_context: TurnContext, query: MessagingExtensionQuery
    ):
        # These commandIds are defined in the Teams App Manifest.
        if not query.command_id == "searchQuery":
            raise NotImplementedError(f"Invalid CommandId: {query.command_id}")

        card = HeroCard(
            title="This is a Link Unfurling Sample",
            subtitle="It will unfurl links from *.BotFramework.com",
            text="This sample demonstrates how to handle link unfurling in Teams.  Please review the readme for more "
            "information. ",
        )

        return MessagingExtensionResponse(
            compose_extension=MessagingExtensionResult(
                attachment_layout="list",
                type="result",
                attachments=[
                    MessagingExtensionAttachment(
                        content=card,
                        content_type=ContentTypes.hero_card,
                        preview=CardFactory.hero_card(card),
                    )
                ],
            )
        )
