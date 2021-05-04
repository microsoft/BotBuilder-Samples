// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.complexdialog;

import java.util.ArrayList;
import java.util.List;

/**
 * Contains information about a user.
 */
public class UserProfile {
    public String name;
    public Integer age;

    // The list of companies the user wants to review.
    public List<String> companiesToReview = new ArrayList<>();
}
