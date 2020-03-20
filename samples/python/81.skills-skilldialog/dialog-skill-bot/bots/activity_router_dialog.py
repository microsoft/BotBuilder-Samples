# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import json
from http import HTTPStatus

from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
    DialogTurnStatus,
)
from botbuilder.core import MessageFactory, InvokeResponse
from botbuilder.schema import Activity, ActivityTypes, InputHints

from config import DefaultConfig
from dialogs import (
    DialogSkillBotRecognizer,
    BookingDialog,
    OAuthTestDialog,
    BookingDetails,
    Location,
)


class ActivityRouterDialog(ComponentDialog):
    """
    A root dialog that can route activities sent to the skill to different dialogs.
    """

    def __init__(
        self, luis_recognizer: DialogSkillBotRecognizer, configuration: DefaultConfig
    ):
        super().__init__(ActivityRouterDialog.__name__)

        self._luis_recognizer = luis_recognizer

        self.add_dialog(BookingDialog())
        self.add_dialog(OAuthTestDialog(configuration))
        self.add_dialog(
            WaterfallDialog(WaterfallDialog.__name__, [self.process_activity])
        )

        self.initial_dialog_id = WaterfallDialog.__name__

    async def process_activity(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        current_activity_type = step_context.context.activity.type

        # A skill can send trace activities if needed
        await step_context.context.send_trace_activity(
            f"{ActivityRouterDialog.__name__}.process_activity()",
            label=f"Got ActivityType: {current_activity_type}",
        )

        if current_activity_type == ActivityTypes.message:
            return await self._on_message_activity(step_context)
        if current_activity_type == ActivityTypes.invoke:
            return await self._on_invoke_activity(step_context)
        if current_activity_type == ActivityTypes.event:
            return await self._on_event_activity(step_context)

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
        activity = step_context.context.activity

        # Resolve what to execute based on the event name.
        if activity.name == "BookFlight":
            booking_details = BookingDetails()
            if activity.value:
                booking_details.from_json(json.loads(activity.value))

            # Start the booking dialog
            booking_dialog = await self.find_dialog(BookingDialog.__name__)
            return await step_context.begin_dialog(booking_dialog.id, booking_details)

        if activity.name == "OAuthTest":
            # Start the oauth dialog
            oauth_dialog = await self.find_dialog(OAuthTestDialog.__name__)
            return await step_context.begin_dialog(oauth_dialog.id, None)

        await step_context.context.send_activity(
            MessageFactory.text(
                f'Unrecognized EventName: "{activity.name}".',
                input_hint=InputHints.ignoring_input,
            )
        )
        return DialogTurnResult(DialogTurnStatus.Complete)

    async def _on_invoke_activity(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        activity = step_context.context.activity

        # Resolve what to execute based on the event name.
        if activity.name == "GetWeather":
            location = Location()
            if activity.value:
                location.from_json(json.loads(activity.value))

            looking_into_it_message = "Getting your weather forecast..."
            await step_context.context.send_activity(
                MessageFactory.text(
                    looking_into_it_message,
                    looking_into_it_message,
                    InputHints.ignoring_input,
                )
            )

            # Create and return an invoke activity with the weather results.
            invoke_response_activity = Activity(
                type="invokeResponse",
                value=InvokeResponse(
                    body=[
                        "New York, NY, Clear, 56 F",
                        "Bellevue, WA, Mostly Cloudy, 48 F",
                    ],
                    status=HTTPStatus.OK,
                ),
            )

            await step_context.context.send_activity(invoke_response_activity)
        else:
            # We didn't get an invoke name we can handle.
            await step_context.context.send_activity(
                MessageFactory.text(
                    f'Unrecognized InvokeName: "{activity.name}".',
                    input_hint=InputHints.ignoring_input,
                )
            )

        return DialogTurnResult(DialogTurnStatus.Complete)

    async def _on_message_activity(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
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
            message += f"Entities found: {len(luis_result.entities) - 1}\n"
            for entity_key, entity_val in luis_result.entities:
                if not entity_key == "$instance":
                    message += f"* {entity_val}\n"

            await step_context.context.send_activity(
                MessageFactory.text(message, input_hint=InputHints.ignoring_input,)
            )

        return DialogTurnResult(DialogTurnStatus.Complete)
