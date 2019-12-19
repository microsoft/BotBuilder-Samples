# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import urllib.parse
import xmlrpc.client
from botbuilder.core import CardFactory, MessageFactory, TurnContext, UserState
from botbuilder.schema import (
    ThumbnailCard,
    CardImage,
    HeroCard,
    CardAction,
)
from botbuilder.schema.teams import (
    MessagingExtensionAction,
    MessagingExtensionAttachment,
    MessagingExtensionQuery,
    MessagingExtensionSuggestedAction,
    MessagingExtensionActionResponse,
    MessagingExtensionResult,
    MessagingExtensionResponse,
    TaskModuleContinueResponse,
    TaskModuleTaskInfo,
)
from botbuilder.core.teams import TeamsActivityHandler
from simple_graph_client import SimpleGraphClient


class TeamsMessagingExtensionsSearchAuthConfigBot(TeamsActivityHandler):
    def __init__(
        self, user_state: UserState, connection_name: str, site_url: str,
    ):
        if user_state is None:
            raise Exception(
                "[TeamsMessagingExtensionsSearchAuthConfigBot]: Missing parameter. user_state is required"
            )
        if connection_name is None:
            raise Exception(
                "[TeamsMessagingExtensionsSearchAuthConfigBot]: Missing parameter. connection_name is required"
            )
        if site_url is None:
            raise Exception(
                "[TeamsMessagingExtensionsSearchAuthConfigBot]: Missing parameter. site_url is required"
            )
        self.user_state = user_state
        self.connection_name = connection_name
        self.site_url = site_url
        self.user_config_property = user_state.create_property("UserConfiguration")

    async def on_turn(self, turn_context: TurnContext):
        await super().on_turn(turn_context)

        # Save any state changes that might have occurred during the turn.
        await self.user_state.save_changes(turn_context, False)

    async def on_teams_messaging_extension_configuration_query_settings_url(  # pylint: disable=unused-argument
        self, turn_context: TurnContext, query: MessagingExtensionQuery
    ) -> MessagingExtensionResponse:
        # The user has requested the Messaging Extension Configuration page.
        user_configuration = await self.user_config_property.get(
            turn_context, "UserConfiguration"
        )
        encoded_configuration = ""
        if user_configuration is not None:
            encoded_configuration = urllib.parse.quote_plus(user_configuration)

        return MessagingExtensionResponse(
            compose_extension=MessagingExtensionResult(
                type="config",
                suggested_actions=MessagingExtensionSuggestedAction(
                    actions=[
                        CardAction(
                            type="openUrl",
                            value=f"{self.site_url}/search_settings.html?settings={encoded_configuration}",
                        )
                    ]
                ),
            )
        )

    async def on_teams_messaging_extension_configuration_setting(  # pylint: disable=unused-argument
        self, turn_context: TurnContext, settings
    ):
        if "state" in settings:
            state = settings["state"]
            if state is not None:
                await self.user_config_property.set(turn_context, state)

    async def on_teams_messaging_extension_query(
        self,
        turn_context: TurnContext,
        query: MessagingExtensionQuery,  # pylint: disable=unused-argument
    ):
        search_query = str(query.parameters[0].value).strip()

        user_configuration = await self.user_config_property.get(
            turn_context, "UserConfiguration"
        )
        if user_configuration is not None and "email" in user_configuration:
            # When the Bot Service Auth flow completes, the action.State will contain
            # a magic code used for verification.
            magic_code = ""
            if query.state is not None:
                magic_code = query.state

            token_response = await turn_context.adapter.get_user_token(
                turn_context, self.connection_name, magic_code
            )
            if token_response is None or token_response.token is None:
                # There is no token, so the user has not signed in yet.

                # Retrieve the OAuth Sign in Link to use in the MessagingExtensionResult Suggested Actions
                sign_in_link = await turn_context.adapter.get_oauth_sign_in_link(
                    turn_context, self.connection_name
                )
                return MessagingExtensionResponse(
                    compose_extension=MessagingExtensionResult(
                        type="auth",
                        suggested_actions=MessagingExtensionSuggestedAction(
                            actions=[
                                CardAction(
                                    type="openUrl",
                                    value=sign_in_link,
                                    title="Bot Service OAuth",
                                )
                            ]
                        ),
                    )
                )
            # User is signed in, so use their token to search email via the Graph Client
            client = SimpleGraphClient(token_response.token)
            search_results = await client.search_mail_inbox(search_query)

            # Here we construct a ThumbnailCard for every attachment, and provide a HeroCard which will be
            # displayed if the user selects that item.
            attachments = []
            for message_meta in search_results:
                message = message_meta["_source"]
                message_from = message["from"] if "from" in message else None
                if message_from:
                    subtitle = (
                        f"{message_from['emailAddress']['name']},"
                        f"<{message_from['emailAddress']['address']}>"
                    )
                else:
                    subtitle = ""

                hero_card = HeroCard(
                    title=message["subject"] if "subject" in message else "",
                    subtitle=subtitle,
                    text=message["bodyPreview"] if "bodyPreview" in message else "",
                )

                thumbnail_card = CardFactory.thumbnail_card(
                    ThumbnailCard(
                        title=subtitle,
                        subtitle=message["subject"],
                        images=[
                            CardImage(
                                url="https://botframeworksamples.blob.core.windows.net/samples"
                                "/OutlookLogo.jpg",
                                alt="Outlook Logo",
                            )
                        ],
                    )
                )
                attachment = MessagingExtensionAttachment(
                    content_type=CardFactory.content_types.hero_card,
                    content=hero_card,
                    preview=thumbnail_card,
                )
                attachments.append(attachment)

            return MessagingExtensionResponse(
                compose_extension=MessagingExtensionResult(
                    type="result", attachment_layout="list", attachments=attachments
                )
            )

        # The user configuration is NOT set to search Email.
        if search_query is None:
            turn_context.send_activity(
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

    async def on_teams_messaging_extension_submit_action(
        self,
        turn_context: TurnContext,
        action: MessagingExtensionAction,  # pylint: disable=unused-argument
    ) -> MessagingExtensionActionResponse:
        # This method is to handle the 'Close' button on the confirmation Task Module after the user signs out.
        return MessagingExtensionActionResponse(task=None)

    async def on_teams_messaging_extension_fetch_task(
        self,
        turn_context: TurnContext,
        action: MessagingExtensionAction,  # pylint: disable=unused-argument
    ) -> MessagingExtensionActionResponse:
        if action.command_id == "SignOutCommand":
            await turn_context.adapter.sign_out_user(
                turn_context,
                self.connection_name,
                turn_context.activity.from_property.id,
            )
            card = CardFactory.adaptive_card(
                {
                    "actions": [{"type": "Action.Submit", "title": "Close",}],
                    "body": [
                        {
                            "text": "You have been signed out.",
                            "type": "TextBlock",
                            "weight": "bolder",
                        },
                    ],
                    "type": "AdaptiveCard",
                    "version": "1.0",
                }
            )

            task_info = TaskModuleTaskInfo(
                card=card, height=200, title="Adaptive Card Example", width=400
            )
            continue_response = TaskModuleContinueResponse(
                type="continue", value=task_info
            )
            return MessagingExtensionActionResponse(task=continue_response)

        return None

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
