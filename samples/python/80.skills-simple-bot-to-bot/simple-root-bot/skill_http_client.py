# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import (
    BotFrameworkHttpClient,
    InvokeResponse,
    TurnContext,
)
from botbuilder.core.skills import (
    SkillConversationIdFactory,
    BotFrameworkSkill,
)
from botbuilder.schema import Activity
from botframework.connector.auth import SimpleCredentialProvider


class SkillHttpClient(BotFrameworkHttpClient):
    def __init__(
        self,
        credential_provider: SimpleCredentialProvider,
        skill_conversation_id_factory: SkillConversationIdFactory,
    ):
        if not skill_conversation_id_factory:
            raise TypeError("skill_conversation_id_factory can't be None")

        super().__init__(credential_provider)

        self._skill_conversation_id_factory = skill_conversation_id_factory

    async def post_activity(
        self,
        from_bot_id: str,
        to_skill: BotFrameworkSkill,
        service_url: str,
        activity: Activity,
    ) -> InvokeResponse:

        skill_conversation_id = await self._skill_conversation_id_factory.create_skill_conversation_id(
            TurnContext.get_conversation_reference(activity)
        )
        return await super().post_activity(
            from_bot_id,
            to_skill.app_id,
            to_skill.skill_endpoint,
            service_url,
            skill_conversation_id,
            activity,
        )
