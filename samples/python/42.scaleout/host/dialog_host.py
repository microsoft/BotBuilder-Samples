# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json

from jsonpickle import encode
from jsonpickle.unpickler import Unpickler

from botbuilder.core import TurnContext
from botbuilder.dialogs import Dialog, ComponentDialog
from botbuilder.schema import Activity

from helpers.dialog_helper import DialogHelper
from host.dialog_host_adapter import DialogHostAdapter
from store import RefAccessor


class DialogHost:
    """
    The essential code for running a dialog. The execution of the dialog is treated here as a pure function call.
    The input being the existing (or old) state and the inbound Activity and the result being the updated (or new)
    state and the Activities that should be sent. The assumption is that this code can be re-run without causing any
    unintended or harmful side-effects, for example, any outbound service calls made directly from the
    dialog implementation should be idempotent.
    """

    @staticmethod
    async def run(dialog: Dialog, activity: Activity, old_state) -> ():
        """
        A function to run a dialog while buffering the outbound Activities.
        """

        # A custom adapter and corresponding TurnContext that buffers any messages sent.
        adapter = DialogHostAdapter()
        turn_context = TurnContext(adapter, activity)

        # Run the dialog using this TurnContext with the existing state.
        new_state = await DialogHost.__run_turn(dialog, turn_context, old_state)

        # The result is a set of activities to send and a replacement state.
        return adapter.activities, new_state

    @staticmethod
    async def __run_turn(dialog: Dialog, turn_context: TurnContext, state):
        """
        Execute the turn of the bot. The functionality here closely resembles that which is found in the
        Bot.on_turn method in an implementation that is using the regular BotFrameworkAdapter.
        Also here in this example the focus is explicitly on Dialogs but the pattern could be adapted
        to other conversation modeling abstractions.
        """
        # If we have some state, deserialize it. (This mimics the shape produced by BotState.cs.)
        dialog_state_property = (
            state[ComponentDialog.persisted_dialog_state] if state else None
        )
        dialog_state = (
            None
            if not dialog_state_property
            else Unpickler().restore(json.loads(dialog_state_property))
        )

        # A custom accessor is used to pass a handle on the state to the dialog system.
        accessor = RefAccessor(dialog_state)

        # Run the dialog.
        await DialogHelper.run_dialog(dialog, turn_context, accessor)

        # Serialize the result (available as Value on the accessor), and put its value back into a new json object.
        return {
            ComponentDialog.persisted_dialog_state: None
            if not accessor.value
            else encode(accessor.value)
        }
