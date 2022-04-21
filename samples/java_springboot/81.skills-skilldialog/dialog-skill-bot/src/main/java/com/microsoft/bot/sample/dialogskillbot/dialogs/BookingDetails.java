// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.dialogs;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

public class BookingDetails {

    @JsonProperty(value = "destination")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String destination;

    @JsonProperty(value = "origin")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String origin;

    @JsonProperty(value = "travelDate")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String travelDate;

    /**
     * @return the Destination value as a String.
     */
    public String getDestination() {
        return this.destination;
    }

    /**
     * @param withDestination The Destination value.
     */
    public void setDestination(String withDestination) {
        this.destination = withDestination;
    }

    /**
     * @return the Origin value as a String.
     */
    public String getOrigin() {
        return this.origin;
    }

    /**
     * @param withOrigin The Origin value.
     */
    public void setOrigin(String withOrigin) {
        this.origin = withOrigin;
    }

    /**
     * @return the TravelDate value as a String.
     */
    public String getTravelDate() {
        return this.travelDate;
    }

    /**
     * @param withTravelDate The TravelDate value.
     */
    public void setTravelDate(String withTravelDate) {
        this.travelDate = withTravelDate;
    }

}

