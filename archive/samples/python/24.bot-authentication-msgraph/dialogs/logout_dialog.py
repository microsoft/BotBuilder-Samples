# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.dialogs import DialogTurnResult, ComponentDialog, DialogContext
from botbuilder.core import BotFrameworkAdapter
from botbuilder.schema import ActivityTypes


class LogoutDialog(ComponentDialog):
    def __init__(self, dialog_id: str, connection_name: str):
        super(LogoutDialog, self).__init__(dialog_id)

        self.connection_name = connection_name

    async def on_begin_dialog(
        self, inner_dc: DialogContext, options: object
    ) -> DialogTurnResult:
        result = await self._interrupt(inner_dc)
        if result:
            return result
        return await super().on_begin_dialog(inner_dc, options)

    async def on_continue_dialog(self, inner_dc: DialogContext) -> DialogTurnResult:
        result = await self._interrupt(inner_dc)
        if result:
            return result
        return await super().on_continue_dialog(inner_dc)

    async def _interrupt(self, inner_dc: DialogContext):
        if inner_dc.context.activity.type == ActivityTypes.message:
            text = inner_dc.context.activity.text.lower()
            if text == "logout":
                bot_adapter: BotFrameworkAdapter = inner_dc.context.adapter
                await bot_adapter.sign_out_user(inner_dc.context, self.connection_name)
                await inner_dc.context.send_activity("You have been signed out.")
                return await inner_dc.cancel_all_dialogs()
