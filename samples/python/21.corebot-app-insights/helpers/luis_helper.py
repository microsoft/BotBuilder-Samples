# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""Helper to call LUIS service."""
from botbuilder.ai.luis import LuisRecognizer, LuisApplication, LuisPredictionOptions
from botbuilder.core import TurnContext, BotTelemetryClient, NullTelemetryClient

from booking_details import BookingDetails

# pylint: disable=line-too-long
class LuisHelper:
    """LUIS helper implementation."""

    @staticmethod
    async def execute_luis_query(
        configuration,
        turn_context: TurnContext,
        telemetry_client: BotTelemetryClient = None,
    ) -> BookingDetails:
        """Invoke LUIS service to perform prediction/evaluation of utterance."""
        booking_details = BookingDetails()

        # pylint:disable=broad-except
        try:
            luis_application = LuisApplication(
                configuration.get("LUIS_APP_ID"),
                configuration.get("LUIS_API_KEY"),
                configuration.get("LUIS_API_HOST_NAME"),
            )
            options = LuisPredictionOptions()
            options.telemetry_client = (
                telemetry_client
                if telemetry_client is not None
                else NullTelemetryClient()
            )
            recognizer = LuisRecognizer(luis_application, prediction_options=options)
            recognizer_result = await recognizer.recognize(turn_context)
            print(f"Recognize Result: {recognizer_result}")

            if recognizer_result.intents:
                intent = sorted(
                    recognizer_result.intents,
                    key=recognizer_result.intents.get,
                    reverse=True,
                )[:1][0]
                if intent == "Book_flight":
                    # We need to get the result from the LUIS JSON which at every level returns an array.
                    to_entities = recognizer_result.entities.get("$instance", {}).get(
                        "To", []
                    )
                    if to_entities:
                        booking_details.destination = to_entities[0]["text"]
                    from_entities = recognizer_result.entities.get("$instance", {}).get(
                        "From", []
                    )
                    if from_entities:
                        booking_details.origin = from_entities[0]["text"]

                    # This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
                    # TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                    date_entities = recognizer_result.entities.get("$instance", {}).get(
                        "datetime", []
                    )
                    if date_entities:
                        booking_details.travel_date = (
                            None  # Set when we get a timex format
                        )
        except Exception as exception:
            print(exception)

        return booking_details
