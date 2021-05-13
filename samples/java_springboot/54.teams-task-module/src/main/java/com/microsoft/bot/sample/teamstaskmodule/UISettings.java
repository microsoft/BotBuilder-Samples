// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamstaskmodule;

public class UISettings {
    private int width;
    private int height;
    private String title;
    private String id;
    private String buttonTitle;

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

    public void setWidth(int withWidth) {
        this.width = withWidth;
    }

    public int getHeight() {
        return height;
    }

    public void setHeight(int withHeight) {
        this.height = withHeight;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String withTitle) {
        this.title = withTitle;
    }

    public String getId() {
        return id;
    }

    public void setId(String withId) {
        this.id = withId;
    }

    public String getButtonTitle() {
        return buttonTitle;
    }

    public void setButtonTitle(String withButtonTitle) {
        this.buttonTitle = withButtonTitle;
    }
}
