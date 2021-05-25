# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import json

from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
    DialogTurnStatus,
)
from botbuilder.core import MessageFactory
from botbuilder.schema import ActivityTypes, InputHints

from .dialog_skill_bot_recognizer import DialogSkillBotRecognizer
from .booking_dialog import BookingDialog
from .booking_details import BookingDetails
from .location import Location


class ActivityRouterDialog(ComponentDialog):
    """
    A root dialog that can route activities sent to the skill to different sub-dialogs.
    """

    def __init__(self, luis_recognizer: DialogSkillBotRecognizer):
        super().__init__(ActivityRouterDialog.__name__)

        self._luis_recognizer = luis_recognizer

        self.add_dialog(BookingDialog())
        self.add_dialog(
            WaterfallDialog(WaterfallDialog.__name__, [self.process_activity])
        )

        self.initial_dialog_id = WaterfallDialog.__name__

    async def process_activity(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        current_activity_type = step_context.context.activity.type

        # A skill can send trace activities, if needed.
        await step_context.context.send_trace_activity(
            f"{ActivityRouterDialog.__name__}.process_activity()",
            label=f"Got ActivityType: {current_activity_type}",
        )

        if current_activity_type == ActivityTypes.event:
            return await self._on_event_activity(step_context)
        if current_activity_type == ActivityTypes.message:
            return await self._on_message_activity(step_context)
        else:
            # We didn't get an activity type we can handle.
            await step_context.context.send_activity(
                MessageFactory.text(
                    f'Unrecognized ActivityType: "{current_activity_type}".',
                    input_hint=InputHints.ignoring_input,
                )
            )
            return DialogTurnResult(DialogTurnStatus.Complete)

    async def _on_event_activity(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        """
        This method performs different tasks based on the event name.
        """

        activity = step_context.context.activity

        # Resolve what to execute based on the event name.
        if activity.name == "BookFlight":
            return await self._begin_book_flight(step_context)

        if activity.name == "GetWeather":
            return await self._begin_get_weather(step_context)

        # We didn't get an activity name we can handle.
        await step_context.context.send_activity(
            MessageFactory.text(
                f'Unrecognized ActivityName: "{activity.name}".',
                input_hint=InputHints.ignoring_input,
            )
        )
        return DialogTurnResult(DialogTurnStatus.Complete)

    async def _on_message_activity(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        """
        This method just gets a message activity and runs it through LUIS.
        """

        activity = step_context.context.activity

        if not self._luis_recognizer.is_configured:
            await step_context.context.send_activity(
                MessageFactory.text(
                    "NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and"
                    " 'LuisAPIHostName' to the config.py file.",
                    input_hint=InputHints.ignoring_input,
                )
            )
        else:
            # Call LUIS with the utterance.
            luis_result = await self._luis_recognizer.recognize(step_context.context)

            message = f'LUIS results for "{activity.Text}":\n'
            intent, intent_score = None, None
            if luis_result.intents:
                max_value_key = max(
                    luis_result.intents, key=lambda key: luis_result.intents[key]
                )
                intent, intent_score = max_value_key, luis_result.intents[max_value_key]

            message += f'Intent: "{intent}" Score: {intent_score}\n'

            await step_context.context.send_activity(
                MessageFactory.text(message, input_hint=InputHints.ignoring_input,)
            )

            # Start a dialog if we recognize the intent.
            top_intent = luis_result.get_top_scoring_intent().intent

            if top_intent == "BookFlight":
                return await self._begin_book_flight(step_context)

            if top_intent == "GetWeather":
                return await self._begin_get_weather(step_context)

            # Catch all for unhandled intents.
            didnt_understand_message_text = f"Sorry, I didn't get that. Please try asking in a different way (intent was {top_intent})"
            await step_context.context.send_activity(
                MessageFactory.text(
                    didnt_understand_message_text,
                    didnt_understand_message_text,
                    input_hint=InputHints.ignoring_input,
                )
            )

        return DialogTurnResult(DialogTurnStatus.Complete)

    async def _begin_get_weather(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        activity = step_context.context.activity
        location = Location()
        if activity.value:
            location.from_json(activity.value)

        # We haven't implemented the GetWeatherDialog so we just display a TODO message.
        get_weather_message = f"TODO: get weather for here (lat: {location.latitude}, long: {location.longitude}"
        await step_context.context.send_activity(
            MessageFactory.text(
                get_weather_message, get_weather_message, InputHints.ignoring_input,
            )
        )

        return DialogTurnResult(DialogTurnStatus.Complete)

    async def _begin_book_flight(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        activity = step_context.context.activity
        booking_details = BookingDetails()
        if activity.value:
            booking_details.from_json(activity.value)

        # Start the booking dialog
        booking_dialog = await self.find_dialog(BookingDialog.__name__)
        return await step_context.begin_dialog(booking_dialog.id, booking_details)
