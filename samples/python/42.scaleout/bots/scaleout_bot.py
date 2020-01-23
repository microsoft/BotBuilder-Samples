# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import ActivityHandler, TurnContext
from botbuilder.dialogs import Dialog

from host import DialogHost
from store import Store


class ScaleoutBot(ActivityHandler):
    """
    This bot runs Dialogs that send message Activities in a way that can be scaled out with a multi-machine deployment.
    The bot logic makes use of the standard HTTP ETag/If-Match mechanism for optimistic locking. This mechanism
    is commonly supported on cloud storage technologies from multiple vendors including teh Azure Blob Storage
    service. A full implementation against Azure Blob Storage is included in this sample.
    """

    def __init__(self, store: Store, dialog: Dialog):
        self.store = store
        self.dialog = dialog

    async def on_message_activity(self, turn_context: TurnContext):
        # Create the storage key for this conversation.
        key = f"{turn_context.activity.channel_id}/conversations/{turn_context.activity.conversation.id}"

        # The execution sits in a loop because there might be a retry if the save operation fails.
        while True:
            # Load any existing state associated with this key
            old_state, e_tag = await self.store.load(key)

            # Run the dialog system with the old state and inbound activity, the result is a new state and outbound
            # activities.
            activities, new_state = await DialogHost.run(
                self.dialog, turn_context.activity, old_state
            )

            # Save the updated state associated with this key.
            success = await self.store.save(key, new_state, e_tag)
            if success:
                if activities:
                    # This is an actual send on the TurnContext we were given and so will actual do a send this time.
                    await turn_context.send_activities(activities)

                break
