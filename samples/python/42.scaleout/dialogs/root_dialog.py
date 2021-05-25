# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import MessageFactory
from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
    NumberPrompt,
    PromptOptions,
)


class RootDialog(ComponentDialog):
    def __init__(self):
        super(RootDialog, self).__init__(RootDialog.__name__)

        self.add_dialog(self.__create_waterfall())
        self.add_dialog(NumberPrompt("number"))

        self.initial_dialog_id = "waterfall"

    def __create_waterfall(self) -> WaterfallDialog:
        return WaterfallDialog("waterfall", [self.__step1, self.__step2, self.__step3])

    async def __step1(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        return await step_context.prompt(
            "number", PromptOptions(prompt=MessageFactory.text("Enter a number."))
        )

    async def __step2(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        first: int = step_context.result
        step_context.values["first"] = first

        return await step_context.prompt(
            "number",
            PromptOptions(
                prompt=MessageFactory.text(f"I have {first}, now enter another number")
            ),
        )

    async def __step3(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        first: int = step_context.values["first"]
        second: int = step_context.result

        await step_context.prompt(
            "number",
            PromptOptions(
                prompt=MessageFactory.text(
                    f"The result of the first minus the second is {first - second}."
                )
            ),
        )

        return await step_context.end_dialog()
