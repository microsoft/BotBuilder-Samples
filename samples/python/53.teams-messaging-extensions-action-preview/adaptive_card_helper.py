# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import CardFactory
from botbuilder.schema import Attachment


def create_adaptive_card_editor(
    user_text: str = None,
    is_multi_select: bool = False,
    option1: str = None,
    option2: str = None,
    option3: str = None,
) -> Attachment:
    return CardFactory.adaptive_card(
        {
            "actions": [
                {
                    "data": {"submitLocation": "messagingExtensionFetchTask"},
                    "title": "Submit",
                    "type": "Action.Submit",
                }
            ],
            "body": [
                {
                    "text": "This is an Adaptive Card within a Task Module",
                    "type": "TextBlock",
                    "weight": "bolder",
                },
                {"type": "TextBlock", "text": "Enter text for Question:"},
                {
                    "id": "Question",
                    "placeholder": "Question text here",
                    "type": "Input.Text",
                    "value": user_text,
                },
                {"type": "TextBlock", "text": "Options for Question:"},
                {"type": "TextBlock", "text": "Is Multi-Select:"},
                {
                    "choices": [
                        {"title": "True", "value": "true"},
                        {"title": "False", "value": "false"},
                    ],
                    "id": "MultiSelect",
                    "isMultiSelect": "false",
                    "style": "expanded",
                    "type": "Input.ChoiceSet",
                    "value": "true" if is_multi_select else "false",
                },
                {
                    "id": "Option1",
                    "placeholder": "Option 1 here",
                    "type": "Input.Text",
                    "value": option1,
                },
                {
                    "id": "Option2",
                    "placeholder": "Option 2 here",
                    "type": "Input.Text",
                    "value": option2,
                },
                {
                    "id": "Option3",
                    "placeholder": "Option 3 here",
                    "type": "Input.Text",
                    "value": option3,
                },
            ],
            "type": "AdaptiveCard",
            "version": "1.0",
        }
    )


def create_adaptive_card_preview(
    user_text: str = None,
    is_multi_select: bool = False,
    option1: str = None,
    option2: str = None,
    option3: str = None,
) -> Attachment:
    return CardFactory.adaptive_card(
        {
            "actions": [
                {
                    "type": "Action.Submit",
                    "title": "Submit",
                    "data": {"submitLocation": "messagingExtensionSubmit"},
                }
            ],
            "body": [
                {
                    "text": "Adaptive Card from Task Module",
                    "type": "TextBlock",
                    "weight": "bolder",
                },
                {"text": user_text, "type": "TextBlock", "id": "Question"},
                {
                    "id": "Answer",
                    "placeholder": "Answer here...",
                    "type": "Input.Text",
                },
                {
                    "choices": [
                        {"title": option1, "value": option1},
                        {"title": option2, "value": option2},
                        {"title": option3, "value": option3},
                    ],
                    "id": "Choices",
                    "isMultiSelect": is_multi_select,
                    "style": "expanded",
                    "type": "Input.ChoiceSet",
                },
            ],
            "type": "AdaptiveCard",
            "version": "1.0",
        }
    )
