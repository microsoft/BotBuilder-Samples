# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os
import urllib.parse
import urllib.request
import base64
import json

from botbuilder.core import ActivityHandler, MessageFactory, TurnContext, CardFactory
from botbuilder.schema import (
    ChannelAccount,
    HeroCard,
    CardAction,
    ActivityTypes,
    Attachment,
    AttachmentData,
    Activity,
    ActionTypes,
)


class AttachmentsBot(ActivityHandler):
    """
    Represents a bot that processes incoming activities.
    For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    This is a Transient lifetime service. Transient lifetime services are created
    each time they're requested. For each Activity received, a new instance of this
    class is created. Objects that are expensive to construct, or have a lifetime
    beyond the single turn, should be carefully managed.
    """

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        await self._send_welcome_message(turn_context)

    async def on_message_activity(self, turn_context: TurnContext):
        if (
            turn_context.activity.attachments
            and len(turn_context.activity.attachments) > 0
        ):
            await self._handle_incoming_attachment(turn_context)
        else:
            await self._handle_outgoing_attachment(turn_context)

        await self._display_options(turn_context)

    async def _send_welcome_message(self, turn_context: TurnContext):
        """
        Greet the user and give them instructions on how to interact with the bot.
        :param turn_context:
        :return:
        """
        for member in turn_context.activity.members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    f"Welcome to AttachmentsBot {member.name}. This bot will introduce "
                    f"you to Attachments. Please select an option"
                )
                await self._display_options(turn_context)

    async def _handle_incoming_attachment(self, turn_context: TurnContext):
        """
        Handle attachments uploaded by users. The bot receives an Attachment in an Activity.
        The activity has a List of attachments.
        Not all channels allow users to upload files. Some channels have restrictions
        on file type, size, and other attributes. Consult the documentation for the channel for
        more information. For example Skype's limits are here
        <see ref="https://support.skype.com/en/faq/FA34644/skype-file-sharing-file-types-size-and-time-limits"/>.
        :param turn_context:
        :return:
        """
        for attachment in turn_context.activity.attachments:
            attachment_info = await self._download_attachment_and_write(attachment)
            if "filename" in attachment_info:
                await turn_context.send_activity(
                    f"Attachment {attachment_info['filename']} has been received to {attachment_info['local_path']}"
                )

    async def _download_attachment_and_write(self, attachment: Attachment) -> dict:
        """
        Retrieve the attachment via the attachment's contentUrl.
        :param attachment:
        :return: Dict: keys "filename", "local_path"
        """
        try:
            response = urllib.request.urlopen(attachment.content_url)
            headers = response.info()

            # If user uploads JSON file, this prevents it from being written as
            # "{"type":"Buffer","data":[123,13,10,32,32,34,108..."
            if headers["content-type"] == "application/json":
                data = bytes(json.load(response)["data"])
            else:
                data = response.read()

            local_filename = os.path.join(os.getcwd(), attachment.name)
            with open(local_filename, "wb") as out_file:
                out_file.write(data)

            return {"filename": attachment.name, "local_path": local_filename}
        except Exception as exception:
            print(exception)
            return {}

    async def _handle_outgoing_attachment(self, turn_context: TurnContext):
        reply = Activity(type=ActivityTypes.message)

        first_char = turn_context.activity.text[0]
        if first_char == "1":
            reply.text = "This is an inline attachment."
            reply.attachments = [self._get_inline_attachment()]
        elif first_char == "2":
            reply.text = "This is an internet attachment."
            reply.attachments = [self._get_internet_attachment()]
        elif first_char == "3":
            reply.text = "This is an uploaded attachment."
            reply.attachments = [await self._get_upload_attachment(turn_context)]
        else:
            reply.text = "Your input was not recognized, please try again."

        await turn_context.send_activity(reply)

    async def _display_options(self, turn_context: TurnContext):
        """
        Create a HeroCard with options for the user to interact with the bot.
        :param turn_context:
        :return:
        """

        # Note that some channels require different values to be used in order to get buttons to display text.
        # In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
        # need to provide a value for other parameters like 'text' or 'displayText'.
        card = HeroCard(
            text="You can upload an image or select one of the following choices",
            buttons=[
                CardAction(
                    type=ActionTypes.im_back, title="1. Inline Attachment", value="1"
                ),
                CardAction(
                    type=ActionTypes.im_back, title="2. Internet Attachment", value="2"
                ),
                CardAction(
                    type=ActionTypes.im_back, title="3. Uploaded Attachment", value="3"
                ),
            ],
        )

        reply = MessageFactory.attachment(CardFactory.hero_card(card))
        await turn_context.send_activity(reply)

    def _get_inline_attachment(self) -> Attachment:
        """
        Creates an inline attachment sent from the bot to the user using a base64 string.
        Using a base64 string to send an attachment will not work on all channels.
        Additionally, some channels will only allow certain file types to be sent this way.
        For example a .png file may work but a .pdf file may not on some channels.
        Please consult the channel documentation for specifics.
        :return: Attachment
        """
        file_path = os.path.join(os.getcwd(), "resources/architecture-resize.png")
        with open(file_path, "rb") as in_file:
            base64_image = base64.b64encode(in_file.read()).decode()

        return Attachment(
            name="architecture-resize.png",
            content_type="image/png",
            content_url=f"data:image/png;base64,{base64_image}",
        )

    async def _get_upload_attachment(self, turn_context: TurnContext) -> Attachment:
        """
        Creates an "Attachment" to be sent from the bot to the user from an uploaded file.
        :param turn_context:
        :return: Attachment
        """
        with open(
            os.path.join(os.getcwd(), "resources/architecture-resize.png"), "rb"
        ) as in_file:
            image_data = in_file.read()

        connector = await turn_context.adapter.create_connector_client(
            turn_context.activity.service_url
        )
        conversation_id = turn_context.activity.conversation.id
        response = await connector.conversations.upload_attachment(
            conversation_id,
            AttachmentData(
                name="architecture-resize.png",
                original_base64=image_data,
                type="image/png",
            ),
        )

        base_uri: str = connector.config.base_url
        attachment_uri = (
            base_uri
            + ("" if base_uri.endswith("/") else "/")
            + f"v3/attachments/{response.id}/views/original"
        )

        return Attachment(
            name="architecture-resize.png",
            content_type="image/png",
            content_url=attachment_uri,
        )

    def _get_internet_attachment(self) -> Attachment:
        """
        Creates an Attachment to be sent from the bot to the user from a HTTP URL.
        :return: Attachment
        """
        return Attachment(
            name="architecture-resize.png",
            content_type="image/png",
            content_url="https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
        )
