# Copyright (c) Microsoft Corp. All rights reserved.
# Licensed under the MIT License.

from .ui_settings import UISettings
from .task_module_ids import TaskModuleIds


class TaskModuleUIConstants:
    YOUTUBE = UISettings(1000, 700, "YouTube Video", TaskModuleIds.YOUTUBE, "YouTube")
    CUSTOM_FORM = UISettings(
        510, 450, "Custom Form", TaskModuleIds.CUSTOM_FORM, "Custom Form"
    )
    ADAPTIVE_CARD = UISettings(
        400, 200, "Adaptive Card: Inputs", TaskModuleIds.ADAPTIVE_CARD, "Adaptive Card",
    )
