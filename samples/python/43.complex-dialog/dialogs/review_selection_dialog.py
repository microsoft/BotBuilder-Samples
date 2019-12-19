# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List

from botbuilder.dialogs import (
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
    ComponentDialog,
)
from botbuilder.dialogs.prompts import ChoicePrompt, PromptOptions
from botbuilder.dialogs.choices import Choice, FoundChoice
from botbuilder.core import MessageFactory


class ReviewSelectionDialog(ComponentDialog):
    def __init__(self, dialog_id: str = None):
        super(ReviewSelectionDialog, self).__init__(
            dialog_id or ReviewSelectionDialog.__name__
        )

        self.COMPANIES_SELECTED = "value-companiesSelected"
        self.DONE_OPTION = "done"

        self.company_options = [
            "Adatum Corporation",
            "Contoso Suites",
            "Graphic Design Institute",
            "Wide World Importers",
        ]

        self.add_dialog(ChoicePrompt(ChoicePrompt.__name__))
        self.add_dialog(
            WaterfallDialog(
                WaterfallDialog.__name__, [self.selection_step, self.loop_step]
            )
        )

        self.initial_dialog_id = WaterfallDialog.__name__

    async def selection_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        # step_context.options will contains the value passed in begin_dialog or replace_dialog.
        # if this value wasn't provided then start with an emtpy selection list.  This list will
        # eventually be returned to the parent via end_dialog.
        selected: [
            str
        ] = step_context.options if step_context.options is not None else []
        step_context.values[self.COMPANIES_SELECTED] = selected

        if len(selected) == 0:
            message = (
                f"Please choose a company to review, or `{self.DONE_OPTION}` to finish."
            )
        else:
            message = (
                f"You have selected **{selected[0]}**. You can review an additional company, "
                f"or choose `{self.DONE_OPTION}` to finish. "
            )

        # create a list of options to choose, with already selected items removed.
        options = self.company_options.copy()
        options.append(self.DONE_OPTION)
        if len(selected) > 0:
            options.remove(selected[0])

        # prompt with the list of choices
        prompt_options = PromptOptions(
            prompt=MessageFactory.text(message),
            retry_prompt=MessageFactory.text("Please choose an option from the list."),
            choices=self._to_choices(options),
        )
        return await step_context.prompt(ChoicePrompt.__name__, prompt_options)

    def _to_choices(self, choices: [str]) -> List[Choice]:
        choice_list: List[Choice] = []
        for choice in choices:
            choice_list.append(Choice(value=choice))
        return choice_list

    async def loop_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        selected: List[str] = step_context.values[self.COMPANIES_SELECTED]
        choice: FoundChoice = step_context.result
        done = choice.value == self.DONE_OPTION

        # If they chose a company, add it to the list.
        if not done:
            selected.append(choice.value)

        # If they're done, exit and return their list.
        if done or len(selected) >= 2:
            return await step_context.end_dialog(selected)

        # Otherwise, repeat this dialog, passing in the selections from this iteration.
        return await step_context.replace_dialog(
            ReviewSelectionDialog.__name__, selected
        )
