# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from enum import Enum


class TranslationSettings(str, Enum):
    default_language = "en"
    english_english = "en"
    english_spanish = "es"
    spanish_english = "in"
    spanish_spanish = "it"
