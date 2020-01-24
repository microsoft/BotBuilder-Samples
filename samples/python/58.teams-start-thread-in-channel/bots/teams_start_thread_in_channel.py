# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import CardFactory, TurnContext, MessageFactory, BotFrameworkAdapter
from botbuilder.core.teams import TeamsActivityHandler, TeamsInfo, teams_get_channel_id
from botframework.connector.auth import MicrosoftAppCredentials
from botbuilder.schema import CardAction, HeroCard, Mention, ConversationParameters
from botbuilder.schema._connector_client_enums import ActionTypes


class TeamsStartThreadInChannel(TeamsActivityHandler):
    def __init__(self, app_id: str):
        self._app_id = app_id

    async def on_message_activity(self, turn_context: TurnContext):
        teams_channel_id = teams_get_channel_id(turn_context.activity)
        message = MessageFactory.text("This will be the start of a new thread")
        new_conversation = await self.teams_create_conversation(turn_context, teams_channel_id, message)

        await turn_context.adapter.continue_conversation(
                                                        new_conversation[0], 
                                                        self.continue_conversation_callback, 
                                                        self._app_id
                                                        )

    async def teams_create_conversation(self, turn_context: TurnContext, teams_channel_id: str, message):
        params = ConversationParameters(
                                        is_group=True, 
                                        channel_data={"channel": {"id": teams_channel_id}},
                                        activity=message
                                        )

        
        connector_client = await turn_context.adapter.create_connector_client(turn_context.activity.service_url)
        conversation_resource_response = await connector_client.conversations.create_conversation(params)
        conversation_reference = TurnContext.get_conversation_reference(turn_context.activity)
        conversation_reference.conversation.id = conversation_resource_response.id
        return [conversation_reference, conversation_resource_response.activity_id]
    
    async def continue_conversation_callback(self, t):
        await t.send_activity(MessageFactory.text("This will be the first reply to my new thread"))
