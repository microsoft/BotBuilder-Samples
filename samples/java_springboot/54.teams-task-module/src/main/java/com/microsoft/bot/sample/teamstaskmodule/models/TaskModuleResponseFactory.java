// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamstaskmodule.models;

import com.microsoft.bot.schema.teams.TaskModuleContinueResponse;
import com.microsoft.bot.schema.teams.TaskModuleMessageResponse;
import com.microsoft.bot.schema.teams.TaskModuleResponse;
import com.microsoft.bot.schema.teams.TaskModuleTaskInfo;

public final class TaskModuleResponseFactory {
    public static TaskModuleResponse createResponse(String message) {
        TaskModuleMessageResponse taskModuleMessageResponse = new TaskModuleMessageResponse();
        taskModuleMessageResponse.setValue(message);
        TaskModuleResponse taskModuleResponse = new TaskModuleResponse();
        taskModuleResponse.setTask(taskModuleMessageResponse);
        return taskModuleResponse;
    }

    public static TaskModuleResponse createResponse(TaskModuleTaskInfo taskInfo) {
        TaskModuleContinueResponse taskModuleContinueResponse = new TaskModuleContinueResponse();
        taskModuleContinueResponse.setValue(taskInfo);
        TaskModuleResponse taskModuleResponse = new TaskModuleResponse();
        taskModuleResponse.setTask(taskModuleContinueResponse);
        return taskModuleResponse;
    }

    public static TaskModuleResponse toTaskModuleResponse(TaskModuleTaskInfo taskInfo) {
        return createResponse(taskInfo);
    }
}
