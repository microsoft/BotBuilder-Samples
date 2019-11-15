# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Main dialog. """
from typing import Dict
from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
)
from botbuilder.dialogs.prompts import TextPrompt, PromptOptions
from botbuilder.core import MessageFactory
from model_corebot101.booking_details import BookingDetails
from model_corebot101.language_helper import LanguageHelper
from .booking_dialog import BookingDialog

# pylint:disable=line-too-long
class MainDialog(ComponentDialog):
    """Main dialog."""

    def __init__(
        self,
        configuration: Dict[str, object],
        dialog_id: str = None,
        language_helper: LanguageHelper = None,
    ):
        super(MainDialog, self).__init__(dialog_id or MainDialog.__name__)

        self._configuration = configuration
        self._language_helper = language_helper

        self.add_dialog(TextPrompt(TextPrompt.__name__))
        self.add_dialog(BookingDialog())
        self.add_dialog(
            WaterfallDialog(
                "WFDialog", [self.intro_step, self.act_step, self.final_step]
            )
        )

        self.initial_dialog_id = "WFDialog"

        # Initialize language models if we're not using service
        self._language_helper = LanguageHelper()
        if not self._configuration["USE_MODEL_RUNTIME_SERVICE"]:
            self._language_helper.initialize_models()

    async def intro_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        """Initial prompt."""
        return await step_context.prompt(
            TextPrompt.__name__,
            PromptOptions(
                prompt=MessageFactory.text("What can I help you with today?")
            ),
        )

    async def act_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        """Use language understanding to gather details about booking.
        In this sample we only have a single Intent we are concerned with.
        However, typically a scenario will have multiple different Intents
        each corresponding to starting a different child Dialog."""

        if step_context.result is not None:
            utterance = step_context.context.activity.text
            if not self._configuration["USE_MODEL_RUNTIME_SERVICE"]:
                booking_details = await self._language_helper.excecute_query_inproc(
                    utterance
                )
            else:
                booking_details = await self._language_helper.excecute_query_service(
                    self._configuration, utterance
                )
        else:
            booking_details = BookingDetails()

        # Run the BookingDialog giving it whatever details we have from the
        # model.  The dialog will prompt to find out the remaining details.
        return await step_context.begin_dialog(BookingDialog.__name__, booking_details)

    async def final_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        """Complete dialog.
        At this step, with details from the user, display the completed
        flight booking to the user.
        """
        # If the child dialog ("BookingDialog") was cancelled or the user failed
        # to confirm, the Result here will be null.
        if step_context.result is not None:
            result = step_context.result

            # Now we have all the booking details call the booking service.
            # If the call to the booking service was successful tell the user.
            # time_property = Timex(result.travel_date)
            # travel_date_msg = time_property.to_natural_language(datetime.now())
            msg = (
                f"I have you booked to {result.destination} from"
                f" {result.origin} on {result.travel_date}."
            )
            await step_context.context.send_activity(MessageFactory.text(msg))
        else:
            await step_context.context.send_activity(MessageFactory.text("Thank you."))
        return await step_context.end_dialog()
