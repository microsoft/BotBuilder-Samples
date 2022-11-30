# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import xmlrpc.client

from botbuilder.core import CardFactory, MessageFactory, TurnContext
from botbuilder.schema import HeroCard, CardAction
from botbuilder.schema.teams import (
    MessagingExtensionAttachment,
    MessagingExtensionQuery,
    MessagingExtensionResult,
    MessagingExtensionResponse,
)
from botbuilder.core.teams import TeamsActivityHandler


class SearchBasedMessagingExtension(TeamsActivityHandler):
    async def on_teams_messaging_extension_query(
        self, turn_context: TurnContext, query: MessagingExtensionQuery
    ):
        search_query = str(query.parameters[0].value).strip()
        if search_query == "":
            await turn_context.send_activity(
                MessageFactory.text("You cannot enter a blank string for the search")
            )
            return

        search_results = self._get_search_results(search_query)

        attachments = []
        for obj in search_results:
            hero_card = HeroCard(
                title=obj["name"], tap=CardAction(type="invoke", value=obj)
            )

            attachment = MessagingExtensionAttachment(
                content_type=CardFactory.content_types.hero_card,
                content=HeroCard(title=obj["name"]),
                preview=CardFactory.hero_card(hero_card),
            )
            attachments.append(attachment)
        return MessagingExtensionResponse(
            compose_extension=MessagingExtensionResult(
                type="result", attachment_layout="list", attachments=attachments
            )
        )

    async def on_teams_messaging_extension_select_item(
        self, turn_context: TurnContext, query
    ) -> MessagingExtensionResponse:
        hero_card = HeroCard(
            title=query["name"],
            subtitle=query["summary"],
            buttons=[
                CardAction(
                    type="openUrl", value=f"https://pypi.org/project/{query['name']}"
                )
            ],
        )
        attachment = MessagingExtensionAttachment(
            content_type=CardFactory.content_types.hero_card, content=hero_card
        )

        return MessagingExtensionResponse(
            compose_extension=MessagingExtensionResult(
                type="result", attachment_layout="list", attachments=[attachment]
            )
        )

    def _get_search_results(self, query: str):
        client = xmlrpc.client.ServerProxy("https://pypi.org/pypi")
        search_results = client.search({"name": query})
        return search_results[:10] if len(search_results) > 10 else search_results
