// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamstaskmodule.models;

public class UISettings {
    int width;
    int height;
    String title;
    String id;
    String buttonTitle;

    public UISettings(
        int withWidth,
        int withHeight,
        String withTitle,
        String withId,
        String withButtonTitle
    ) {
        width = withWidth;
        height = withHeight;
        title = withTitle;
        id = withId;
        buttonTitle = withButtonTitle;
    }

    public int getWidth() {
        return width;
    }

    public void setWidth(int width) {
        this.width = width;
    }

    public int getHeight() {
        return height;
    }

    public void setHeight(int height) {
        this.height = height;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getButtonTitle() {
        return buttonTitle;
    }

    public void setButtonTitle(String buttonTitle) {
        this.buttonTitle = buttonTitle;
    }
}
