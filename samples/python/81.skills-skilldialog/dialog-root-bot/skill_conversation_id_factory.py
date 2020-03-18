# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
from typing import Union

from botbuilder.core import Storage, TurnContext
from botbuilder.core.skills import (
    ConversationIdFactoryBase,
    SkillConversationIdFactoryOptions,
    SkillConversationReference,
)
from botbuilder.schema import ConversationReference


class SkillConversationIdFactory(ConversationIdFactoryBase):
    def __init__(self, storage: Storage):
        if not storage:
            raise TypeError("storage can't be None")

        self._storage = storage

    async def create_skill_conversation_id(
        self,
        options_or_conversation_reference: Union[
            SkillConversationIdFactoryOptions, ConversationReference
        ],
    ) -> str:
        if not options_or_conversation_reference:
            raise TypeError("Need options or conversation reference")

        if not isinstance(
            options_or_conversation_reference, SkillConversationIdFactoryOptions
        ):
            raise TypeError(
                "This SkillConversationIdFactory can only handle SkillConversationIdFactoryOptions"
            )

        options = options_or_conversation_reference

        # Create the storage key based on the SkillConversationIdFactoryOptions.
        conversation_reference = TurnContext.get_conversation_reference(
            options.activity
        )
        skill_conversation_id = (
            f"{conversation_reference.conversation.id}"
            f"-{options.bot_framework_skill.id}"
            f"-{conversation_reference.channel_id}"
            f"-skillconvo"
        )

        # Create the SkillConversationReference instance.
        skill_conversation_reference = SkillConversationReference(
            conversation_reference=conversation_reference,
            oauth_scope=options.from_bot_oauth_scope,
        )

        # Store the SkillConversationReference using the skill_conversation_id as a key.
        skill_conversation_info = {skill_conversation_id: skill_conversation_reference}
        await self._storage.write(skill_conversation_info)

        # Return the generated skill_conversation_id (that will be also used as the conversation ID to call the skill).
        return skill_conversation_id

    async def get_skill_conversation_reference(
        self, skill_conversation_id: str
    ) -> Union[SkillConversationReference, ConversationReference]:
        if not skill_conversation_id:
            raise TypeError("skill_conversation_id can't be None")

        # Get the SkillConversationReference from storage for the given skill_conversation_id.
        skill_conversation_info = await self._storage.read([skill_conversation_id])

        return skill_conversation_info.get(skill_conversation_id)

    async def delete_conversation_reference(self, skill_conversation_id: str):
        await self._storage.delete([skill_conversation_id])
