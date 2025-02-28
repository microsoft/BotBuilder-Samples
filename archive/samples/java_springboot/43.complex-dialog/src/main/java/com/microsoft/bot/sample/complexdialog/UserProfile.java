// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.complexdialog;

import java.util.ArrayList;
import java.util.List;

/**
 * Contains information about a user.
 */
public class UserProfile {
    private String name;
    private Integer age;

    // The list of companies the user wants to review.
    private List<String> companiesToReview = new ArrayList<>();

    /**
     * Gets the name of the user
     * @return Name of the user
     */
    public String getName() {
        return name;
    }

    /**
     * Sets the name of the user
     * @param name Name of the user
     */
    public void setName(String name) {
        this.name = name;
    }

    /**
     * Gets the age of the user
     * @return Age of the user
     */
    public Integer getAge() {
        return age;
    }

    /**
     * Sets the age of the user
     * @param age Age of the user
     */
    public void setAge(Integer age) {
        this.age = age;
    }

    /**
     * Gets the list of companies
     * @return A list of companies
     */
    public List<String> getCompaniesToReview() {
        return companiesToReview;
    }

    /**
     * Sets a list of companies
     * @param companiesToReview A list of companies
     */
    public void setCompaniesToReview(List<String> companiesToReview) {
        this.companiesToReview = companiesToReview;
    }
}
