# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import json

from botbuilder.core import MessageFactory
from botbuilder.dialogs import (
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult
)
from botbuilder.dialogs.prompts import (
    OAuthPrompt,
    OAuthPromptSettings,
    ConfirmPrompt,
    PromptOptions,
    TextPrompt
)

from dialogs import LogoutDialog
from simple_graph_client import SimpleGraphClient


class MainDialog(LogoutDialog):
    def __init__(
        self, connection_name: str
    ):
        super(MainDialog, self).__init__(MainDialog.__name__, connection_name)

        self.add_dialog(
            OAuthPrompt(
                OAuthPrompt.__name__,
                OAuthPromptSettings(
                    connection_name=connection_name,
                    text="Please Sign In",
                    title="Sign In",
                    timeout=300000
                )
            )
        )

        self.add_dialog(TextPrompt(TextPrompt.__name__))
        self.add_dialog(ConfirmPrompt(ConfirmPrompt.__name__))

        self.add_dialog(
            WaterfallDialog(
                "WFDialog", [
                    self.prompt_step,
                    self.login_step,
                    self.command_step,
                    self.process_step
                ]
            )
        )

        self.initial_dialog_id = "WFDialog"

    async def prompt_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        return await step_context.begin_dialog(OAuthPrompt.__name__)

    async def login_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        # Get the token from the previous step. Note that we could also have gotten the
        # token directly from the prompt itself. There is an example of this in the next method.
        if step_context.result:
            await step_context.context.send_activity("You are now logged in.")
            return await step_context.prompt(TextPrompt.__name__, PromptOptions(
                prompt=MessageFactory.text("Would you like to do? (type 'me', 'send <EMAIL>' or 'recent')")
            ))

        await step_context.context.send_activity("Login was not successful please try again.")
        return await step_context.end_dialog()

    async def command_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        step_context.values["command"] = step_context.result

        # Call the prompt again because we need the token. The reasons for this are:
        # 1. If the user is already logged in we do not need to store the token locally in the bot and worry
        #    about refreshing it. We can always just call the prompt again to get the token.
        # 2. We never know how long it will take a user to respond. By the time the
        #    user responds the token may have expired. The user would then be prompted to login again.
        #
        # There is no reason to store the token locally in the bot because we can always just call
        # the OAuth prompt to get the token or get a new token if needed.
        return await step_context.begin_dialog(OAuthPrompt.__name__)

    async def process_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        if step_context.result:
            token_response = step_context.result
            if token_response and token_response.token:
                parts = step_context.values["command"].split(' ')
                command = parts[0]
                if command == "me":
                    client = SimpleGraphClient(token_response.token)
                    me = await client.get_me()
                    await step_context.context.send_activity(f"You are {me['displayName']}")
                elif command == "send":
                    client = SimpleGraphClient(token_response.token)
                    me = await client.get_me()
                    response = await client.send_mail(
                        parts[1],
                        "Message from a bot!",
                        f"Hi there! I had this message sent from a bot. - Your friend, {me['displayName']}"
                    )
                    await step_context.context.send_activity(f"I sent a message to { parts[1] } from your account.")
                elif command == "recent":
                    client = SimpleGraphClient(token_response.token)
                    messages = await client.get_recent_mail()
                    await step_context.context.send_activity("recent")
                else:
                    await step_context.context.send_activity(f"Your token is {token_response.token}")
        else:
            await step_context.context.send_activity("We couldn't log you in. Please try again later.")

        return await step_context.end_dialog()
