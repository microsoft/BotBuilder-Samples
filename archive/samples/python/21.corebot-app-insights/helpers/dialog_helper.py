# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Utility to run dialogs."""
from botbuilder.core import StatePropertyAccessor, TurnContext
from botbuilder.dialogs import Dialog, DialogSet, DialogTurnStatus


class DialogHelper:
    """Dialog Helper implementation."""

    @staticmethod
    async def run_dialog(
        dialog: Dialog, turn_context: TurnContext, accessor: StatePropertyAccessor
    ):  # pylint: disable=line-too-long
        """Run dialog."""
        dialog_set = DialogSet(accessor)
        dialog_set.add(dialog)

        dialog_context = await dialog_set.create_context(turn_context)
        results = await dialog_context.continue_dialog()
        if results.status == DialogTurnStatus.Empty:
            await dialog_context.begin_dialog(dialog.id)
