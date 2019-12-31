# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Main dialog. """

from botbuilder.core import BotTelemetryClient, NullTelemetryClient
from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
)
from botbuilder.dialogs.prompts import TextPrompt, PromptOptions
from botbuilder.core import MessageFactory
from booking_details import BookingDetails
from helpers.luis_helper import LuisHelper
from .booking_dialog import BookingDialog


class MainDialog(ComponentDialog):
    """Main dialog. """

    def __init__(
        self,
        configuration: dict,
        dialog_id: str = None,
        telemetry_client: BotTelemetryClient = NullTelemetryClient(),
    ):
        super(MainDialog, self).__init__(dialog_id or MainDialog.__name__)

        self._configuration = configuration
        self.telemetry_client = telemetry_client

        text_prompt = TextPrompt(TextPrompt.__name__)
        text_prompt.telemetry_client = self.telemetry_client

        booking_dialog = BookingDialog(telemetry_client=self._telemetry_client)
        booking_dialog.telemetry_client = self.telemetry_client

        wf_dialog = WaterfallDialog(
            "WFDialog", [self.intro_step, self.act_step, self.final_step]
        )
        wf_dialog.telemetry_client = self.telemetry_client

        self.add_dialog(text_prompt)
        self.add_dialog(booking_dialog)
        self.add_dialog(wf_dialog)

        self.initial_dialog_id = "WFDialog"

    async def intro_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        """Initial prompt."""
        if (
            not self._configuration.get("LUIS_APP_ID", "")
            or not self._configuration.get("LUIS_API_KEY", "")
            or not self._configuration.get("LUIS_API_HOST_NAME", "")
        ):
            await step_context.context.send_activity(
                MessageFactory.text(
                    "NOTE: LUIS is not configured. To enable all"
                    " capabilities, add 'LUIS_APP_ID', 'LUIS_API_KEY' and 'LUIS_API_HOST_NAME'"
                    " to the config.py file."
                )
            )

            return await step_context.next(None)

        return await step_context.prompt(
            TextPrompt.__name__,
            PromptOptions(
                prompt=MessageFactory.text("What can I help you with today?")
            ),
        )

    async def act_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        """Use language understanding to gather details about booking."""
        # Call LUIS and gather any potential booking details. (Note the TurnContext
        # has the response to the prompt.)
        booking_details = (
            await LuisHelper.execute_luis_query(
                self._configuration, step_context.context, self.telemetry_client
            )
            if step_context.result is not None
            else BookingDetails()
        )  # pylint: disable=bad-continuation

        # In this sample we only have a single Intent we are concerned with. However,
        # typically a scenario will have multiple different Intents each corresponding
        # to starting a different child Dialog.

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
