// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.dialogs;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

public class Location {

    @JsonProperty(value = "latitude")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private Double latitude;

    @JsonProperty(value = "longitude")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private Double longitude;

    @JsonProperty(value = "postalCode")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String postalCode;

    /**
     * @return the Latitude value as a double?.
     */
    public Double getLatitude() {
        return this.latitude;
    }

    /**
     * @param withLatitude The Latitude value.
     */
    public void setLatitude(Double withLatitude) {
        this.latitude = withLatitude;
    }

    /**
     * @return the Longitude value as a double?.
     */
    public Double getLongitude() {
        return this.longitude;
    }

    /**
     * @param withLongitude The Longitude value.
     */
    public void setLongitude(Double withLongitude) {
        this.longitude = withLongitude;
    }

    /**
     * @return the PostalCode value as a String.
     */
    public String getPostalCode() {
        return this.postalCode;
    }

    /**
     * @param withPostalCode The PostalCode value.
     */
    public void setPostalCode(String withPostalCode) {
        this.postalCode = withPostalCode;
    }
}
