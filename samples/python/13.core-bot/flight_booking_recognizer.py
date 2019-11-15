# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.ai.luis import LuisApplication, LuisRecognizer
from botbuilder.core import Recognizer, RecognizerResult, TurnContext


class FlightBookingRecognizer(Recognizer):
    def __init__(self, configuration: dict):
        self._recognizer = None

        luis_is_configured = (
            configuration["LUIS_APP_ID"]
            and configuration["LUIS_API_KEY"]
            and configuration["LUIS_API_HOST_NAME"]
        )
        if luis_is_configured:
            luis_application = LuisApplication(
                configuration["LUIS_APP_ID"],
                configuration["LUIS_API_KEY"],
                "https://" + configuration["LUIS_API_HOST_NAME"],
            )

            self._recognizer = LuisRecognizer(luis_application)

    @property
    def is_configured(self) -> bool:
        # Returns true if luis is configured in the appsettings.json and initialized.
        return self._recognizer is not None

    async def recognize(self, turn_context: TurnContext) -> RecognizerResult:
        return await self._recognizer.recognize(turn_context)
