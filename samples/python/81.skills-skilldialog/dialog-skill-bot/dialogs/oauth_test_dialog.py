# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.dialogs import (
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
)
from botbuilder.dialogs.prompts import (
    ConfirmPrompt,
    PromptOptions,
    OAuthPrompt,
    OAuthPromptSettings,
)
from botbuilder.core import BotFrameworkAdapter, MessageFactory
from botbuilder.schema import InputHints

from config import DefaultConfig
from .cancel_and_help_dialog import CancelAndHelpDialog


class OAuthTestDialog(CancelAndHelpDialog):
    def __init__(self, configuration: DefaultConfig):
        super().__init__(OAuthTestDialog.__name__)

        self._connection_name = configuration.CONNECTION_NAME

        self.add_dialog(
            OAuthPrompt(
                OAuthPrompt.__name__,
                OAuthPromptSettings(
                    connection_name=self._connection_name,
                    text=f"Please Sign In to connection: '{self._connection_name}'",
                    title="Sign In",
                    timeout=300000,
                ),
            )
        )

        self.add_dialog(
            WaterfallDialog(
                WaterfallDialog.__name__, [self.prompt_step, self.login_step]
            )
        )

        self.initial_dialog_id = WaterfallDialog.__name__

    async def prompt_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        return await step_context.begin_dialog(OAuthPrompt.__name__)

    async def login_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        # Get the token from the previous step.
        token_response = step_context.result
        if token_response:
            # Show the token
            logged_in_message = "You are now logged in."
            show_token_message = "Here is your token:"
            await step_context.context.send_activity(
                MessageFactory.text(
                    logged_in_message, logged_in_message, InputHints.ignoring_input
                )
            )
            await step_context.context.send_activity(
                MessageFactory.text(
                    f"{show_token_message} {token_response.token}",
                    show_token_message,
                    InputHints.ignoring_input,
                )
            )

            # Sign out
            bot_adapter: BotFrameworkAdapter = step_context.context.adapter
            await bot_adapter.sign_out_user(step_context.context, self._connection_name)
            sign_out_message = "You have been signed out."
            await step_context.context.send_activity(
                MessageFactory.text(
                    sign_out_message, sign_out_message, InputHints.ignoring_input
                )
            )

            return await step_context.end_dialog()

        try_again_message = "Login was not successful please try again."
        await step_context.context.send_activity(
            MessageFactory.text(
                try_again_message, try_again_message, InputHints.ignoring_input
            )
        )
        return await step_context.end_dialog()
