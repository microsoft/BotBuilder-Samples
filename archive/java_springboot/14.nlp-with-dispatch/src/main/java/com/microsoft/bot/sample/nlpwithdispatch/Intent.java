// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.nlpwithdispatch;

import java.util.List;

public class Intent {
    private String name;
    private Double score;
    private List<Intent> childIntents;
    private String topIntent;

    public String getName() {
        return this.name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Double getScore() {
        return this.score;
    }

    public void setScore(Double score) {
        this.score = score;
    }

    public List<Intent> getChildIntents() {
        return this.childIntents;
    }

    public void setChildIntents(List<Intent> childIntents) {
        this.childIntents = childIntents;
    }

    public String getTopIntent() {
        return this.topIntent;
    }

    public void setTopIntent(String topIntent) {
        this.topIntent = topIntent;
    }

}
