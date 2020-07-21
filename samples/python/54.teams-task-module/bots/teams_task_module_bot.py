# Copyright (c) Microsoft Corp. All rights reserved.
# Licensed under the MIT License.

import json
import os

from botbuilder.core import (
    CardFactory,
    MessageFactory,
    TurnContext,
)
from botbuilder.schema import HeroCard, Attachment, CardAction
from botbuilder.schema.teams import (
    TaskModuleMessageResponse,
    TaskModuleRequest,
    TaskModuleResponse,
    TaskModuleTaskInfo,
)
from botbuilder.core.teams import TeamsActivityHandler

from config import DefaultConfig
from models import (
    UISettings,
    TaskModuleUIConstants,
    TaskModuleIds,
    TaskModuleResponseFactory,
)


class TeamsTaskModuleBot(TeamsActivityHandler):
    def __init__(self, config: DefaultConfig):
        self.__base_url = config.BASE_URL

    async def on_message_activity(self, turn_context: TurnContext):
        reply = MessageFactory.list(
            [
                TeamsTaskModuleBot.__get_task_module_hero_card_options(),
                TeamsTaskModuleBot.__get_task_module_adaptive_card_options(),
            ]
        )
        await turn_context.send_activity(reply)

    async def on_teams_task_module_fetch(
        self, turn_context: TurnContext, task_module_request: TaskModuleRequest
    ) -> TaskModuleResponse:
        card_task_fetch_value = task_module_request.data["data"]

        task_info = TaskModuleTaskInfo()
        if card_task_fetch_value == TaskModuleIds.YOUTUBE:
            task_info.url = task_info.fallback_url = (
                self.__base_url + "/" + TaskModuleIds.YOUTUBE + ".html"
            )
            TeamsTaskModuleBot.__set_task_info(task_info, TaskModuleUIConstants.YOUTUBE)
        elif card_task_fetch_value == TaskModuleIds.CUSTOM_FORM:
            task_info.url = task_info.fallback_url = (
                self.__base_url + "/" + TaskModuleIds.CUSTOM_FORM + ".html"
            )
            TeamsTaskModuleBot.__set_task_info(
                task_info, TaskModuleUIConstants.CUSTOM_FORM
            )
        elif card_task_fetch_value == TaskModuleIds.ADAPTIVE_CARD:
            task_info.card = TeamsTaskModuleBot.__create_adaptive_card_attachment()
            TeamsTaskModuleBot.__set_task_info(
                task_info, TaskModuleUIConstants.ADAPTIVE_CARD
            )

        return TaskModuleResponseFactory.to_task_module_response(task_info)

    async def on_teams_task_module_submit(
        self, turn_context: TurnContext, task_module_request: TaskModuleRequest
    ) -> TaskModuleResponse:
        await turn_context.send_activity(
            MessageFactory.text(
                f"on_teams_task_module_submit: {json.dumps(task_module_request.data)}"
            )
        )

        message_response = TaskModuleMessageResponse(value="Thanks!")
        return TaskModuleResponse(task=message_response)

    @staticmethod
    def __set_task_info(task_info: TaskModuleTaskInfo, ui_constants: UISettings):
        task_info.height = ui_constants.height
        task_info.width = ui_constants.width
        task_info.title = ui_constants.title

    @staticmethod
    def __get_task_module_hero_card_options() -> Attachment:
        buttons = [
            CardAction(
                type="invoke",
                title=card_type.button_title,
                value={"type": "task/fetch", "data": card_type.id},
            )
            for card_type in [
                TaskModuleUIConstants.ADAPTIVE_CARD,
                TaskModuleUIConstants.CUSTOM_FORM,
                TaskModuleUIConstants.YOUTUBE,
            ]
        ]

        card = HeroCard(title="Task Module Invocation from Hero Card", buttons=buttons,)
        return CardFactory.hero_card(card)

    @staticmethod
    def __get_task_module_adaptive_card_options() -> Attachment:
        adaptive_card = {
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "version": "1.0",
            "type": "AdaptiveCard",
            "body": [
                {
                    "type": "TextBlock",
                    "text": "Task Module Invocation from Adaptive Card",
                    "weight": "bolder",
                    "size": 3,
                },
            ],
            "actions": [
                {
                    "type": "Action.Submit",
                    "title": card_type.button_title,
                    "data": {"msteams": {"type": "task/fetch"}, "data": card_type.id},
                }
                for card_type in [
                    TaskModuleUIConstants.ADAPTIVE_CARD,
                    TaskModuleUIConstants.CUSTOM_FORM,
                    TaskModuleUIConstants.YOUTUBE,
                ]
            ],
        }

        return CardFactory.adaptive_card(adaptive_card)

    @staticmethod
    def __create_adaptive_card_attachment() -> Attachment:
        card_path = os.path.join(os.getcwd(), "resources/adaptiveCard.json")
        with open(card_path, "rb") as in_file:
            card_data = json.load(in_file)

        return CardFactory.adaptive_card(card_data)
