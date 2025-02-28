// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.inspection;

/**
 * Custom application state.
 *
 * <p>
 * Any POJO can be used to store bot state.
 * </p>
 *
 * <p>
 * NOTE: Standard Java getters/setters must be used for properties.
 * Alternatively, the Jackson JSON annotations could be used instead. If any
 * methods start with "get" but aren't a property, the Jackson JSON 'JsonIgnore'
 * annotation must be used.
 * </p>
 *
 * @see EchoBot
 */
public class CustomState {
    public int getValue() {
        return value;
    }

    public void setValue(int value) {
        this.value = value;
    }

    private int value;
}
