// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.dialogrootbot;

import com.fasterxml.jackson.annotation.JsonAnyGetter;
import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.HashMap;
import java.util.Map;

public class Body {

    @JsonProperty("type")
    private String type;

    @JsonProperty("url")
    private String url;

    @JsonProperty("size")
    private String size;

    @JsonProperty("spacing")
    private String spacing;

    @JsonProperty("weight")
    private String weight;

    @JsonProperty("text")
    private String text;

    @JsonProperty("wrap")
    private String wrap;

    @JsonProperty("maxLines")
    private Integer maxLines;

    @JsonProperty("color")
    private String color;

    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("type")
    public String getType() {
        return type;
    }

    @JsonProperty("type")
    public void setType(String type) {
        this.type = type;
    }

    @JsonProperty("url")
    public String getUrl() {
        return url;
    }

    @JsonProperty("url")
    public void setUrl(String url) {
        this.url = url;
    }

    @JsonProperty("size")
    public String getSize() {
        return size;
    }

    @JsonProperty("size")
    public void setSize(String size) {
        this.size = size;
    }

    @JsonProperty("spacing")
    public String getSpacing() {
        return spacing;
    }

    @JsonProperty("spacing")
    public void setSpacing(String spacing) {
        this.spacing = spacing;
    }

    @JsonProperty("weight")
    public String getWeight() {
        return weight;
    }

    @JsonProperty("weight")
    public void setWeight(String weight) {
        this.weight = weight;
    }

    @JsonProperty("text")
    public String getText() {
        return text;
    }

    @JsonProperty("text")
    public void setText(String text) {
        this.text = text;
    }

    @JsonProperty("wrap")
    public String getWrap() {
        return wrap;
    }

    @JsonProperty("wrap")
    public void setWrap(String wrap) {
        this.wrap = wrap;
    }

    @JsonProperty("maxLines")
    public Integer getMaxLines() {
        return maxLines;
    }

    @JsonProperty("MaxLines")
    public void setMaxLines(Integer maxLines) {
        this.maxLines = maxLines;
    }

    @JsonAnyGetter
    public Map<String, Object> getAdditionalProperties() {
        return this.additionalProperties;
    }

    @JsonAnySetter
    public void setAdditionalProperty(String name, Object value) {
        this.additionalProperties.put(name, value);
    }
}
