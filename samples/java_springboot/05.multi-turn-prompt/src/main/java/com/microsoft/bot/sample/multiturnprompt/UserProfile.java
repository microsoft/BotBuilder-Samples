// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.multiturnprompt;

import com.microsoft.bot.schema.Attachment;

/**
 * This is our application state.
 */
public class UserProfile {
    public String transport;
    public String name;
    public Integer age;
    public Attachment picture;
}
