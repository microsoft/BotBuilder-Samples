// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamstaskmodule;

public final class TaskModuleUIConstants {
    private TaskModuleUIConstants() {

    }

    public static final UISettings YOUTUBE = new UISettings(
        1000,
        700,
        "YouTube Video",
        TaskModuleIds.YOUTUBE,
        "YouTube"
    );

    public static final UISettings CUSTOMFORM = new UISettings(
        510,
        450,
        "Custom Form",
        TaskModuleIds.CUSTOMFORM,
        "Custom Form"
    );

    public static final UISettings ADAPTIVECARD = new UISettings(
        400,
        200,
        "Adaptive Card: Inputs",
        TaskModuleIds.ADAPTIVECARD,
        "Adaptive Card"
    );
}
