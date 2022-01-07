// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.nlpwithdispatch;

import java.util.List;

public class PredictionResult {
    private String topIntent;
    private List<Intent> intents;

    public String getTopIntent() {
        return this.topIntent;
    }

    public void setTopIntent(String topIntent) {
        this.topIntent = topIntent;
    }

    public List<Intent> getIntents() {
        return this.intents;
    }

    public void setIntents(List<Intent> intents) {
        this.intents = intents;
    }
}
