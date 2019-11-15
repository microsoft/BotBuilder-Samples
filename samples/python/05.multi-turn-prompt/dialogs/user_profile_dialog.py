# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
)
from botbuilder.dialogs.prompts import (
    TextPrompt,
    NumberPrompt,
    ChoicePrompt,
    ConfirmPrompt,
    PromptOptions,
    PromptValidatorContext,
)
from botbuilder.dialogs.choices import Choice
from botbuilder.core import MessageFactory, UserState

from data_models import UserProfile


class UserProfileDialog(ComponentDialog):
    def __init__(self, user_state: UserState):
        super(UserProfileDialog, self).__init__(UserProfileDialog.__name__)

        self.user_profile_accessor = user_state.create_property("UserProfile")

        self.add_dialog(
            WaterfallDialog(
                WaterfallDialog.__name__,
                [
                    self.transport_step,
                    self.name_step,
                    self.name_confirm_step,
                    self.age_step,
                    self.confirm_step,
                    self.summary_step,
                ],
            )
        )
        self.add_dialog(TextPrompt(TextPrompt.__name__))
        self.add_dialog(
            NumberPrompt(NumberPrompt.__name__, UserProfileDialog.age_prompt_validator)
        )
        self.add_dialog(ChoicePrompt(ChoicePrompt.__name__))
        self.add_dialog(ConfirmPrompt(ConfirmPrompt.__name__))

        self.initial_dialog_id = WaterfallDialog.__name__

    async def transport_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        # WaterfallStep always finishes with the end of the Waterfall or with another dialog;
        # here it is a Prompt Dialog. Running a prompt here means the next WaterfallStep will
        # be run when the users response is received.
        return await step_context.prompt(
            ChoicePrompt.__name__,
            PromptOptions(
                prompt=MessageFactory.text("Please enter your mode of transport."),
                choices=[Choice("Car"), Choice("Bus"), Choice("Bicycle")],
            ),
        )

    async def name_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        step_context.values["transport"] = step_context.result.value

        return await step_context.prompt(
            TextPrompt.__name__,
            PromptOptions(prompt=MessageFactory.text("Please enter your name.")),
        )

    async def name_confirm_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        step_context.values["name"] = step_context.result

        # We can send messages to the user at any point in the WaterfallStep.
        await step_context.context.send_activity(
            MessageFactory.text(f"Thanks {step_context.result}")
        )

        # WaterfallStep always finishes with the end of the Waterfall or
        # with another dialog; here it is a Prompt Dialog.
        return await step_context.prompt(
            ConfirmPrompt.__name__,
            PromptOptions(
                prompt=MessageFactory.text("Would you like to give your age?")
            ),
        )

    async def age_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        if step_context.result:
            # User said "yes" so we will be prompting for the age.
            # WaterfallStep always finishes with the end of the Waterfall or with another dialog,
            # here it is a Prompt Dialog.
            return await step_context.prompt(
                NumberPrompt.__name__,
                PromptOptions(
                    prompt=MessageFactory.text("Please enter your age."),
                    retry_prompt=MessageFactory.text(
                        "The value entered must be greater than 0 and less than 150."
                    ),
                ),
            )

        # User said "no" so we will skip the next step. Give -1 as the age.
        return await step_context.next(-1)

    async def confirm_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        age = step_context.result
        step_context.values["age"] = step_context.result

        msg = (
            "No age given."
            if step_context.result == -1
            else f"I have your age as {age}."
        )

        # We can send messages to the user at any point in the WaterfallStep.
        await step_context.context.send_activity(MessageFactory.text(msg))

        # WaterfallStep always finishes with the end of the Waterfall or
        # with another dialog; here it is a Prompt Dialog.
        return await step_context.prompt(
            ConfirmPrompt.__name__,
            PromptOptions(prompt=MessageFactory.text("Is this ok?")),
        )

    async def summary_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        if step_context.result:
            #  Get the current profile object from user state.  Changes to it
            # will saved during Bot.on_turn.
            user_profile = await self.user_profile_accessor.get(
                step_context.context, UserProfile
            )

            user_profile.transport = step_context.values["transport"]
            user_profile.name = step_context.values["name"]
            user_profile.age = step_context.values["age"]

            msg = f"I have your mode of transport as {user_profile.transport} and your name as {user_profile.name}."
            if user_profile.age != -1:
                msg += f" And age as {user_profile.age}."

            await step_context.context.send_activity(MessageFactory.text(msg))
        else:
            await step_context.context.send_activity(
                MessageFactory.text("Thanks. Your profile will not be kept.")
            )

        # WaterfallStep always finishes with the end of the Waterfall or with another
        # dialog, here it is the end.
        return await step_context.end_dialog()

    @staticmethod
    async def age_prompt_validator(prompt_context: PromptValidatorContext) -> bool:
        # This condition is our validation rule. You can also change the value at this point.
        return (
            prompt_context.recognized.succeeded
            and 0 < prompt_context.recognized.value < 150
        )
