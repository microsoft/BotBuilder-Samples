// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamstaskmodule.models;

import com.fasterxml.jackson.annotation.JsonProperty;
import java.util.HashMap;

public class AdaptiveCardTaskFetchValue<T> {
    @JsonProperty(value = "msteams")
    private Object type = new HashMap<String, String>() {{
       put("type", "task/fetch");
    }};

    @JsonProperty(value = "data")
    private T data;

    /**
     * Gets the type of the task fetch value.
     * @return The fetch value.
     */
    public Object getType() {
        return type;
    }

    /**
     * Sets the type of the task fetch value.
     * @param type The type.
     */
    public void setType(Object type) {
        this.type = type;
    }

    /**
     * Gets the fetch data.
     * @return The fetch data.
     */
    public T getData() {
        return data;
    }

    /**
     * Sets the fetch data.
     * @param data The data.
     */
    public void setData(T data) {
        this.data = data;
    }
}
