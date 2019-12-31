# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
)
from botbuilder.core import MessageFactory, UserState

from data_models import UserProfile
from dialogs.top_level_dialog import TopLevelDialog


class MainDialog(ComponentDialog):
    def __init__(self, user_state: UserState):
        super(MainDialog, self).__init__(MainDialog.__name__)

        self.user_state = user_state

        self.add_dialog(TopLevelDialog(TopLevelDialog.__name__))
        self.add_dialog(
            WaterfallDialog("WFDialog", [self.initial_step, self.final_step])
        )

        self.initial_dialog_id = "WFDialog"

    async def initial_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        return await step_context.begin_dialog(TopLevelDialog.__name__)

    async def final_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        user_info: UserProfile = step_context.result

        companies = (
            "no companies"
            if len(user_info.companies_to_review) == 0
            else " and ".join(user_info.companies_to_review)
        )
        status = f"You are signed up to review {companies}."

        await step_context.context.send_activity(MessageFactory.text(status))

        # store the UserProfile
        accessor = self.user_state.create_property("UserProfile")
        await accessor.set(step_context.context, user_info)

        return await step_context.end_dialog()
