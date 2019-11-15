# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List, Dict

from botbuilder.dialogs import (
    DialogContext,
    DialogTurnResult,
    Dialog,
    DialogInstance,
    DialogReason,
)
from botbuilder.schema import ActivityTypes

from dialogs.slot_details import SlotDetails


class SlotFillingDialog(Dialog):
    """
    This is an example of implementing a custom Dialog class. This is similar to the Waterfall dialog in the
    framework; however, it is based on a Dictionary rather than a sequential set of functions. The dialog is defined
    by a list of 'slots', each slot represents a property we want to gather and the dialog we will be using to
    collect it. Often the property is simply an atomic piece of data such as a number or a date. But sometimes the
    property is itself a complex object, in which case we can use the slot dialog to collect that compound property.
    """

    def __init__(self, dialog_id: str, slots: List[SlotDetails]):
        super(SlotFillingDialog, self).__init__(dialog_id)

        # Custom dialogs might define their own custom state. Similarly to the Waterfall dialog we will have a set of
        # values in the ConversationState. However, rather than persisting an index we will persist the last property
        # we prompted for. This way when we resume this code following a prompt we will have remembered what property
        # we were filling.
        self.SLOT_NAME = "slot"
        self.PERSISTED_VALUES = "values"

        # The list of slots defines the properties to collect and the dialogs to use to collect them.
        self.slots = slots

    async def begin_dialog(
        self, dialog_context: "DialogContext", options: object = None
    ):
        if dialog_context.context.activity.type != ActivityTypes.message:
            return await dialog_context.end_dialog({})
        return await self._run_prompt(dialog_context)

    async def continue_dialog(self, dialog_context: "DialogContext"):
        if dialog_context.context.activity.type != ActivityTypes.message:
            return Dialog.end_of_turn
        return await self._run_prompt(dialog_context)

    async def resume_dialog(
        self, dialog_context: DialogContext, reason: DialogReason, result: object
    ):
        slot_name = dialog_context.active_dialog.state[self.SLOT_NAME]
        values = self._get_persisted_values(dialog_context.active_dialog)
        values[slot_name] = result

        return await self._run_prompt(dialog_context)

    async def _run_prompt(self, dialog_context: DialogContext) -> DialogTurnResult:
        """
        This helper function contains the core logic of this dialog. The main idea is to compare the state we have
        gathered with the list of slots we have been asked to fill. When we find an empty slot we execute the
        corresponding prompt.
        :param dialog_context:
        :return:
        """
        state = self._get_persisted_values(dialog_context.active_dialog)

        # Run through the list of slots until we find one that hasn't been filled yet.
        unfilled_slot = None
        for slot_detail in self.slots:
            if slot_detail.name not in state:
                unfilled_slot = slot_detail
                break

        # If we have an unfilled slot we will try to fill it
        if unfilled_slot:
            # The name of the slot we will be prompting to fill.
            dialog_context.active_dialog.state[self.SLOT_NAME] = unfilled_slot.name

            # Run the child dialog
            return await dialog_context.begin_dialog(
                unfilled_slot.dialog_id, unfilled_slot.options
            )

        # No more slots to fill so end the dialog.
        return await dialog_context.end_dialog(state)

    def _get_persisted_values(
        self, dialog_instance: DialogInstance
    ) -> Dict[str, object]:
        obj = dialog_instance.state.get(self.PERSISTED_VALUES)

        if not obj:
            obj = {}
            dialog_instance.state[self.PERSISTED_VALUES] = obj

        return obj
