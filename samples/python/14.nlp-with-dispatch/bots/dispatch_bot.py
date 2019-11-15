# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from azure.cognitiveservices.language.luis.runtime.models import LuisResult
from flask import Config

from botbuilder.ai.luis import LuisApplication, LuisRecognizer, LuisPredictionOptions
from botbuilder.ai.qna import QnAMaker, QnAMakerEndpoint
from botbuilder.core import ActivityHandler, TurnContext, RecognizerResult
from botbuilder.schema import ChannelAccount


class DispatchBot(ActivityHandler):
    def __init__(self, config: Config):
        self.qna_maker = QnAMaker(
            QnAMakerEndpoint(
                knowledge_base_id=config["QNA_KNOWLEDGEBASE_ID"],
                endpoint_key=config["QNA_ENDPOINT_KEY"],
                host=config["QNA_ENDPOINT_HOST"],
            )
        )

        # If the includeApiResults parameter is set to true, as shown below, the full response
        # from the LUIS api will be made available in the properties  of the RecognizerResult
        luis_application = LuisApplication(
            config["LUIS_APP_ID"],
            config["LUIS_API_KEY"],
            "https://" + config["LUIS_API_HOST_NAME"],
        )
        luis_options = LuisPredictionOptions(
            include_all_intents=True, include_instance_data=True
        )
        self.recognizer = LuisRecognizer(luis_application, luis_options, True)

    async def on_members_added_activity(
        self, members_added: [ChannelAccount], turn_context: TurnContext
    ):
        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    f"Welcome to Dispatch bot {member.name}. Type a greeting or a "
                    f"question about the weather to get started."
                )

    async def on_message_activity(self, turn_context: TurnContext):
        # First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
        recognizer_result = await self.recognizer.recognize(turn_context)

        # Top intent tell us which cognitive service to use.
        intent = LuisRecognizer.top_intent(recognizer_result)

        # Next, we call the dispatcher with the top intent.
        await self._dispatch_to_top_intent(turn_context, intent, recognizer_result)

    async def _dispatch_to_top_intent(
        self, turn_context: TurnContext, intent, recognizer_result: RecognizerResult
    ):
        if intent == "l_HomeAutomation":
            await self._process_home_automation(
                turn_context, recognizer_result.properties["luisResult"]
            )
        elif intent == "l_Weather":
            await self._process_weather(
                turn_context, recognizer_result.properties["luisResult"]
            )
        elif intent == "q_sample-qna":
            await self._process_sample_qna(turn_context)
        else:
            await turn_context.send_activity(f"Dispatch unrecognized intent: {intent}.")

    async def _process_home_automation(
        self, turn_context: TurnContext, luis_result: LuisResult
    ):
        await turn_context.send_activity(
            f"HomeAutomation top intent {luis_result.top_scoring_intent}."
        )

        intents_list = "\n\n".join(
            [intent_obj.intent for intent_obj in luis_result.intents]
        )
        await turn_context.send_activity(
            f"HomeAutomation intents detected: {intents_list}."
        )

        if luis_result.entities:
            entities_list = "\n\n".join(
                [entity_obj.entity for entity_obj in luis_result.entities]
            )
            await turn_context.send_activity(
                f"HomeAutomation entities were found in the message: {entities_list}."
            )

    async def _process_weather(
        self, turn_context: TurnContext, luis_result: LuisResult
    ):
        await turn_context.send_activity(
            f"ProcessWeather top intent {luis_result.top_scoring_intent}."
        )

        intents_list = "\n\n".join(
            [intent_obj.intent for intent_obj in luis_result.intents]
        )
        await turn_context.send_activity(
            f"ProcessWeather intents detected: {intents_list}."
        )

        if luis_result.entities:
            entities_list = "\n\n".join(
                [entity_obj.entity for entity_obj in luis_result.entities]
            )
            await turn_context.send_activity(
                f"ProcessWeather entities were found in the message: {entities_list}."
            )

    async def _process_sample_qna(self, turn_context: TurnContext):
        results = await self.qna_maker.get_answers(turn_context)

        if results:
            await turn_context.send_activity(results[0].answer)
        else:
            await turn_context.send_activity(
                "Sorry, could not find an answer in the Q and A system."
            )
